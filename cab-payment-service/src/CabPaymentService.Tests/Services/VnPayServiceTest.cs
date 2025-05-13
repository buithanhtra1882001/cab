using AutoMapper;
using Bogus;
using CAB.BuildingBlocks.EventBus.Abstractions;
using CAB.BuildingBlocks.EventBus.Events;
using CabPaymentService.Infrastructures.Communications;
using CabPaymentService.Infrastructures.Configurations;
using CabPaymentService.Infrastructures.Constants;
using CabPaymentService.Infrastructures.DbContexts;
using CabPaymentService.Infrastructures.IntegrationEvents;
using CabPaymentService.Infrastructures.Repositories;
using CabPaymentService.Infrastructures.Repositories.Interfaces;
using CabPaymentService.Model.Dtos;
using CabPaymentService.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CabPaymentService.Tests.TestVnPayService;

public class VnPayServiceTest
{
    private readonly Services.VnPayService _vnPayService;
    private readonly VnPayTransactionRepository _vnPayTransactionRepository;
    private readonly BillInfoRepository _billInfoRepository;
    private readonly InvoiceRepository _invoiceRepository;
    private readonly Mock<IEventBus> _eventBus;

    public VnPayServiceTest()
    {
        var mockLogger = new Mock<ILogger<VnPayService>>();
        var vnPayConfig = new VnPayConfiguration()
        {
            Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
            Version = "2.1.0",
            HashSecret = "PWAKRUONSTULCMTFUDGVLELZQRCMEUFX",
            HashType = "HMACSHA512",
            QueryDr = "https://sandbox.vnpayment.vn/merchant_webapi/merchant.html",
            ReturnUrl = "http://localhost:8080/return-page",
            TmnCode = "CABVN001"
        };
        var context = new PostgresDbContext(FakeObjects.GetConfiguration());
        var options = new DbContextOptionsBuilder<PostgresDbContext>()
            .UseInMemoryDatabase(databaseName: "UserDb")
            .Options;
        var _commissionConfig = new CommissionConfig() { High = 0.20m, Medium = .15m, Low = .1m };
        _vnPayTransactionRepository = new VnPayTransactionRepository(new PostgresDbContext(configuration: FakeObjects.GetConfiguration(), options));
        _billInfoRepository = new BillInfoRepository(context);
        _invoiceRepository = new InvoiceRepository(context);
        var mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
        var queryDrResponse = new HttpResponseMessage()
        { StatusCode = HttpStatusCode.OK, Content = new StringContent("vnp_ResponseCode=00&vnp_Amount=999999") };
        mockHttpClientWrapper.Setup(_ => _.GetAsync("MockQueryDrRequest", null, null, null).Result)
            .Returns(queryDrResponse);
        var mockMapper = new Mock<IMapper>();
        _eventBus = new Mock<IEventBus>();
        _vnPayService = new Services.VnPayService(mockLogger.Object, vnPayConfig, _commissionConfig, _vnPayTransactionRepository,
            mockHttpClientWrapper.Object, mockMapper.Object, _eventBus.Object);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task QueryTransactionFromVnPay_ShouldLogMessageByResponseStatus(bool isSuccessResponseFromVnPay)
    {
        // setup
        var mockLogger = new Mock<ILogger<VnPayService>>();
        var mockEventBus = new Mock<IEventBus>();
        var mockVnpayTransRepo = new Mock<IVnPayTransactionRepository>();
        var mockMapper = new Mock<IMapper>();
        var vnPayConfig = new VnPayConfiguration()
        {
            Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
            Version = "2.1.0",
            HashSecret = "PWAKRUONSTULCMTFUDGVLELZQRCMEUFX",
            HashType = "HMACSHA512",
            QueryDr = "https://sandbox.vnpayment.vn/merchant_webapi/merchant.html",
            ReturnUrl = "http://localhost:8080/return-page",
            TmnCode = "CABVN001"
        };
        var _commissionConfig = new CommissionConfig() { High = 0.20m, Medium = .15m, Low = .1m };
        mockVnpayTransRepo.Setup(_ => _.GetByIdAsync(It.IsAny<Guid>()).Result)
            .Returns(FakeObjects.FakeVnPayTransaction());

        var mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
        var content = isSuccessResponseFromVnPay ? $"vnp_ResponseCode=00" : "vnp_ResponseCode=01";
        var mockInfo = "something";
        content += $"&vnp_OrderInfo={mockInfo}&vnp_SecureHash=somehash";
        mockHttpClientWrapper.Setup(_ => _.GetAsync(It.IsAny<string>(), null, null, null).Result)
            .Returns(new HttpResponseMessage()
            { Content = new StringContent(content) });

        // sut: partial mock VnPayService to bypass ValidateSignature
        var sut = new Mock<VnPayService>(mockLogger.Object, vnPayConfig, _commissionConfig, mockVnpayTransRepo.Object,
            mockHttpClientWrapper.Object, mockMapper.Object, mockEventBus.Object);
        sut.CallBase = true;
        sut.Setup(_ => _.ValidateSignature(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        // action
        var resultObj = await sut.Object.QueryTransactionFromVnPay(Guid.NewGuid(), new Faker().Internet.IpAddress().ToString());

        // assert
        if (isSuccessResponseFromVnPay)
        {
            mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        if (!isSuccessResponseFromVnPay)
        {
            mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        resultObj.Should().BeOfType<QueryTransactionResponseDto>();
        resultObj.vnp_OrderInfo.Should().Be(mockInfo);
    }

    [Theory]
    [InlineData(true, VnPayTransactionType.TOP_UP)]
    [InlineData(true, VnPayTransactionType.DONATE)]
    [InlineData(false)]
    public async Task IpnCheckMethodReceiveConfirmation_ShouldUpdateVnPayTransactionInDb_And_ShouldPublishEvent(
        bool isSuccessConfirmationFromVnPay, VnPayTransactionType? transactionType = null)
    {
        // set up
        var vnPayTrans = FakeObjects.FakeVnPayTransaction(transactionType: transactionType);
        await _vnPayTransactionRepository.CreateAsync(vnPayTrans);
        var updateOrderInfoDto = isSuccessConfirmationFromVnPay
            ? FakeObjects.FakeUpdateOrderInfoDto(transactionId: vnPayTrans.Id.ToString(), amount: vnPayTrans.Amount.ToString())
            : FakeObjects.FakeUpdateOrderInfoDto(transactionId: vnPayTrans.Id.ToString(), amount: vnPayTrans.Amount.ToString(), vnp_ResponseCode: "01");

        // action
        await _vnPayService.IpnCheck(updateOrderInfoDto);


        // assert
        vnPayTrans =
            await _vnPayTransactionRepository.GetByIdAsync(vnPayTrans
                .Id); // vnPayTrans obj should be updated after receiving confirmation from VnPay

        if (isSuccessConfirmationFromVnPay)
        {
            vnPayTrans.PaymentTranId.Should().Be(long.Parse(updateOrderInfoDto.vnp_TransactionNo));
            vnPayTrans.ResponseCode.Should().Be(updateOrderInfoDto.vnp_ResponseCode);
            vnPayTrans.Status.Should().Be("1");
            switch (transactionType)
            {
                case VnPayTransactionType.TOP_UP:
                    _eventBus.Verify(obj => obj.Publish(It.IsAny<PaymentTopUpIntegrationEvent>()), Times.Once);
                    vnPayTrans.PaymentCommission.Should().BeNull();
                    break;
                case VnPayTransactionType.DONATE:
                    _eventBus.Verify(obj => obj.Publish(It.IsAny<PaymentDonateIntegrationEvent>()), Times.Once);
                    vnPayTrans.PaymentCommission.Should().NotBeNull();
                    break;
                default:
                    throw new InvalidOperationException("Only TOP_UP and DONATE");
            }
        }

        if (!isSuccessConfirmationFromVnPay)
        {
            vnPayTrans.PaymentTranId.Should().Be(long.Parse(updateOrderInfoDto.vnp_TransactionNo));
            vnPayTrans.ResponseCode.Should().Be(updateOrderInfoDto.vnp_ResponseCode);
            vnPayTrans.Status.Should().Be("2");
            _eventBus.Verify(obj => obj.Publish(It.IsAny<IntegrationEvent>()), Times.Never);
        }
    }

    private static IEnumerable<object[]> GetData
    {
        get
        {
            return new List<object[]>
            {
                new object[]
                {
                    new OrderInfoDto()
                    {
                        Amount = 123456, BillInfo = new BillInfoDto()
                        {
                            Mobile = "0349039671",
                            Email = "dnhanh95@gmail.com",
                            Address = "Ha Noi",
                            FullName = "Dang Nhat Anh",
                            City = "Ha Noi",
                            Country = "Viet Nam",
                            State = "Ha Noi",
                        }
                    }
                }
            };
        }
    }
}