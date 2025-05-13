using CabPaymentService.Model.Dtos;
using static CabPaymentService.Infrastructures.Constants.VnPayConstants;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Xunit;
using CabPaymentService.Infrastructures.Extensions;
using CabPaymentService.Model.Entities;
using Newtonsoft.Json;

namespace CabPaymentService.Tests
{
    public class TestObjectExtensionMethod
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void CanParseIDictionaryToObject(IDictionary<string, string> source, Type type)
        {
            var toObject = typeof(ObjectExtensions).GetMethod("ToObject");

            var toObjectGeneric = toObject.MakeGenericMethod(new[] { type });

            var result = toObjectGeneric.Invoke(null, new object[] { source });
            
            Assert.IsType(type, result);

        }

        public static IEnumerable<object[]> Data =>

                 new List<object[]>
                    {
                        new object[] { new SortedList<string,string>()
                        {
                            { TMN_CODE, "querydr" },
                            { TXN_REF, "querydr" },
                            { AMOUNT, "10000" },
                            { ORDER_INFO, "querydr" },
                            { RESPONSE_CODE, "00" },
                            { MESSAGE, "00" },
                            { BANK_CODE, "00" },
                            { PAY_DATE, "20221031120152" }, //yyyyMMddHHmmss
                            { TRANSACTION_NO, "querydr" },
                            { TRANSACTION_TYPE, "01010101" },
                            { TRANSACTION_STATUS, "Success" },
                            { VNP_SECURE_HASH, "somehash" },
                            { VNP_SECURE_HASH_TYPE, "HMAC256" },
                        }
                        , typeof(QueryTransactionResponseDto)
                        },

                        new object[] { new SortedList<string,string>()
                        {
                            { "OrderId", Guid.NewGuid().ToString() },
                            { "Amount", "1234567" },
                            { "OrderDesc", "somedescription" },
                            { "CreatedDate", "20221031120152" },
                            { "Status", "00" },
                            { "PaymentTranId", "12345" },
                            { "BankCode", "00" },
                            { "PayStatus", "20221031120152" }, //yyyyMMddHHmmss
                            { "Locale", "querydr" },
                            { "Category", "01010101" },
                            { "ExpireDate", "Success" },
                            { "BillInfo", JsonConvert.SerializeObject(new BillInfo() {Address = "", City = "city", Country = "sdf", Id = Guid.NewGuid()}) },
                            { "Invoice", "Email=SomeEmail&Taxcode=SomeTaxCode" },
                        }
                        , typeof(OrderInfoDto)
                        },
                    };
       
    }
}