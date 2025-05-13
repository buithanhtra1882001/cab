using AutoMapper;
using CAB.BuildingBlocks.EventBus.Abstractions;
using CabPaymentService.Infrastructures.Communications;
using CabPaymentService.Infrastructures.Comparer;
using CabPaymentService.Infrastructures.Configurations;
using CabPaymentService.Infrastructures.Constants;
using CabPaymentService.Infrastructures.Extensions;
using CabPaymentService.Infrastructures.Helpers;
using CabPaymentService.Infrastructures.IntegrationEvents;
using CabPaymentService.Infrastructures.Repositories.Interfaces;
using CabPaymentService.Model.Dtos;
using CabPaymentService.Model.Entities;
using CabPaymentService.Services.Base;
using CabPaymentService.Services.Interfaces;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static CabPaymentService.Infrastructures.Constants.VnPayConstants;

namespace CabPaymentService.Services
{
    public class VnPayService : BaseService<VnPayService>, IVnPayService
    {
        private readonly VnPayConfiguration _vnPayConfiguration;
        private readonly CommissionConfig _commissionConfig;
        private readonly IVnPayTransactionRepository _vnPayTransactionRepository;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;
        private readonly string _version;
        private readonly SortedList<string, string> _requestData;
        private readonly IHttpClientWrapper _httpClientWrapper;

        public VnPayService(
            ILogger<VnPayService> logger,
            VnPayConfiguration vnPayConfiguration,
            CommissionConfig commissionConfig,
            IVnPayTransactionRepository vnPayTransactionRepository,
            IHttpClientWrapper httpClientWrapper,
            IMapper mapper,
            IEventBus eventBus) : base(logger)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            _vnPayConfiguration = vnPayConfiguration;
            _commissionConfig = commissionConfig;
            _requestData = new SortedList<string, string>(new VnPayCompare());
            _version = vnPayConfiguration.Version ?? string.Empty;
            _vnPayTransactionRepository = vnPayTransactionRepository;
            _httpClientWrapper = httpClientWrapper;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        public async Task<string> CreatePayment(OrderInfoDto order, string IpAddress)
        {
            try
            {
                if (order.TransactionType == VnPayTransactionType.TOP_UP && order.CreatedBy != order.PaymentReceiver)
                {
                    throw new InvalidOperationException(
                        "sender user id must be equal to receiver user id when transaction type is TOP_UP");
                }

                if (order.TransactionType == VnPayTransactionType.DONATE && order.CreatedBy == order.PaymentReceiver)
                {
                    throw new InvalidOperationException(
                        "sender user id must be different from receiver user id when transaction type is TOP_UP");
                }

                var entity = _mapper.Map<VnPayTransaction>(order);
                entity.BankCode = string.Empty;
                entity.ExpireDate = DateTime.UtcNow.AddHours(1).ToString("yyyyMMddHHmmss");
                entity.Status = "0";

                await _vnPayTransactionRepository.CreateAsync(entity);

                BuildPaymentRequest(_vnPayConfiguration, order, IpAddress);
                var paymentUrl = CreateRequestUrl(
                    _vnPayConfiguration.Url,
                    _vnPayConfiguration.HashSecret);

                return paymentUrl;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create payment url for VnPay with order {@order} from ipaddress {ip}",
                    order, IpAddress);

                throw;
            }
        }

        public async Task<object> Refund(RefundOrderDto order, string IpAddress)
        {
            try
            {
                var strDatax = string.Empty;

                BuildRefundRequest(order, IpAddress);
                var refundUrl = CreateRequestUrl(
                    _vnPayConfiguration.Url,
                    _vnPayConfiguration.HashSecret);

                var response = await _httpClientWrapper.GetAsync(refundUrl);
                using var stream = response.Content.ReadAsStream();

                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    strDatax = reader.ReadToEnd();
                }

                return new { strDatax };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to Refund order {@order} from ip address {@ip}", order, IpAddress);

                throw;
            }
        }

        /// <summary>
        /// Đây là method để hệ thống cab nhận kết quả thanh toán trả về từ VNPAY.
        /// Trên URL VNPAY gọi về có mang thông tin thanh toán để căn cứ vào kết quả đó Website TMĐT xử lý các bước tiếp theo (ví dụ: cập nhật kết quả thanh toán vào Database …)
        /// </summary>
        /// <param name="infoDto"></param>
        /// <returns></returns>
        public async Task<string> IpnCheck(UpdateOrderInfoDto infoDto)
        {
            var result = string.Empty;
            var isProceed = true;
            var validateSignatureResult = false;
            try
            {
                AddRequestData(infoDto);

                if (_requestData.Count == 0)
                {
                    result = "{\"RspCode\":\"99\",\"Message\":\"Input data required\"}";
                    isProceed = false;
                }

                if (isProceed)
                {
                    validateSignatureResult = ValidateSignature(
                        infoDto.vnp_SecureHash,
                        _vnPayConfiguration.HashSecret);
                }

                if (isProceed && validateSignatureResult)
                {
                    var id = Guid.Parse(infoDto.vnp_TxnRef);
                    result = await GetIpnTransactionResult(infoDto, id);
                }

                if (isProceed && !validateSignatureResult)
                {
                    result = "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}";
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, result, infoDto);

                throw;
            }
        }

        public async Task<QueryTransactionResponseDto> QueryTransactionFromVnPay(Guid transId, string ipAddress)
        {
            try
            {
                var transaction = await _vnPayTransactionRepository.GetByIdAsync(transId);
                var uri = _vnPayConfiguration.QueryDr;

                BuildTransactionQueryRequest(_vnPayConfiguration, transaction, ipAddress);

                var queryUrl = CreateRequestUrl(
                    _vnPayConfiguration.QueryDr,
                    _vnPayConfiguration.HashSecret);

                var transResult = await _httpClientWrapper.GetAsync(queryUrl);

                var result = await ParseResult(transResult);

                if (result.vnp_ResponseCode == "00")
                {
                    _logger.LogInformation("Transaction returned from VnPay: {@transaction}", transResult);
                }

                if (result.vnp_ResponseCode != "00")
                {
                    _logger.LogError(
                        "Failed to query for transaction in VnPay server for transaction Id {@id}. Initiated Ip address {ipAddress}. Transaction returned from Vnpay: {@transaction}",
                        transId, ipAddress, transResult);
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e,
                    "Exception occured while querying for transaction in VnPay server for transaction Id {@id} from ip address {ipAddress}",
                    transId, ipAddress);

                throw;
            }
        }

        /// <summary>
        /// QueryDr VnPay returns in query string format. This method parses the returned query string into QueryTransactionResponseDto
        /// </summary>
        /// <param name="transResult"></param>
        /// <returns></returns>
        private async Task<QueryTransactionResponseDto> ParseResult(HttpResponseMessage transResult)
        {
            var stream = await transResult.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            var result = new QueryTransactionResponseDto();

            content.ParseFromQueryStringToSortedList(_requestData);

            var validateSignatureResult = ValidateSignature(
                _requestData["vnp_SecureHash"],
                _vnPayConfiguration.HashSecret);

            if (!validateSignatureResult)
            {
                _logger.LogWarning(
                    "Invalid signature returned from querying transaction VnPay QueryDr. Content returned from VnPay {@content}",
                    content);

                result = new QueryTransactionResponseDto { vnp_ResponseCode = "97", vnp_Message = "Chữ ký không hợp lệ" };
            }

            if (validateSignatureResult)
            {
                result = _requestData.ToObject<QueryTransactionResponseDto>();
            }

            return result;
        }

        #region private sub/func

        private void BuildTransactionQueryRequest(VnPayConfiguration configuration, VnPayTransaction transaction,
            string IpAddress)
        {
            _requestData
                .AddVersion(_version)
                .AddCommand("querydr")
                .AddTmnCode(configuration.TmnCode ?? string.Empty)
                .AddTxnRef(transaction.Id.ToString())
                .AddOrderInfo($"Truy van ket qua thanh toan {transaction.Id}")
                .AddTransactionNo(transaction.PaymentTranId.ToString())
                .AddTransDate(transaction.TransactionCreatedDate.ToString("yyyyMMddHHmmss"))
                .AddCreateDate(DateTime.Now.ToString("yyyyMMddHHmmss"))
                .AddIpAddress(IpAddress);
        }

        private async Task<string> GetIpnTransactionResult(UpdateOrderInfoDto infoDto, Guid transactionId)
        {
            var result = string.Empty;
            try
            {
                var logMessage = string.Empty;
                var amountFromDto = !string.IsNullOrEmpty(infoDto.vnp_Amount) ? long.Parse(infoDto.vnp_Amount) : -1;
                var transaction = await _vnPayTransactionRepository.GetByIdAsync(transactionId);
                result = "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
                var isProceed = true;

                if (transaction == null)
                {
                    result = "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
                    isProceed = false;
                }

                if (isProceed && (transaction.Amount * 100) != amountFromDto)
                {
                    result = "{\"RspCode\":\"04\",\"Message\":\"invalid amount\"}";
                    isProceed = false;
                }

                if (isProceed && string.Compare(transaction.Status, "0", true) != 0)
                {
                    result = "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
                    isProceed = false;
                }

                if (isProceed)
                {
                    var status = "2";
                    logMessage = string.Format("Thanh toan loi, OrderId={0}, VNPAY TranId={1}, ResponseCode={2}",
                        transaction.Id, transaction.PaymentTranId, transaction.ResponseCode);

                    if (infoDto.vnp_ResponseCode == "00" && infoDto.vnp_TransactionStatus == "00")
                    {
                        status = "1";
                        logMessage = string.Format("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}",
                            transaction.Id, transaction.PaymentTranId);

                        if (transaction.TransactionType == VnPayTransactionType.TOP_UP)
                        {
                            var paymentTopUpEvent = new PaymentTopUpIntegrationEvent
                            {
                                ReceivingUserId = transaction.PaymentReceiver.ToString(),
                                Amount = transaction.Amount.ToString(),
                            };
                            _eventBus.Publish(paymentTopUpEvent);
                        }

                        if (transaction.TransactionType == VnPayTransactionType.DONATE)
                        {
                            var amount = transaction.Amount;
                            var commissionPercent = _commissionConfig.Low;
                            var commissionAmount = amount * commissionPercent;
                            var afterCommissionAmount = amount - commissionAmount;

                            var paymentCommission = new PaymentCommission
                            {
                                CreatedBy = transaction.CreatedBy,
                                Amount = commissionAmount,
                                CommissionPercentage = commissionPercent
                            };

                            transaction.PaymentCommission = paymentCommission;

                            var paymentDonateEvent = new PaymentDonateIntegrationEvent
                            {
                                ReceivingUserId = transaction.PaymentReceiver.ToString(),
                                Amount = afterCommissionAmount.ToString(),
                            };

                            _eventBus.Publish(paymentDonateEvent);
                        }
                    }

                    //update db
                    transaction.Status = status;
                    transaction.ResponseCode = infoDto.vnp_ResponseCode;
                    transaction.PaymentTranId = long.Parse(infoDto.vnp_TransactionNo);

                    await _vnPayTransactionRepository.UpdateAsync(transaction);
                }

                logMessage += "\nResult returned to VnPay:" + result;

                _logger.LogInformation(logMessage);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e,
                    "Failure in GetIpnTransactionResult() with UpdateOrderInfoDto {@infoDto} and Guid Id {@id}. Result string is {@result}",
                    infoDto, transactionId, result);

                throw;
            }
        }

        /// <summary>
        /// Add key, value vào list request params
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        /// <summary>
        /// Add list key,value vào list response data
        /// </summary>
        /// <param name="data"></param>
        private void AddRequestData(SortedList<string, string> data)
        {
            foreach (var kv in data)
            {
                AddRequestData(kv.Key, kv.Value);
            }
        }

        /// <summary>
        /// Add list key,value vào list response data
        /// </summary>
        /// <param name="data"></param>
        private void AddRequestData(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var responseData = JsonConvert.DeserializeObject<SortedList<string, string>>(json);
            AddRequestData(responseData);
        }

        /// <summary>
        /// Tạo request api
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="vnp_HashSecret"></param>
        /// <returns></returns>
        private string CreateRequestUrl(string? baseUrl, string? vnp_HashSecret)
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string queryString = data.ToString();

            baseUrl += "?" + queryString;
            string signData = queryString;
            if (signData.Length > 0)
            {
                signData = signData.Remove(data.Length - 1, 1);
            }

            string? vnp_SecureHash = string.Empty;

            if (_vnPayConfiguration.HashType.Equals(HMACSHA512, StringComparison.CurrentCultureIgnoreCase))
            {
                vnp_SecureHash = EncryptionHelper.HmacSHA512(vnp_HashSecret ?? string.Empty, signData);
            }

            if (_vnPayConfiguration.HashType.Equals(SHA256, StringComparison.CurrentCultureIgnoreCase))
            {
                vnp_SecureHash = EncryptionHelper.Sha256(vnp_HashSecret ?? string.Empty, signData);
            }

            baseUrl += VNP_SECURE_HASH + "=" + vnp_SecureHash;

            return baseUrl;
        }

        /// <summary>
        /// build param cho payment request
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="order"></param>
        /// <param name="IpAddress"></param>
        private void BuildPaymentRequest(VnPayConfiguration configuration, OrderInfoDto order, string IpAddress)
        {
            var now = DateTime.Now;

            _requestData
                .AddVersion(_version)
                .AddCommand("pay")
                .AddTmnCode(configuration.TmnCode ?? string.Empty)
                .AddAmount((order.Amount * 100).ToString())
                //.AddBankCode(order.BankCode)
                .AddCreateDate(now.ToString("yyyyMMddHHmmss"))
                .AddCurrCode("VND")
                .AddIpAddress(IpAddress)
                .AddLocale(order.Locale)
                .AddOrderInfo("Thanh toan don hang " + order.OrderId.ToString())
                .AddOrderType(order.Category)
                .AddReturnUrl(configuration.ReturnUrl ?? string.Empty)
                .AddTxnRef(order.OrderId.ToString())
                .AddExpireDate(now.AddHours(1).ToString("yyyyMMddHHmmss"));
        }

        private void BuildRefundRequest(RefundOrderDto order, string IpAddress)
        {
            var amounTrf = Convert.ToInt32(order.Amount) * 100;
            _requestData
                .AddVersion(_version)
                .AddCommand("refund")
                .AddTmnCode(_vnPayConfiguration.TmnCode ?? string.Empty)
                .AddTransactionType(order.RefundCategory ?? string.Empty)
                .AddCreateBy(order.Email ?? string.Empty)
                .AddTxnRef(order.OrderId ?? string.Empty)
                .AddAmount(amounTrf.ToString())
                .AddOrderInfo("REFUND ORDERID " + order.OrderId)
                .AddTransDate(order.PayDate ?? string.Empty)
                .AddCreateDate(DateTime.Now.ToString("yyyyMMddHHmmss"))
                .AddIpAddress(IpAddress);
        }

        /// <summary>
        /// Kiểm tra chữ kí
        /// </summary>
        /// <param name="inputHash"></param>
        /// <param name="secretKey"></param>
        /// <param name="data">leave null to validate _requestData</param>
        /// <returns></returns>
        public virtual bool ValidateSignature(string? inputHash, string? secretKey)
        {
            var result = false;
            if (!string.IsNullOrEmpty(secretKey))
            {
                string rspRaw = EncodeData();
                string myChecksum = EncryptionHelper.HmacSHA512(secretKey, rspRaw);
                result = myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
            }

            return result;
        }

        private string EncodeData()
        {
            var sb = new StringBuilder();
            foreach (var kv in _requestData)
            {
                if (!kv.Value.IsNullOrEmpty() &&
                    !kv.Key.Contains("vnp_SecureHashType") &&
                    !kv.Key.Contains("vnp_SecureHash"))
                {
                    sb.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            if (sb.Length > 0)
            {
                sb = sb.Remove(sb.Length - 1, 1);
            }

            var rawData = sb.ToString();
            return rawData;
        }

        #endregion
    }
}