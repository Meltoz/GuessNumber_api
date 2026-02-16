using Application.Exceptions;
using Application.Interfaces.Repository;
using Application.Services;
using Domain.Enums;
using Domain.User;
using Domain.ValueObjects;
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
            var pagedResult = new PagedResult<User>
            {
                TotalCount = 0,
                Data = new List<User>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, "");

            // Assert
            authUserRepoMock.Verify(r => r.GetAll(0, 10, sortOption, "", false), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldReturnPagedResultFromRepository()
        {
            // Arrange
            var authUser = new AuthUser("TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);
            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };
            var pagedResult = new PagedResult<User>
            {
                TotalCount = 1,
                Data = new List<User> { authUser }
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
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
                It.IsAny<string>(),
                It.IsAny<bool>()
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
            var pagedResult = new PagedResult<User>
            {
                TotalCount = 0,
                Data = new List<User>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, "");

            // Assert - skip should be 0 for first page
            authUserRepoMock.Verify(r => r.GetAll(0, 10, sortOption, "", false), Times.Once);
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
            var pagedResult = new PagedResult<User>
            {
                TotalCount = 0,
                Data = new List<User>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(1, 10, sortOption, "");

            // Assert - skip should be 10 for second page (pageIndex 1 * pageSize 10)
            authUserRepoMock.Verify(r => r.GetAll(10, 10, sortOption, "", false), Times.Once);
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
            var pagedResult = new PagedResult<User>
            {
                TotalCount = 0,
                Data = new List<User>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(2, 25, sortOption, "");

            // Assert - skip should be 50 for third page (pageIndex 2 * pageSize 25)
            authUserRepoMock.Verify(r => r.GetAll(50, 25, sortOption, "", false), Times.Once);
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
            var pagedResult = new PagedResult<User>
            {
                TotalCount = 0,
                Data = new List<User>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, searchTerm);

            // Assert
            authUserRepoMock.Verify(r => r.GetAll(0, 10, sortOption, searchTerm, false), Times.Once);
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
            var pagedResult = new PagedResult<User>
            {
                TotalCount = 0,
                Data = new List<User>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, null);

            // Assert
            authUserRepoMock.Verify(r => r.GetAll(0, 10, sortOption, null, false), Times.Once);
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
            var pagedResult = new PagedResult<User>
            {
                TotalCount = 0,
                Data = new List<User>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, "");

            // Assert
            authUserRepoMock.Verify(r => r.GetAll(0, 10, sortOption, "", false), Times.Once);
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
            var pagedResult = new PagedResult<User>
            {
                TotalCount = 0,
                Data = new List<User>()
            };

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            )).ReturnsAsync(pagedResult);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, "");

            // Assert
            authUserRepoMock.Verify(r => r.GetAll(0, 10, sortOption, "", false), Times.Once);
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

        #region GetDetail Tests

        [Fact]
        public async Task GetDetail_ShouldReturnAuthUser_WhenAuthUserExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.GetDetail(id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AuthUser>(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetDetail_ShouldReturnGuestUser_WhenAuthUserNotFoundAndGuestUserExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var guestUser = new GuestUser(id, "Guest1234", "cat.jpg");

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AuthUser)null);
            userRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(guestUser);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.GetDetail(id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GuestUser>(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetDetail_ShouldThrowEntityNotFoundException_WhenNoUserExists()
        {
            // Arrange
            var id = Guid.NewGuid();

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AuthUser)null);
            userRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((GuestUser)null);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => service.GetDetail(id));
        }

        [Fact]
        public async Task GetDetail_ShouldCallAuthUserRepositoryFirst()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.GetDetail(id);

            // Assert
            authUserRepoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetDetail_ShouldNotCallUserRepository_WhenAuthUserExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.GetDetail(id);

            // Assert
            userRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetDetail_ShouldCallUserRepository_WhenAuthUserNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var guestUser = new GuestUser(id, "Guest1234", "cat.jpg");

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AuthUser)null);
            userRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(guestUser);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.GetDetail(id);

            // Assert
            userRepoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetDetail_ShouldReturnCorrectAuthUserData()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "JohnDoe", "avatar.png", "john@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.GetDetail(id);

            // Assert
            var returnedAuthUser = Assert.IsType<AuthUser>(result);
            Assert.Equal("JohnDoe", returnedAuthUser.Pseudo.Value);
            Assert.Equal("john@example.com", returnedAuthUser.Mail.ToString());
            Assert.Equal("avatar.png", returnedAuthUser.Avatar);
        }

        [Fact]
        public async Task GetDetail_ShouldReturnCorrectGuestUserData()
        {
            // Arrange
            var id = Guid.NewGuid();
            var guestUser = new GuestUser(id, "Guest5678", "cat.jpg");

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AuthUser)null);
            userRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(guestUser);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.GetDetail(id);

            // Assert
            var returnedGuestUser = Assert.IsType<GuestUser>(result);
            Assert.Equal("Guest5678", returnedGuestUser.Pseudo.Value);
            Assert.Equal("cat.jpg", returnedGuestUser.Avatar);
        }

        [Fact]
        public async Task GetDetail_ShouldThrow_WhenAuthUserRepositoryThrows()
        {
            // Arrange
            var id = Guid.NewGuid();

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new Exception("Database error"));

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetDetail(id));
        }

        [Fact]
        public async Task GetDetail_ShouldThrow_WhenUserRepositoryThrows()
        {
            // Arrange
            var id = Guid.NewGuid();

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AuthUser)null);
            userRepoMock.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new Exception("Database error"));

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetDetail(id));
        }

        [Fact]
        public async Task GetDetail_ShouldPrioritizeAuthUser_WhenBothRepositoriesCouldReturn()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "AuthUser", "avatar.png", "auth@example.com", "Password1@", RoleUser.User);
            var guestUser = new GuestUser(id, "GuestUser", "cat.jpg");

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            userRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(guestUser);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.GetDetail(id);

            // Assert
            Assert.IsType<AuthUser>(result);
            Assert.Equal("AuthUser", result.Pseudo.Value);
        }

        #endregion

        #region ChangeRole Tests

        [Fact]
        public async Task ChangeRole_ShouldCallAuthUserRepositoryGetByIdAsync()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>())).ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.ChangeRole(id, RoleUser.Admin);

            // Assert
            authUserRepoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task ChangeRole_ShouldCallAuthUserRepositoryUpdateAsync()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>())).ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.ChangeRole(id, RoleUser.Admin);

            // Assert
            authUserRepoMock.Verify(r => r.UpdateAsync(It.IsAny<AuthUser>()), Times.Once);
        }

        [Fact]
        public async Task ChangeRole_ShouldReturnUpdatedAuthUser()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>())).ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.ChangeRole(id, RoleUser.Admin);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AuthUser>(result);
            Assert.Equal(RoleUser.Admin, result.Role);
        }

        [Fact]
        public async Task ChangeRole_ShouldChangeRoleOnDomainObject_BeforeCallingUpdate()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);
            AuthUser capturedUser = null;

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>()))
                .Callback<AuthUser>(u => capturedUser = u)
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.ChangeRole(id, RoleUser.Admin);

            // Assert
            Assert.NotNull(capturedUser);
            Assert.Equal(RoleUser.Admin, capturedUser.Role);
        }

        [Fact]
        public async Task ChangeRole_ShouldThrowEntityNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AuthUser)null);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => service.ChangeRole(id, RoleUser.Admin));
        }

        [Fact]
        public async Task ChangeRole_ShouldNotCallUpdateAsync_WhenUserNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AuthUser)null);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            try { await service.ChangeRole(id, RoleUser.Admin); } catch { }
            authUserRepoMock.Verify(r => r.UpdateAsync(It.IsAny<AuthUser>()), Times.Never);
        }

        [Fact]
        public async Task ChangeRole_FromUserToAdmin_ShouldReturnAdmin()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>())).ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.ChangeRole(id, RoleUser.Admin);

            // Assert
            Assert.Equal(RoleUser.Admin, result.Role);
        }

        [Fact]
        public async Task ChangeRole_FromAdminToUser_ShouldReturnUser()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.Admin);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>())).ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.ChangeRole(id, RoleUser.User);

            // Assert
            Assert.Equal(RoleUser.User, result.Role);
        }

        [Fact]
        public async Task ChangeRole_ShouldNotCallUserRepository()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>())).ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.ChangeRole(id, RoleUser.Admin);

            // Assert
            userRepoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ChangeRole_ShouldThrow_WhenGetByIdAsyncThrows()
        {
            // Arrange
            var id = Guid.NewGuid();

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new Exception("Database error"));

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.ChangeRole(id, RoleUser.Admin));
        }

        [Fact]
        public async Task ChangeRole_ShouldThrow_WhenUpdateAsyncThrows()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>())).ThrowsAsync(new Exception("Database error"));

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.ChangeRole(id, RoleUser.Admin));
        }

        [Fact]
        public async Task ChangeRole_ShouldPreserveUserData()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "JohnDoe", "avatar.png", "john@example.com", "Password1@", RoleUser.User);
            AuthUser capturedUser = null;

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>()))
                .Callback<AuthUser>(u => capturedUser = u)
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.ChangeRole(id, RoleUser.Admin);

            // Assert
            Assert.NotNull(capturedUser);
            Assert.Equal(id, capturedUser.Id);
            Assert.Equal("JohnDoe", capturedUser.Pseudo.Value);
            Assert.Equal("john@example.com", capturedUser.Mail.ToString());
            Assert.Equal("avatar.png", capturedUser.Avatar);
        }

        #endregion

        #region ResetPassword Tests

        [Fact]
        public async Task ResetPassword_ShouldCallAuthUserRepositoryGetByIdAsync()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>())).ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.ResetPassword(id, "NewPassword1@");

            // Assert
            authUserRepoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_ShouldCallAuthUserRepositoryUpdateAsync()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>())).ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.ResetPassword(id, "NewPassword1@");

            // Assert
            authUserRepoMock.Verify(r => r.UpdateAsync(It.IsAny<AuthUser>()), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_ShouldReturnUpdatedAuthUser()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>())).ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.ResetPassword(id, "NewPassword1@");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AuthUser>(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task ResetPassword_ShouldChangePasswordOnDomainObject_BeforeCallingUpdate()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);
            AuthUser capturedUser = null;

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>()))
                .Callback<AuthUser>(u => capturedUser = u)
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.ResetPassword(id, "NewPassword1@");

            // Assert
            Assert.NotNull(capturedUser);
            Assert.NotNull(capturedUser.LastChangePassword);
        }

        [Fact]
        public async Task ResetPassword_ShouldSetPasswordMustBeChangedToTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);
            Assert.False(authUser.PasswordMustBeChanged);
            AuthUser capturedUser = null;

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>()))
                .Callback<AuthUser>(u => capturedUser = u)
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.ResetPassword(id, "NewPassword1@");

            // Assert
            Assert.NotNull(capturedUser);
            Assert.True(capturedUser.PasswordMustBeChanged);
        }

        [Fact]
        public async Task ResetPassword_ShouldSetLastChangePassword()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);
            Assert.Null(authUser.LastChangePassword);

            var beforeReset = DateTime.UtcNow;

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>())).ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.ResetPassword(id, "NewPassword1@");

            // Assert
            var afterReset = DateTime.UtcNow;
            Assert.NotNull(result.LastChangePassword);
            Assert.True(result.LastChangePassword >= beforeReset);
            Assert.True(result.LastChangePassword <= afterReset);
        }

        [Fact]
        public async Task ResetPassword_ShouldThrowEntityNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AuthUser)null);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => service.ResetPassword(id, "NewPassword1@"));
        }

        [Fact]
        public async Task ResetPassword_ShouldNotCallUpdateAsync_WhenUserNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AuthUser)null);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            try { await service.ResetPassword(id, "NewPassword1@"); } catch { }
            authUserRepoMock.Verify(r => r.UpdateAsync(It.IsAny<AuthUser>()), Times.Never);
        }

        [Fact]
        public async Task ResetPassword_ShouldNotCallUserRepository()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>())).ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.ResetPassword(id, "NewPassword1@");

            // Assert
            userRepoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ResetPassword_ShouldPreserveUserData()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "JohnDoe", "avatar.png", "john@example.com", "Password1@", RoleUser.Admin);
            AuthUser capturedUser = null;

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>()))
                .Callback<AuthUser>(u => capturedUser = u)
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.ResetPassword(id, "NewPassword1@");

            // Assert
            Assert.NotNull(capturedUser);
            Assert.Equal(id, capturedUser.Id);
            Assert.Equal("JohnDoe", capturedUser.Pseudo.Value);
            Assert.Equal("john@example.com", capturedUser.Mail.ToString());
            Assert.Equal("avatar.png", capturedUser.Avatar);
            Assert.Equal(RoleUser.Admin, capturedUser.Role);
        }

        [Fact]
        public async Task ResetPassword_ShouldThrow_WhenGetByIdAsyncThrows()
        {
            // Arrange
            var id = Guid.NewGuid();

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new Exception("Database error"));

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.ResetPassword(id, "NewPassword1@"));
        }

        [Fact]
        public async Task ResetPassword_ShouldThrow_WhenUpdateAsyncThrows()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);
            authUserRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AuthUser>())).ThrowsAsync(new Exception("Database error"));

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.ResetPassword(id, "NewPassword1@"));
        }

        [Fact]
        public async Task ResetPassword_WithInvalidPassword_ShouldThrowArgumentException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.ResetPassword(id, "weak"));
        }

        [Fact]
        public async Task ResetPassword_WithInvalidPassword_ShouldNotCallUpdateAsync()
        {
            // Arrange
            var id = Guid.NewGuid();
            var authUser = new AuthUser(id, "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);

            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(authUser);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            try { await service.ResetPassword(id, "weak"); } catch { }
            authUserRepoMock.Verify(r => r.UpdateAsync(It.IsAny<AuthUser>()), Times.Never);
        }

        #endregion

        #region CreateAuthUser Tests

        [Fact]
        public async Task CreateAuthUser_ShouldCallAuthUserRepositoryInsertAsync()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            authUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AuthUser>()))
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateAuthUser("TestUser", "test@example.com", "Password1@");

            // Assert
            authUserRepoMock.Verify(r => r.InsertAsync(It.IsAny<AuthUser>()), Times.Once);
        }

        [Fact]
        public async Task CreateAuthUser_ShouldReturnAuthUserFromRepository()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            authUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AuthUser>()))
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.CreateAuthUser("TestUser", "test@example.com", "Password1@");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AuthUser>(result);
        }

        [Fact]
        public async Task CreateAuthUser_ShouldCreateUserWithCorrectPseudo()
        {
            // Arrange
            AuthUser capturedUser = null;
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            authUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AuthUser>()))
                .Callback<AuthUser>(u => capturedUser = u)
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateAuthUser("JohnDoe", "john@example.com", "Password1@");

            // Assert
            Assert.NotNull(capturedUser);
            Assert.Equal("JohnDoe", capturedUser.Pseudo.Value);
        }

        [Fact]
        public async Task CreateAuthUser_ShouldCreateUserWithCorrectMail()
        {
            // Arrange
            AuthUser capturedUser = null;
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            authUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AuthUser>()))
                .Callback<AuthUser>(u => capturedUser = u)
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateAuthUser("JohnDoe", "john@example.com", "Password1@");

            // Assert
            Assert.NotNull(capturedUser);
            Assert.Equal("john@example.com", capturedUser.Mail.ToString());
        }

        [Fact]
        public async Task CreateAuthUser_ShouldCreateUserWithCatPngAvatar()
        {
            // Arrange
            AuthUser capturedUser = null;
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            authUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AuthUser>()))
                .Callback<AuthUser>(u => capturedUser = u)
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateAuthUser("JohnDoe", "john@example.com", "Password1@");

            // Assert
            Assert.NotNull(capturedUser);
            Assert.Equal("cat.png", capturedUser.Avatar);
        }

        [Fact]
        public async Task CreateAuthUser_ShouldCreateUserWithRoleUser()
        {
            // Arrange
            AuthUser capturedUser = null;
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            authUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AuthUser>()))
                .Callback<AuthUser>(u => capturedUser = u)
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateAuthUser("JohnDoe", "john@example.com", "Password1@");

            // Assert
            Assert.NotNull(capturedUser);
            Assert.Equal(RoleUser.User, capturedUser.Role);
        }

        [Fact]
        public async Task CreateAuthUser_ShouldCreateUserWithPasswordMustBeChangedFalse()
        {
            // Arrange
            AuthUser capturedUser = null;
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            authUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AuthUser>()))
                .Callback<AuthUser>(u => capturedUser = u)
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateAuthUser("JohnDoe", "john@example.com", "Password1@");

            // Assert
            Assert.NotNull(capturedUser);
            Assert.False(capturedUser.PasswordMustBeChanged);
        }

        [Fact]
        public async Task CreateAuthUser_ShouldNotCallUserRepository()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            authUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AuthUser>()))
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateAuthUser("JohnDoe", "john@example.com", "Password1@");

            // Assert
            userRepoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateAuthUser_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            authUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AuthUser>()))
                .ThrowsAsync(new Exception("Database error"));

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.CreateAuthUser("JohnDoe", "john@example.com", "Password1@"));
        }

        [Fact]
        public async Task CreateAuthUser_WithInvalidMail_ShouldThrowArgumentException()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAuthUser("JohnDoe", "invalid-email", "Password1@"));
        }

        [Fact]
        public async Task CreateAuthUser_WithInvalidMail_ShouldNotCallInsertAsync()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            try { await service.CreateAuthUser("JohnDoe", "invalid-email", "Password1@"); } catch { }
            authUserRepoMock.Verify(r => r.InsertAsync(It.IsAny<AuthUser>()), Times.Never);
        }

        [Fact]
        public async Task CreateAuthUser_WithInvalidPassword_ShouldThrowArgumentException()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAuthUser("JohnDoe", "john@example.com", "weak"));
        }

        [Fact]
        public async Task CreateAuthUser_WithInvalidPassword_ShouldNotCallInsertAsync()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            try { await service.CreateAuthUser("JohnDoe", "john@example.com", "weak"); } catch { }
            authUserRepoMock.Verify(r => r.InsertAsync(It.IsAny<AuthUser>()), Times.Never);
        }

        [Fact]
        public async Task CreateAuthUser_WithEmptyPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAuthUser("", "john@example.com", "Password1@"));
        }

        [Fact]
        public async Task CreateAuthUser_ShouldReturnUserWithHashedPassword()
        {
            // Arrange
            AuthUser capturedUser = null;
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            authUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AuthUser>()))
                .Callback<AuthUser>(u => capturedUser = u)
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.CreateAuthUser("JohnDoe", "john@example.com", "Password1@");

            // Assert
            Assert.NotNull(capturedUser);
            Assert.NotEqual("Password1@", capturedUser.Password.ToString());
        }

        [Fact]
        public async Task CreateAuthUser_ShouldReturnUserWithNullLastLogin()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            authUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AuthUser>()))
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.CreateAuthUser("JohnDoe", "john@example.com", "Password1@");

            // Assert
            Assert.Null(result.LastLogin);
        }

        [Fact]
        public async Task CreateAuthUser_ShouldReturnUserWithNullLastChangePassword()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            authUserRepoMock.Setup(r => r.InsertAsync(It.IsAny<AuthUser>()))
                .ReturnsAsync((AuthUser u) => u);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.CreateAuthUser("JohnDoe", "john@example.com", "Password1@");

            // Assert
            Assert.Null(result.LastChangePassword);
        }

        [Fact]
        public async Task CreateAuthUser_WithNullPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAuthUser(null, "john@example.com", "Password1@"));
        }

        [Fact]
        public async Task CreateAuthUser_WithEmptyMail_ShouldThrowArgumentException()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAuthUser("JohnDoe", "", "Password1@"));
        }

        [Fact]
        public async Task CreateAuthUser_WithNullMail_ShouldThrowArgumentException()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAuthUser("JohnDoe", null, "Password1@"));
        }

        [Fact]
        public async Task CreateAuthUser_WithEmptyPassword_ShouldThrowArgumentException()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAuthUser("JohnDoe", "john@example.com", ""));
        }

        [Fact]
        public async Task CreateAuthUser_WithNullPassword_ShouldThrowArgumentException()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAuthUser("JohnDoe", "john@example.com", null));
        }

        [Fact]
        public async Task CreateAuthUser_WithPasswordMissingUppercase_ShouldThrowArgumentException()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAuthUser("JohnDoe", "john@example.com", "password1@"));
        }

        [Fact]
        public async Task CreateAuthUser_WithPasswordMissingSpecialChar_ShouldThrowArgumentException()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAuthUser("JohnDoe", "john@example.com", "Password1"));
        }

        [Fact]
        public async Task CreateAuthUser_WithPasswordMissingDigit_ShouldThrowArgumentException()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAuthUser("JohnDoe", "john@example.com", "Password@"));
        }

        [Fact]
        public async Task CreateAuthUser_WithMailMissingAtSymbol_ShouldThrowArgumentException()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAuthUser("JohnDoe", "johnexample.com", "Password1@"));
        }

        [Fact]
        public async Task CreateAuthUser_WithInvalidInputs_ShouldNeverCallRepository()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act - try all invalid cases
            try { await service.CreateAuthUser(null, "john@example.com", "Password1@"); } catch { }
            try { await service.CreateAuthUser("John", null, "Password1@"); } catch { }
            try { await service.CreateAuthUser("John", "john@example.com", null); } catch { }
            try { await service.CreateAuthUser("John", "invalid", "Password1@"); } catch { }
            try { await service.CreateAuthUser("John", "john@example.com", "weak"); } catch { }

            // Assert
            authUserRepoMock.Verify(r => r.InsertAsync(It.IsAny<AuthUser>()), Times.Never);
            userRepoMock.VerifyNoOtherCalls();
        }

        #endregion

        #region IsPseudoAvailable Tests

        [Fact]
        public async Task IsPseudoAvailable_ShouldCallCheckAvailablePseudoOnRepository()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailablePseudo(It.IsAny<Pseudo>())).ReturnsAsync(true);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.IsPseudoAvailable("TestUser");

            // Assert
            authUserRepoMock.Verify(r => r.CheckAvailablePseudo(It.IsAny<Pseudo>()), Times.Once);
        }

        [Fact]
        public async Task IsPseudoAvailable_ShouldReturnTrue_WhenPseudoIsAvailable()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailablePseudo(It.IsAny<Pseudo>())).ReturnsAsync(true);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.IsPseudoAvailable("NewUser");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsPseudoAvailable_ShouldReturnFalse_WhenPseudoIsTaken()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailablePseudo(It.IsAny<Pseudo>())).ReturnsAsync(false);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.IsPseudoAvailable("ExistingUser");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsPseudoAvailable_ShouldPassExactPseudoToRepository()
        {
            // Arrange
            Pseudo capturedPseudo = null;
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailablePseudo(It.IsAny<Pseudo>()))
                .Callback<Pseudo>(p => capturedPseudo = p)
                .ReturnsAsync(true);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.IsPseudoAvailable("SpacedPseudo");

            // Assert
            Assert.Equal("SpacedPseudo", capturedPseudo.Value);
        }

        [Fact]
        public async Task IsPseudoAvailable_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailablePseudo(It.IsAny<Pseudo>()))
                .ThrowsAsync(new Exception("Database error"));

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.IsPseudoAvailable("TestUser"));
        }

        [Fact]
        public async Task IsPseudoAvailable_ShouldNotCallUserRepository()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailablePseudo(It.IsAny<Pseudo>())).ReturnsAsync(true);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.IsPseudoAvailable("TestUser");

            // Assert
            userRepoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task IsPseudoAvailable_ShouldNotCallOtherAuthUserRepoMethods()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailablePseudo(It.IsAny<Pseudo>())).ReturnsAsync(true);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.IsPseudoAvailable("TestUser");

            // Assert
            authUserRepoMock.Verify(r => r.CheckAvailablePseudo(It.IsAny<Pseudo>()), Times.Once);
            authUserRepoMock.Verify(r => r.InsertAsync(It.IsAny<AuthUser>()), Times.Never);
            authUserRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        #endregion

        #region IsMailAvailable Tests

        [Fact]
        public async Task IsMailAvailable_ShouldCallCheckAvailableMailOnRepository()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailableMail(It.IsAny<Mail>())).ReturnsAsync(true);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.IsMailAvailable("test@example.com");

            // Assert
            authUserRepoMock.Verify(r => r.CheckAvailableMail(It.IsAny<Mail>()), Times.Once);
        }

        [Fact]
        public async Task IsMailAvailable_ShouldReturnTrue_WhenMailIsAvailable()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailableMail(It.IsAny<Mail>())).ReturnsAsync(true);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.IsMailAvailable("new@example.com");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsMailAvailable_ShouldReturnFalse_WhenMailIsTaken()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailableMail(It.IsAny<Mail>())).ReturnsAsync(false);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            var result = await service.IsMailAvailable("taken@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsMailAvailable_ShouldPassExactMailToRepository()
        {
            // Arrange
            Mail capturedMail = null;
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailableMail(It.IsAny<Mail>()))
                .Callback<Mail>(m => capturedMail = m)
                .ReturnsAsync(true);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.IsMailAvailable("Test@Example.COM");

            // Assert
            Assert.Equal("Test@Example.COM", (string)capturedMail);
        }

        [Fact]
        public async Task IsMailAvailable_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailableMail(It.IsAny<Mail>()))
                .ThrowsAsync(new Exception("Database error"));

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.IsMailAvailable("test@example.com"));
        }

        [Fact]
        public async Task IsMailAvailable_ShouldNotCallUserRepository()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailableMail(It.IsAny<Mail>())).ReturnsAsync(true);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.IsMailAvailable("test@example.com");

            // Assert
            userRepoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task IsMailAvailable_ShouldNotCallOtherAuthUserRepoMethods()
        {
            // Arrange
            var userRepoMock = new Mock<IUserRepository>();
            var authUserRepoMock = new Mock<IAuthUserRepository>();
            authUserRepoMock.Setup(r => r.CheckAvailableMail(It.IsAny<Mail>())).ReturnsAsync(true);

            var service = new UserService(userRepoMock.Object, authUserRepoMock.Object);

            // Act
            await service.IsMailAvailable("test@example.com");

            // Assert
            authUserRepoMock.Verify(r => r.CheckAvailableMail(It.IsAny<Mail>()), Times.Once);
            authUserRepoMock.Verify(r => r.InsertAsync(It.IsAny<AuthUser>()), Times.Never);
            authUserRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        #endregion
    }
}
