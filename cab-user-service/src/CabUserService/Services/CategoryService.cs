using AutoMapper;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.Models.Dtos;
using CabUserService.Services.Base;
using CabUserService.Services.Interfaces;

namespace CabUserService.Services
{
    public class CategoryService : BaseService<CategoryService>, ICategoryService
    {
         private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        public CategoryService(ILogger<CategoryService> logger, IMapper mapper, IServiceProvider serviceProvider) : base(logger)
        {
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<List<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categoryRepository = _serviceProvider.GetRequiredService<ICategoryRepository>();
            var allCategories = await categoryRepository.GetAllCategoriesAsync();
            return _mapper.Map<List<CategoryResponse>>(allCategories);
        }
    }
}
