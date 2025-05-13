using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Bogus;
using CabPaymentService.Infrastructures.Constants;
using CabPaymentService.Infrastructures.Extensions;
using CabPaymentService.Infrastructures.Helpers;
using CabPaymentService.Model.Dtos;
using CabPaymentService.Model.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CabPaymentService.Tests;

public class FakeObjects
{
    public static IConfiguration GetConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"VnPay:url", "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"},
            {"VnPay:querydr", "https://sandbox.vnpayment.vn/merchant_webapi/merchant.html"},
            {"VnPay:tmnCode", "CABVN001"},
            {"VnPay:hashSecret", "PWAKRUONSTULCMTFUDGVLELZQRCMEUFX"},
            {"VnPay:returnUrl", "http://localhost:8080/return-page"},
            {"VnPay:hashType", "HMACSHA512"},
            {"VnPay:version", "2.1.0"},
            {"PaymentDbConnectionString", "Host=localhost;Database=UserDb;Username=postgres;Password=mysecretpassword"},
            //...populate as needed for the test
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        return configuration;
    }
    public static BillInfo FakeBillInfo()
    {
        var fakeObj = new Faker<BillInfo>()
            .Rules(((faker, info) =>
            {
                info.Id = Guid.NewGuid();
                info.Address = faker.Address.StreetAddress();
                info.City = faker.Address.City();
                info.Country = faker.Address.Country();
                info.State = faker.Address.State();
                info.Email = faker.Internet.Email();
                info.Mobile = faker.Phone.PhoneNumber();
                info.Transaction = null; // setting transaction could create infinite loop. As FakeVnPayTransaction() also create BillInfo
                info.FullName = faker.Person.FullName;
            }))
            .Generate();

        return fakeObj;
    }
    
    public static Invoice FakeInvoice()
    {
        var fakeObj = new Faker<Invoice>()
            .Rules((faker, invoice) =>
            {
                invoice.Id = Guid.NewGuid();
                invoice.Phone = faker.Phone.PhoneNumber();
                invoice.Address = faker.Address.FullAddress();
                invoice.Company = faker.Company.CompanyName();
                invoice.Customer = faker.Person.FullName;
                invoice.Email = faker.Internet.Email();
                invoice.Transaction = null; // setting transaction could create infinite loop. As FakeVnPayTransaction() also create Invoice
                invoice.Type = faker.Random.String(length: 3);
                invoice.Taxcode = faker.Random.String(length: 2);

            })
            .Generate();

        return fakeObj;
    }
    public static VnPayTransaction FakeVnPayTransaction(VnPayTransactionType? transactionType = null)
    {
        var billInfo = FakeBillInfo();
        var invoice = FakeInvoice();
        var fakeObj = new Faker<VnPayTransaction>().Rules(((faker, transaction) =>
        {
            transaction.Id = Guid.NewGuid();
            transaction.Amount = faker.Random.Long(min: 0, 999_999_999_999);
            transaction.OrderDesc = faker.Lorem.Sentence();
            transaction.TransactionCreatedDate = faker.Date.Past();
            transaction.TransactionType =  transactionType ?? faker.PickRandomParam(VnPayTransactionType.TOP_UP, VnPayTransactionType.DONATE);
            transaction.BankCode = faker.Finance.Bic();
            transaction.PayStatus = faker.Lorem.Word();
            transaction.BillId = billInfo.Id;
            transaction.BillInfo = billInfo;
            transaction.InvoiceId = invoice.Id;
            transaction.Invoice = invoice;
            transaction.Status = "0";
        })).Generate();

        billInfo.Transaction = fakeObj;
        invoice.Transaction = fakeObj;

        return fakeObj;
    }

    public static UpdateOrderInfoDto FakeUpdateOrderInfoDto(string transactionId = null, string amount = null, string vnp_ResponseCode = null)
    {
        var configuration = GetConfiguration();
        var fakeObj = new Faker<UpdateOrderInfoDto>().Rules((f, o) =>
        {
            o.vnp_TmnCode = configuration.GetValue<string>("VnPay:tmnCode");
            o.vnp_Amount = amount ?? f.Random.Int(min: 10_000, max: 999_999_999).ToString();
            o.vnp_BankCode = f.Finance.Bic();
            o.vnp_BankTranNo = Guid.NewGuid().ToString();
            o.vnp_CardType = f.Random.ArrayElement(new[] {"ATM", "QRCODE"});
            o.vnp_PayDate = f.Date.Past().ToString("yyyyMMddHHmmss");
            o.vnp_OrderInfo = f.Lorem.Sentence();
            o.vnp_TransactionNo = f.Random.Long(min: 10000000).ToString();
            o.vnp_ResponseCode = vnp_ResponseCode ?? "00";
            o.vnp_TxnRef = transactionId ?? Guid.NewGuid().ToString();
            o.vnp_TransactionStatus = "00";
        }).Generate();

        // generate secure hash from properties
        var json = JsonConvert.SerializeObject(fakeObj);
        var requestData = JsonConvert.DeserializeObject<SortedList<string, string>>(json);
        var data = new StringBuilder();
        foreach (var entry in requestData)
        {
            if (entry.Value.IsNullOrEmpty()) continue;

            data.Append(WebUtility.UrlEncode(entry.Key) + "=" + WebUtility.UrlEncode(entry.Value) + "&");
        }

        data = data.Remove(data.Length - 1, 1);
        var encrypted =
            EncryptionHelper.HmacSHA512(configuration.GetValue<string>("VnPay:hashSecret"), data.ToString());
        fakeObj.vnp_SecureHash = encrypted;

        return fakeObj;
    }
}