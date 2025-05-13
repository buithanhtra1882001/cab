namespace CabPostService.Constants
{
    public static class PostConstant
    {
        public const int ACTIVE = 1;
        public const int INACTIVE = 0;

        public const string OUR_EPOCH = "2011/01/01 00:00";

        public const int VOTE_UP = 1;
        public const int VOTE_DOWN = -1;

        public const int POST_CATEGORY_TYPE_IMAGE = 1;
        public const int POST_CATEGORY_TYPE_VIDEO = 2;
    }

    public class JsonSerializerNames
    {
        #region Properties

        public const string SnakeCase = "SnakeCase";
        public const string CamelCase = "CamelCase";

        #endregion
    }
}
