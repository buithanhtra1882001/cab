using Bogus;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Helpers
{
    public class DataSeeder
    {
        private static decimal randomScore()
        {
            Random random = new Random();
            double randomDouble = random.NextDouble();
            decimal randomScore = (decimal)(randomDouble * (500 - 100) + 100);
            return randomScore;
        }
        public static void SeedPostsAndCategories<TContext>(
            TContext context,
            IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var scopeProvider = scope.ServiceProvider;
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger logger = loggerFactory.CreateLogger("SeedPostsAndCategories");

            try
            {
                var postCategoryRepo = scopeProvider.GetRequiredService<IPostCategoryRepository>();
                var postRepo = scopeProvider.GetRequiredService<IPostRepository>();

                var categoryCountTask = postCategoryRepo.CountPostCategory();
                categoryCountTask.Wait();
                var categoryCount = categoryCountTask.Result;

                var postCountTask = postRepo.CountPosts();
                postCountTask.Wait();
                var postCount = postCountTask.Result;

                var fakeCategory = new Faker<PostCategory>()
                    .Rules((fake, obj) =>
                    {
                        obj.Id = Guid.NewGuid();
                        obj.Slug = fake.Lorem.Slug();
                        obj.Name = fake.Lorem.Word();
                        obj.Score = randomScore();
                        obj.Description = fake.Lorem.Sentence();
                        obj.Thumbnail = fake.Image.LoremFlickrUrl();
                        obj.Status = fake.Random.Int(0, 5);
                        obj.UpdatedAt = fake.Date.Recent(days: 50);
                        obj.CreatedAt = fake.Date.Recent(days: 500);
                        obj.IsSoftDeleted = false;
                    });

                var fakePost = new Faker<Post>()
                    .RuleForType(typeof(string), f => f.Lorem.Word())
                    .RuleForType(typeof(Guid), f => Guid.NewGuid())
                    .RuleForType(typeof(Guid[]), f => Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToArray())
                    .RuleForType(typeof(int), f => f.Random.Int(1, 10))
                    .RuleForType(typeof(bool), f => f.Random.Bool())
                    .Rules((f, o) =>
                    {
                        o.Id = Guid.NewGuid().ToString();
                        o.Hashtags = string.Join(" ", Enumerable.Range(1, 5).Select(_ => f.Lorem.Word()).ToArray());
                        o.IsSoftDeleted = false;
                        o.PosterScore = randomScore();
                        //o.ImageIds = Enumerable.Range(1, 5).Select(_ => f.Image.LoremFlickrUrl()).ToArray();
                        //o.VideoIds = Enumerable.Range(1, 5).Select(_ => f.Internet.Url()).ToArray();
                    });

                var categoryList = Enumerable.Range(1, 5).Select(_ =>
                {
                    var cat = fakeCategory.Generate();
                    return cat;
                }).ToList();

                var postList = Enumerable.Range(1, 60)
                    .Select(_ => fakePost.Generate())
                    .ToList();

                postList.ForEach(post =>
                {
                    var random = new Random();
                    int randomIndex = random.Next(categoryList.Count);
                    post.CategoryId = categoryList[randomIndex].Id;
                });

                if (categoryCount < 6)
                {
                    if (categoryCount == 0)
                    {
                        postCategoryRepo.InsertCategories(categoryList);
                        logger.LogInformation("Successfully seeded PostCategory records");
                    }
                    else
                    {
                        postCategoryRepo.SeedDataCategoryAndCategoryType();
                        logger.LogInformation("Successfully seeded PostCategory and PostCategoryType records");
                    }
                }

                if (postCount == 0)
                {
                    postRepo.InsertPosts(postList);
                    logger.LogInformation("Successfully seeded Post records");
                }
            }
            catch (Exception e)
            {
                logger.LogError("Failed to seed Posts and PostCategories. Exception {e}", e.Message);
            }
        }
    }
}
