using CabPostService.Models.Entities;
using CabPostService.Models.Entities.Base;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace CabPostService.Infrastructures.DbContexts
{
    public class CassandraDbContext
    {
        public readonly Cassandra.ISession _session;

        public CassandraDbContext(IConfiguration configuration, ILogger<CassandraDbContext> logger)
        {
            var host = configuration.GetValue<string>("Cassandra:Host");
            var port = configuration.GetValue<int>("Cassandra:Port");
            var username = configuration.GetValue<string>("Cassandra:Username");
            var password = configuration.GetValue<string>("Cassandra:Password");
            var keyspace = configuration.GetValue<string>("Cassandra:Keyspace");

            var cluster = Cluster.Builder()
                     //.AddContactPoints(new IPEndPoint(IPAddress.Parse(host), port))
                     .AddContactPoint(host)
                     .WithPort(port)
                     .WithCredentials(username, password)
                     .Build();
            _session = cluster.Connect(keyspace);

            MappingConfiguration.Global.Define<PostCommentMappings>();
            MappingConfiguration.Global.Define<PostCommentReplyMappings>();
            MappingConfiguration.Global.Define<PostReportMapping>();
            MappingConfiguration.Global.Define<PostDonateMapping>();
            MappingConfiguration.Global.Define<PostCommentPostIdMaterializedViewMappings>();
            MappingConfiguration.Global.Define<PostCommentReplyCommentIdMaterializedViewMappings>();
            MappingConfiguration.Global.Define<SequenceGeneratorMapping>();
            MappingConfiguration.Global.Define<UserMapping>();
            MappingConfiguration.Global.Define<PostImageMappings>();
        }

        public Table<T> GetTable<T>()
        {
            return new Table<T>(_session);
        }
    }

    internal static class GenericMappings
    {
        public static Map<T> MapFromBaseEntity<T>(this Map<T> mapper) where T : BaseEntity
        {
            return mapper.Column(p => p.CreatedAt, cm => cm.WithName("created_at"))
               .Column(p => p.UpdatedAt, cm => cm.WithName("updated_at"));
        }
    }

    internal class PostImageMappings : Mappings
    {
        public PostImageMappings()
        {
            For<PostImage>()
                .TableName("post_posts_images")
                .PartitionKey(x => x.Id)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.PostId, cm => cm.WithName("post_id"))
                .Column(p => p.ImageId, cm => cm.WithName("image_id"))
                .Column(p => p.Url, cm => cm.WithName("image_url"))
                .Column(p => p.IsViolence, cm => cm.WithName("is_violence"))
                .MapFromBaseEntity();
        }
    }

    internal class PostCommentMappings : Mappings
    {
        public PostCommentMappings()
        {
            For<PostComment>()
                .TableName("post_comments")
                .PartitionKey(x => x.Id)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.PostId, cm => cm.WithName("post_id"))
                .Column(p => p.UserId, cm => cm.WithName("user_id"))
                .Column(p => p.Content, cm => cm.WithName("content"))
                .Column(p => p.UpvotesCount, cm => cm.WithName("upvotes_count"))
                .Column(p => p.DownvotesCount, cm => cm.WithName("downvotes_count"))
                .Column(p => p.Status, cm => cm.WithName("status"))
                .MapFromBaseEntity();
        }
    }

    internal class PostCommentPostIdMaterializedViewMappings : Mappings
    {
        public PostCommentPostIdMaterializedViewMappings()
        {
            For<PostCommentPostIdMaterializedView>()
                .TableName("post_comments_postid")
                .PartitionKey(x => x.PostId)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.PostId, cm => cm.WithName("post_id"))
                .Column(p => p.UserId, cm => cm.WithName("user_id"))
                .Column(p => p.Content, cm => cm.WithName("content"))
                .Column(p => p.UpvotesCount, cm => cm.WithName("upvotes_count"))
                .Column(p => p.DownvotesCount, cm => cm.WithName("downvotes_count"))
                .Column(p => p.Status, cm => cm.WithName("status"))
                .MapFromBaseEntity();
        }
    }

    internal class PostCommentReplyMappings : Mappings
    {
        public PostCommentReplyMappings()
        {
            For<PostCommentReply>()
                .TableName("post_comment_replies")
                .PartitionKey(x => x.CommentId)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.CommentId, cm => cm.WithName("comment_id"))
                .Column(p => p.ParentReplyId, cm => cm.WithName("parent_reply_id"))
                .Column(p => p.ReplyLevel, cm => cm.WithName("reply_level"))
                .Column(p => p.UserId, cm => cm.WithName("user_id"))
                .Column(p => p.Content, cm => cm.WithName("content"))
                .Column(p => p.UpvotesCount, cm => cm.WithName("upvotes_count"))
                .Column(p => p.DownvotesCount, cm => cm.WithName("downvotes_count"))
                .Column(p => p.Status, cm => cm.WithName("status"))
                .MapFromBaseEntity();
        }
    }

    internal class PostCommentReplyCommentIdMaterializedViewMappings : Mappings
    {
        public PostCommentReplyCommentIdMaterializedViewMappings()
        {
            For<PostCommentReplyCommentIdMaterializedView>()
                .TableName("post_comment_replies_commentid")
                .PartitionKey(x => x.CommentId)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.CommentId, cm => cm.WithName("comment_id"))
                .Column(p => p.ParentReplyId, cm => cm.WithName("parent_reply_id"))
                .Column(p => p.ReplyLevel, cm => cm.WithName("reply_level"))
                .Column(p => p.UserId, cm => cm.WithName("user_id"))
                .Column(p => p.Content, cm => cm.WithName("content"))
                .Column(p => p.UpvotesCount, cm => cm.WithName("upvotes_count"))
                .Column(p => p.DownvotesCount, cm => cm.WithName("downvotes_count"))
                .Column(p => p.Status, cm => cm.WithName("status"))
                .MapFromBaseEntity();
        }
    }

    internal class ImageCommentMappings : Mappings
    {
        public ImageCommentMappings()
        {
            For<ImageComment>()
                .TableName("image_comments")
                .PartitionKey(x => x.Id)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.ImageId, cm => cm.WithName("image_id"))
                .Column(p => p.UserId, cm => cm.WithName("user_id"))
                .Column(p => p.Content, cm => cm.WithName("content"))
                .Column(p => p.UpvotesCount, cm => cm.WithName("upvotes_count"))
                .Column(p => p.DownvotesCount, cm => cm.WithName("downvotes_count"))
                .Column(p => p.Status, cm => cm.WithName("status"))
                .MapFromBaseEntity();
        }
    }

    internal class PostReportMapping : Mappings
    {
        public PostReportMapping()
        {
            For<PostReport>()
                .TableName("post_reports")
                .PartitionKey(x => x.Id)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.UserId, cm => cm.WithName("user_id"))
                .Column(p => p.PostId, cm => cm.WithName("post_id"))
                .Column(p => p.Reason, cm => cm.WithName("reason"))
                .Column(p => p.Description, cm => cm.WithName("description"))
                .MapFromBaseEntity();
        }
    }

    internal class PostDonateMapping : Mappings
    {
        public PostDonateMapping()
        {
            For<PostDonate>()
                .TableName("post_donates")
                .PartitionKey(x => x.Id)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.PostId, cm => cm.WithName("post_id"))
                .Column(p => p.DonaterId, cm => cm.WithName("donater_id"))
                .Column(p => p.ReceiverId, cm => cm.WithName("receiver_id"))
                .Column(p => p.Title, cm => cm.WithName("title"))
                .Column(p => p.Content, cm => cm.WithName("content"))
                .Column(p => p.Value, cm => cm.WithName("value"))
                .MapFromBaseEntity();
        }
    }
    internal class SequenceGeneratorMapping : Mappings
    {
        public SequenceGeneratorMapping()
        {
            For<SequenceGenerator>()
                .TableName("sequence_generator")
                .PartitionKey(x => x.TableName)
                .Column(p => p.TableName, cm => cm.WithName("table_name"))
                .Column(p => p.SequenceNumber, cm => cm.WithName("sequence_number"));
        }
    }
    internal class UserMapping : Mappings
    {
        public UserMapping()
        {
            For<User>()
                .TableName("users")
                .PartitionKey(x => x.Id)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.Fullname, cm => cm.WithName("fullname"))
                .Column(p => p.Username, cm => cm.WithName("username"))
                .Column(p => p.Avatar, cm => cm.WithName("avatar"))
                .MapFromBaseEntity();
        }
    }

    internal class CommentLikesMappings : Mappings
    {
        public CommentLikesMappings()
        {
            For<CommentLike>()
                .TableName("comment_likes")
                .PartitionKey(x => x.Id)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.UserId, cm => cm.WithName("user_Id"))
                .Column(p => p.CommentId, cm => cm.WithName("comment_id"))
                .Column(p => p.LikeType, cm => cm.WithName("type").WithDbType<int>())
                .MapFromBaseEntity();
        }
    }
}