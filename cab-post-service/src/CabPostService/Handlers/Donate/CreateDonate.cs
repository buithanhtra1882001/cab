using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using CabPostService.Models.Entities;

namespace CabPostService.Handlers.Donate
{
    public partial class DonateHandler :
        ICommandHandler<DonateCommand, Guid>
    {
        public async Task<Guid> Handle(
            DonateCommand request,
            CancellationToken cancellationToken)
        {
            var donate = _mapper.Map<PostDonate>(request);
            donate.Id = Guid.NewGuid();

            var postDonateRepository = _seviceProvider.GetRequiredService<IPostDonateRepository>();
            await postDonateRepository.CreateAsync(donate);

            return donate.Id;
        }
    }
}