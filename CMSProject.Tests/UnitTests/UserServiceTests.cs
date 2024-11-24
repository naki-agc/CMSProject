using Xunit;
using Moq;
using FluentAssertions;
using CMSProject.Application.Services;
using CMSProject.Application.Interfaces;
using CMSProject.Application.Dtos;
using CMSProject.Core.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using CMSProject.Core.Domain.Interfaces;
using CMSProject.Infrastructure.Cache;
using CMSProject.Core.Domain.Exceptions.CMSProject.Core.Exceptions;

namespace CMSProject.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _sut; // System Under Test


        public UserServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCacheService = new Mock<ICacheService>();
            _mockLogger = new Mock<ILogger<UserService>>();

            _sut = new UserService(
                _mockUnitOfWork.Object,
                _mockCacheService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetUserAsync_WithValidId_ReturnsUserDto()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                FullName = "Test User",
                Email = "test@test.com"
            };

            _mockUnitOfWork.Setup(x => x.Users.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _sut.GetUserAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            result.FullName.Should().Be(user.FullName);
            result.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task GetUserAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            var userId = 999;
            _mockUnitOfWork.Setup(x => x.Users.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await _sut.Invoking(x => x.GetUserAsync(userId))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage($"User with id {userId} not found");
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new() { Id = 1, FullName = "User 1", Email = "user1@test.com" },
                new() { Id = 2, FullName = "User 2", Email = "user2@test.com" }
            };

            _mockUnitOfWork.Setup(x => x.Users.GetAllAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _sut.GetAllUsersAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(users, options =>
                options.ExcludingMissingMembers());
        }

        [Fact]
        public async Task CreateUserAsync_WithExistingEmail_ThrowsValidationException()
        {
            // Arrange
            var existingUser = new User
            {
                Id = 1,
                Email = "existing@test.com",
                FullName = "Existing User"
            };

            var createUserDto = new UserDto
            {
                Email = existingUser.Email,
                FullName = "New User"
            };

            _mockUnitOfWork.Setup(x => x.Users.GetByEmailAsync(createUserDto.Email))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await _sut.Invoking(x => x.CreateUserAsync(createUserDto))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage($"User with email {createUserDto.Email} already exists");
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidData_UpdatesUser()
        {
            // Arrange
            var updateUserDto = new UserDto
            {
                Id = 1,
                FullName = "Updated User",
                Email = "updated@test.com"
            };

            var existingUser = new User
            {
                Id = updateUserDto.Id,
                FullName = "Original User",
                Email = "original@test.com"
            };

            _mockUnitOfWork.Setup(x => x.Users.GetByIdAsync(updateUserDto.Id))
                .ReturnsAsync(existingUser);

            _mockUnitOfWork.Setup(x => x.Users.GetByEmailAsync(updateUserDto.Email))
                .ReturnsAsync((User)null);

            // Act
            await _sut.UpdateUserAsync(updateUserDto);

            // Assert
            _mockUnitOfWork.Verify(x => x.Users.UpdateAsync(It.IsAny<User>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
            _mockCacheService.Verify(x => x.RemoveAsync($"user_{updateUserDto.Id}"), Times.Once);
        }

    }
}