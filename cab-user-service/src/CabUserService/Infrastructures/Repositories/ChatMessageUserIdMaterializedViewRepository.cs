using CabUserService.Infrastructures.DbContexts;
using CabUserService.Infrastructures.Repositories.Base;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.Models.Entities;
using Cassandra;

namespace CabUserService.Infrastructures.Repositories
{
    public class ChatMessageUserIdMaterializedViewRepository :
         BaseRepository<ChatMessageUserIdMaterializedView>,
        IChatMessageUserIdMaterializedViewRepository
    {
        private readonly Cassandra.ISession _session;
        public ChatMessageUserIdMaterializedViewRepository(ScyllaDbContext context)
            : base(context)
        {
            _session = context._session;
        }

        public async Task<List<ChatMessageUserIdMaterializedView>> GetListByUserIdAsync(Guid userId)
        {
            var preparedStatementSender = await _session.PrepareAsync("SELECT senderuser_id, recipientuser_id FROM chat_messages_userId WHERE senderuser_id = ?");
            var statementSender = preparedStatementSender.Bind(userId);
            var resultSetSender = await _session.ExecuteAsync(statementSender);

            var preparedStatementRecipient = await _session.PrepareAsync("SELECT senderuser_id, recipientuser_id FROM chat_messages_userId WHERE recipientuser_id = ?");
            var statementRecipient = preparedStatementRecipient.Bind(userId);
            var resultSetRecipient = await _session.ExecuteAsync(statementRecipient);

            var userMessages = new List<ChatMessageUserIdMaterializedView>();

            foreach (var row in resultSetSender)
            {
                userMessages.Add(new ChatMessageUserIdMaterializedView
                {
                    SenderUserId = row.GetValue<Guid>("senderuser_id"),
                    RecipientUserId = row.GetValue<Guid>("recipientuser_id")
                });
            }
            foreach (var row in resultSetRecipient)
            {
                userMessages.Add(new ChatMessageUserIdMaterializedView
                {
                    SenderUserId = row.GetValue<Guid>("senderuser_id"),
                    RecipientUserId = row.GetValue<Guid>("recipientuser_id")
                });
            }

            var groupedMessages = userMessages.GroupBy(m => new { m.SenderUserId, m.RecipientUserId }).Select(g => g.First()).ToList();
            return groupedMessages;
        }
    }
}