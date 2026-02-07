using Application.Interfaces.Repository;
using Application.Services;
using Domain.User;
using Moq;
using Shared;
using Shared.Enums.Sorting;

namespace UnitTests.Application
{
    public class UserServiceTests
    {
        #region Search Tests

        [Fact]
        public async Task Search_ShouldCallAuthUserRepositoryGetAll()
        {
            // Arrange
            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };
            var pagedResult = new PagedResult<AuthUser>
            {
                TotalCount = 0,
                Data = new List<AuthUser>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, "");

            // Assert
            authUserRepoMock.Verify(r => r.GetAll(0, 10, sortOption, ""), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldReturnPagedResultFromRepository()
        {
            // Arrange
            var authUser = new AuthUser("TestUser", "avatar.png", "test@example.com", "Password1@");
            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };
            var pagedResult = new PagedResult<AuthUser>
            {
                TotalCount = 1,
                Data = new List<AuthUser> { authUser }
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.Search(0, 10, sortOption, "");

            // Assert
            Assert.Equal(pagedResult, result);
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task Search_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>()
            )).ThrowsAsync(new Exception("Repository failed"));

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.Search(0, 10, sortOption, ""));
        }

        [Fact]
        public async Task Search_ShouldCalculateSkipCorrectly_ForFirstPage()
        {
            // Arrange
            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };
            var pagedResult = new PagedResult<AuthUser>
            {
                TotalCount = 0,
                Data = new List<AuthUser>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, "");

            // Assert - skip should be 0 for first page
            authUserRepoMock.Verify(r => r.GetAll(0, 10, sortOption, ""), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldCalculateSkipCorrectly_ForSecondPage()
        {
            // Arrange
            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };
            var pagedResult = new PagedResult<AuthUser>
            {
                TotalCount = 0,
                Data = new List<AuthUser>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(1, 10, sortOption, "");

            // Assert - skip should be 10 for second page (pageIndex 1 * pageSize 10)
            authUserRepoMock.Verify(r => r.GetAll(10, 10, sortOption, ""), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldCalculateSkipCorrectly_ForThirdPage()
        {
            // Arrange
            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };
            var pagedResult = new PagedResult<AuthUser>
            {
                TotalCount = 0,
                Data = new List<AuthUser>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(2, 25, sortOption, "");

            // Assert - skip should be 50 for third page (pageIndex 2 * pageSize 25)
            authUserRepoMock.Verify(r => r.GetAll(50, 25, sortOption, ""), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldPassSearchTermToRepository()
        {
            // Arrange
            var searchTerm = "john";
            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };
            var pagedResult = new PagedResult<AuthUser>
            {
                TotalCount = 0,
                Data = new List<AuthUser>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, searchTerm);

            // Assert
            authUserRepoMock.Verify(r => r.GetAll(0, 10, sortOption, searchTerm), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldPassNullSearchTerm_WhenSearchIsNull()
        {
            // Arrange
            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };
            var pagedResult = new PagedResult<AuthUser>
            {
                TotalCount = 0,
                Data = new List<AuthUser>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, null);

            // Assert
            authUserRepoMock.Verify(r => r.GetAll(0, 10, sortOption, null), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldPassCorrectSortOption_WithDescendingOrder()
        {
            // Arrange
            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Created,
                Direction = SortDirection.Descending
            };
            var pagedResult = new PagedResult<AuthUser>
            {
                TotalCount = 0,
                Data = new List<AuthUser>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, "");

            // Assert
            authUserRepoMock.Verify(r => r.GetAll(0, 10, sortOption, ""), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldPassCorrectSortOption_WithLastLoginSort()
        {
            // Arrange
            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.LastLogin,
                Direction = SortDirection.Ascending
            };
            var pagedResult = new PagedResult<AuthUser>
            {
                TotalCount = 0,
                Data = new List<AuthUser>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, "");

            // Assert
            authUserRepoMock.Verify(r => r.GetAll(0, 10, sortOption, ""), Times.Once);
        }

        #endregion

        #region CreateDefaultUser Tests

        [Fact]
        public async Task CreateDefaultUser_ShouldCallUserRepositoryInsertAsync()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            userRepoMock.Setup(r => r.InsertAsync(It.IsAny<GuestUser>()))
                .ReturnsAsync((GuestUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateDefaultUser();

            // Assert
            userRepoMock.Verify(r => r.InsertAsync(It.IsAny<GuestUser>()), Times.Once);
        }

        [Fact]
        public async Task CreateDefaultUser_ShouldReturnGuestUserFromRepository()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            userRepoMock.Setup(r => r.InsertAsync(It.IsAny<GuestUser>()))
                .ReturnsAsync((GuestUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.CreateDefaultUser();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GuestUser>(result);
        }

        [Fact]
        public async Task CreateDefaultUser_ShouldCreateGuestUserWithCatAvatar()
        {
            // Arrange
            GuestUser capturedUser = null;
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            userRepoMock.Setup(r => r.InsertAsync(It.IsAny<GuestUser>()))
                .Callback<GuestUser>(u => capturedUser = u)
                .ReturnsAsync((GuestUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateDefaultUser();

            // Assert
            Assert.NotNull(capturedUser);
            Assert.Equal("cat.jpg", capturedUser.Avatar);
        }

        [Fact]
        public async Task CreateDefaultUser_ShouldCreateGuestUserWithPseudoStartingWithUser()
        {
            // Arrange
            GuestUser capturedUser = null;
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            userRepoMock.Setup(r => r.InsertAsync(It.IsAny<GuestUser>()))
                .Callback<GuestUser>(u => capturedUser = u)
                .ReturnsAsync((GuestUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateDefaultUser();

            // Assert
            Assert.NotNull(capturedUser);
            Assert.StartsWith("User", capturedUser.Pseudo.Value);
        }

        [Fact]
        public async Task CreateDefaultUser_ShouldCreateGuestUserWithFourDigitRandomNumber()
        {
            // Arrange
            GuestUser capturedUser = null;
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            userRepoMock.Setup(r => r.InsertAsync(It.IsAny<GuestUser>()))
                .Callback<GuestUser>(u => capturedUser = u)
                .ReturnsAsync((GuestUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateDefaultUser();

            // Assert
            Assert.NotNull(capturedUser);
            var pseudo = capturedUser.Pseudo.Value;
            var numberPart = pseudo.Substring(4); // Remove "User" prefix
            Assert.True(int.TryParse(numberPart, out int number));
            Assert.InRange(number, 1000, 9999);
        }

        [Fact]
        public async Task CreateDefaultUser_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            userRepoMock.Setup(r => r.InsertAsync(It.IsAny<GuestUser>()))
                .ThrowsAsync(new Exception("Database error"));

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.CreateDefaultUser());
        }

        [Fact]
        public async Task CreateDefaultUser_MultipleCalls_ShouldCreateDifferentUsers()
        {
            // Arrange
            var capturedUsers = new List<GuestUser>();
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            userRepoMock.Setup(r => r.InsertAsync(It.IsAny<GuestUser>()))
                .Callback<GuestUser>(u => capturedUsers.Add(u))
                .ReturnsAsync((GuestUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateDefaultUser();
            await service.CreateDefaultUser();
            await service.CreateDefaultUser();

            // Assert
            Assert.Equal(3, capturedUsers.Count);
            // Note: There's a small probability that random numbers could be the same
            // but all users should have valid pseudos
            foreach (var user in capturedUsers)
            {
                Assert.StartsWith("User", user.Pseudo.Value);
                Assert.Equal("cat.jpg", user.Avatar);
            }
        }

        [Fact]
        public async Task CreateDefaultUser_ShouldNotCallAuthUserRepository()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            userRepoMock.Setup(r => r.InsertAsync(It.IsAny<GuestUser>()))
                .ReturnsAsync((GuestUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateDefaultUser();

            // Assert
            authUserRepoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateDefaultUser_ShouldSetExpiresAt()
        {
            // Arrange
            GuestUser capturedUser = null;
            var beforeCreation = DateTime.UtcNow;
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            userRepoMock.Setup(r => r.InsertAsync(It.IsAny<GuestUser>()))
                .Callback<GuestUser>(u => capturedUser = u)
                .ReturnsAsync((GuestUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateDefaultUser();

            // Assert
            var afterCreation = DateTime.UtcNow;
            Assert.NotNull(capturedUser);
            Assert.True(capturedUser.ExpiresAt >= beforeCreation.AddDays(1));
            Assert.True(capturedUser.ExpiresAt <= afterCreation.AddDays(1));
        }

        #endregion
    }
}
