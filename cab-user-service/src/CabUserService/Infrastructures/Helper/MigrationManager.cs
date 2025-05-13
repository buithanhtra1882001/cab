using CabUserService.Constants;
using CabUserService.Infrastructures.DbContexts;
using CabUserService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace CabUserService.Infrastructures.Helper
{
    public static class MigrationManager
    {
        public static async Task<WebApplication> SeedAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<PostgresDbContext>())
                {
                    try
                    {
                        await appContext.Database.EnsureCreatedAsync();

                        if (!await appContext.Users.AnyAsync(x => x.FullName == "Hệ thống"))
                        {
                            var system = new User
                            {
                                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                                FullName = "Hệ thống"
                            };
                            appContext.Add(system);
                            await appContext.SaveChangesAsync();
                        }

                        if (!await appContext.UserDetails.AnyAsync(x => x.UserId == new Guid("00000000-0000-0000-0000-000000000001")))
                        {
                            var systemDetail = new UserDetail
                            {
                                UserId = new Guid("00000000-0000-0000-0000-000000000001"),
                                Avatar = "https://devcab.org/static/media/logo-dark.bbd91c672835516877ab.png"
                            };
                            appContext.Add(systemDetail);
                            await appContext.SaveChangesAsync();
                        }

                        if (!await appContext.UserTransactionLogs.AnyAsync())
                            {
                                var users = await appContext.Users.ToListAsync();
                                if (users.Any())
                                {
                                    var userTransactions = new List<UserTransaction>();
                                    var random = new Random();

                                    foreach (var fromUser in users)
                                    {
                                        var toUsers = users.Where(x => x.Id != fromUser.Id).ToList();
                                        if (toUsers.Any())
                                        {
                                            var toUser = toUsers[random.Next(toUsers.Count)];
                                            var userTrasaction = new UserTransaction()
                                            {
                                                Id = Guid.NewGuid(),
                                                FromUserId = fromUser.Id,
                                                ToUserId = toUser.Id,
                                                Amount = random.Next(100),
                                                Description = "description",
                                                DonationMessage = "donationMessage",
                                                Status = TransactionStatus.SUCCESS,
                                                Type = TransactionType.TRANSFER,
                                                CreatedAt = DateTime.UtcNow,
                                                UpdatedAt = DateTime.UtcNow,
                                                IsHidingMessage = false,
                                            };

                                            userTransactions.Add(userTrasaction);
                                        }
                                    }

                                    await appContext.AddRangeAsync(userTransactions);
                                    await appContext.SaveChangesAsync();
                                }
                            }

                        if (!await appContext.Categories.AnyAsync())
                        {
                            var categoryList = new List<Category>
                            {
                                 new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Reading",
                                    Avatar = "",
                                    SortOrder =1,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Listening to music",
                                    Avatar = "",
                                    SortOrder =2,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Shopping",
                                    Avatar = "",
                                    SortOrder =3,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Traveling",
                                    Avatar = "",
                                    SortOrder =4,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Hiking",
                                    Avatar = "",
                                    SortOrder =5,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Cycling",
                                    Avatar = "",
                                    SortOrder =6,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Exercising",
                                    Avatar = "",
                                    SortOrder =7,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Cooking",
                                    Avatar = "",
                                    SortOrder =8,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Walking",
                                    Avatar = "",
                                    SortOrder =9,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Baking",
                                    Avatar = "",
                                    SortOrder =10,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Gardening",
                                    Avatar = "",
                                    SortOrder =11,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Drawing",
                                    Avatar = "",
                                    SortOrder =12,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Painting",
                                    Avatar = "",
                                    SortOrder =13,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Blogging",
                                    Avatar = "",
                                    SortOrder =14,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Dancing",
                                    Avatar = "",
                                    SortOrder =15,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Skateboarding",
                                    Avatar = "",
                                        SortOrder =16,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Singing",
                                    Avatar = "",
                                    SortOrder =17,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Playing musical instruments",
                                    Avatar = "",
                                    SortOrder =18,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Learning new languages",
                                    Avatar = "",
                                    SortOrder =19,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Collecting things",
                                    Avatar = "",
                                        SortOrder =20,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Playing computer games",
                                    Avatar = "",
                                    SortOrder =21,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Doing crafts (handmade)",
                                    Avatar = "",
                                    SortOrder =22,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Sewing",
                                    Avatar = "",
                                    SortOrder =23,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Knitting",
                                    Avatar = "",
                                        SortOrder =24,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Playing board games",
                                    Avatar = "",
                                    SortOrder =25,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Writing stories",
                                    Avatar = "",
                                        SortOrder =26,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Fishing",
                                    Avatar = "",
                                    SortOrder =27,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Photography",
                                    Avatar = "",
                                    SortOrder =28,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Skydiving",
                                    Avatar = "",
                                    SortOrder =29,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Skating",
                                    Avatar = "",
                                        SortOrder =30,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Skiing",
                                    Avatar = "",
                                    SortOrder =31,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Surfing",
                                    Avatar = "",
                                    SortOrder =32,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                },new Category()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Others",
                                    Avatar = "",
                                    SortOrder =33,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                }
                            };
                            await appContext.AddRangeAsync(categoryList);
                            await appContext.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }

            return app;
        }
    }
}
