namespace CabPaymentService.Infrastructures.Constants
{
    public class VnPayConstants
    {
        public static readonly string VERSION = "vnp_Version";
        public static readonly string COMMAND = "vnp_Command";
        public static readonly string TMN_CODE = "vnp_TmnCode";
        public static readonly string AMOUNT = "vnp_Amount";
        public static readonly string BANK_CODE = "vnp_BankCode";
        public static readonly string CREATE_DATE = "vnp_CreateDate";
        public static readonly string CURR_CODE = "vnp_CurrCode";
        public static readonly string IP_ADDRESS = "vnp_IpAddr";
        public static readonly string LOCALE = "vnp_Locale";
        public static readonly string ORDER_INFO = "vnp_OrderInfo";
        public static readonly string ORDER_TYPE = "vnp_OrderType";
        public static readonly string RETURN_URL = "vnp_ReturnUrl";
        public static readonly string TXN_REF = "vnp_TxnRef";
        public static readonly string EXPIRE_DATE = "vnp_ExpireDate";
        public static readonly string BILL_MOBILE = "vnp_Bill_Mobile";
        public static readonly string BILL_EMAIL = "vnp_Bill_Email";
        public static readonly string BILL_FIRST_NAME = "vnp_Bill_FirstName";
        public static readonly string BILL_LAST_NAME = "vnp_Bill_LastName";
        public static readonly string BILL_ADDRESS = "vnp_Bill_Address";
        public static readonly string BILL_CITY = "vnp_Bill_City";
        public static readonly string BILL_COUNTRY = "vnp_Bill_Country";
        public static readonly string BILL_STATE = "vnp_Bill_State";
        public static readonly string INV_PHONE = "vnp_Inv_Phone";
        public static readonly string INV_EMAIL = "vnp_Inv_Email";
        public static readonly string INV_CUSTOMER = "vnp_Inv_Customer";
        public static readonly string INV_ADDRESS = "vnp_Inv_Address";
        public static readonly string INV_COMPANY = "vnp_Inv_Company";
        public static readonly string INV_TAXCODE = "vnp_Inv_Taxcode";
        public static readonly string INV_TYPE = "vnp_Inv_Type";
        public static readonly string TRANSACTION_TYPE = "vnp_TransactionType";
        public static readonly string CREATE_BY = "vnp_CreateBy";
        public static readonly string TRANS_DATE = "vnp_TransDate";
        public static readonly string TRANSACTION_NO = "vnp_TransactionNo";
        public static readonly string VNP_SECURE_HASH = "vnp_SecureHash";
        public static readonly string VNP_SECURE_HASH_TYPE = "vnp_SecureHashType";
        public static readonly string HMACSHA512 = "HMACSHA512";
        public static readonly string SHA256 = "SHA256";
        public static readonly string RESPONSE_CODE = "vnp_ResponseCode";
        public static readonly string MESSAGE = "vnp_Message";
        public static readonly string PAY_DATE = "vnp_PayDate";
        public static readonly string TRANSACTION_STATUS = "vnp_TransactionStatus";

    }

    public enum VnPayTransactionType
    {
        TOP_UP,
        DONATE
    }
}
