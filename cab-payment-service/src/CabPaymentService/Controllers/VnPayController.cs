using CabPaymentService.Controllers.Base;
using CabPaymentService.Infrastructures.Helpers;
using CabPaymentService.Model.Dtos;
using CabPaymentService.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CabPaymentService.Controllers
{
    [Route("api/v1/vnpay")]
    [ApiController]
    [EnableCors("CabCors")]
    public class VnPayController : ApiController<VnPayController>
    {
        private readonly IVnPayService _vnPayService;
        private ServiceResult _result;
        public VnPayController(ILogger<VnPayController> logger, IVnPayService vnPayService) : base(logger)
        {
            _vnPayService = vnPayService;
            _result = new();
        }
        /// <summary>
        /// hello world
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create-payment")]
        public async Task<IActionResult> CreateVnPayPayment(OrderInfoDto orderInfo)
        {
            try
            {
                _result.Data = await _vnPayService.CreatePayment(orderInfo, Request.GetIpAddress());
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            return Ok(_result);
        }

        [HttpPost]
        [Route("get-transaction-detail")]
        public async Task<IActionResult> GetTransactionDetail(Guid Id)
        {
            try
            {
                _result.Data = await _vnPayService.QueryTransactionFromVnPay(Id, Request.GetIpAddress());
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            //TODO: frontend handles validation error if vnp_ResponseCode != 0 (error message in vnp_Message)

            return Ok(_result);
        }

        [HttpGet]
        [Route("vnp-ipn")]
        public async Task<IActionResult> IpnCheck([FromQuery] UpdateOrderInfoDto orderInfo)
        {
            try
            {
                _result.Data = await _vnPayService.IpnCheck(orderInfo);
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            
            return Ok(_result);
        }
    }
}
