using CabPaymentService.Infrastructures.Constants;

namespace CabPaymentService.Infrastructures.Helpers
{
    /// <summary>
    /// Class chứa các function dùng cho VnPay
    /// </summary>
    public static class VnPayHelper
    {
        public static void AddInvType(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.INV_TYPE, value);
        public static SortedList<string, string> AddInvTaxCode(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.INV_TAXCODE, value);
        public static SortedList<string, string> AddInvCompany(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.INV_COMPANY, value);
        public static SortedList<string, string> AddInvAddress(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.INV_ADDRESS, value);
        public static SortedList<string, string> AddInvCustomer(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.INV_CUSTOMER, value);
        public static SortedList<string, string> AddInvEmail(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.INV_EMAIL, value);
        public static SortedList<string, string> AddInvPhone(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.INV_PHONE, value);
        public static SortedList<string, string> AddBillState(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.BILL_STATE, value);
        public static SortedList<string, string> AddBillCountry(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.BILL_COUNTRY, value);
        public static SortedList<string, string> AddBillCity(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.BILL_CITY, value);
        public static SortedList<string, string> AddBillAddress(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.BILL_ADDRESS, value);
        public static SortedList<string, string> AddBillLastName(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.BILL_LAST_NAME, value);
        public static SortedList<string, string> AddBillFirstName(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.BILL_FIRST_NAME, value);
        public static SortedList<string, string> AddBillEmail(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.BILL_EMAIL, value);
        public static SortedList<string, string> AddBillMobile(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.BILL_MOBILE, value);
        public static SortedList<string, string> AddExpireDate(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.EXPIRE_DATE, value);
        public static SortedList<string, string> AddTxnRef(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.TXN_REF, value);
        public static SortedList<string, string> AddReturnUrl(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.RETURN_URL, value);
        public static SortedList<string, string> AddOrderType(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.ORDER_TYPE, value);
        public static SortedList<string, string> AddOrderInfo(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.ORDER_INFO, value);
        public static SortedList<string, string> AddLocale(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.LOCALE, value);
        public static SortedList<string, string> AddIpAddress(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.IP_ADDRESS, value);
        public static SortedList<string, string> AddCurrCode(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.CURR_CODE, value);
        public static SortedList<string, string> AddVersion(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.VERSION, value);
        public static SortedList<string, string> AddCommand(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.COMMAND, value);
        public static SortedList<string, string> AddTmnCode(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.TMN_CODE, value);
        public static SortedList<string, string> AddAmount(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.AMOUNT, value);
        public static SortedList<string, string> AddBankCode(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.BANK_CODE, value);
        public static SortedList<string, string> AddCreateDate(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.CREATE_DATE, value);
        public static SortedList<string, string> AddCreateBy(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.CREATE_BY, value);
        public static SortedList<string, string> AddTransactionType(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.TRANSACTION_TYPE, value);
        public static SortedList<string, string> AddTransDate(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.TRANS_DATE, value);
        public static SortedList<string, string> AddTransactionNo(this SortedList<string, string> list, string value) => list.AddRequestData(VnPayConstants.TRANSACTION_NO, value);

        /// <summary>
        /// Add key, value vào list request params
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private static SortedList<string, string> AddRequestData(this SortedList<string, string> list, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                list.Add(key, value);
            }
            return list;
        }
    }
}
