using CabUserService.Infrastructures.DbContexts;
using CabUserService.Infrastructures.Repositories.Base;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.Models.Entities;

namespace CabUserService.Infrastructures.Repositories
{
    public class ChatMessageRepository : BaseRepository<ChatMessage>, IChatMessageRepository
    {
        public ChatMessageRepository(ScyllaDbContext context)
            : base(context)
        {
        }
    }
}
