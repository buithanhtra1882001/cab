using CabUserService.Infrastructures.Repositories;
using CabUserService.Models.Entities;
using CabUserService.Tests.TestHelper;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CabUserService.Tests.Infrastructures.Repositories;
public class TestUserDetailRepository
{
    private readonly UserDetailRepository _sut;
    private readonly UserRepository _userRepository;

    public TestUserDetailRepository()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        _sut = new UserDetailRepository(MockObjects.IConfiguration());
        _userRepository = new UserRepository(MockObjects.IConfiguration());
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUserDetail()
    {
        var userDetailId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        try
        {
            var user = new User { Id = userId };
            await _userRepository.CreateAsync(user);
            var userDetail = new UserDetail { UserDetailId = userDetailId, UserId = userId };
            await _sut.CreateAsync(userDetail);
            var savedObj = await _sut.GetByIdAsync(userDetailId);
            savedObj.Should().NotBeNull();
        }
        finally
        {
            await _sut.HardDeleteAsync(userDetailId);
            await _userRepository.HardDeleteAsync(userId);
        }
    }

}
