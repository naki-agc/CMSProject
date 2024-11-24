using CMSProject.Application.Dtos;
using CMSProject.Application.Interfaces;
using CMSProject.Core.Domain.Entities;
using CMSProject.Core.Domain.Exceptions.CMSProject.Core.Exceptions;
using CMSProject.Core.Domain.Interfaces;
using CMSProject.Infrastructure.Cache;
using Mapster;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSProject.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<UserDto> GetUserAsync(int id)
        {
            string cacheKey = $"user_{id}";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                    throw new NotFoundException($"User with id {id} not found");

                return user.Adapt<UserDto>();
            });
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            string cacheKey = $"user_email_{email}";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(email);
                if (user == null)
                    throw new NotFoundException($"User with email {email} not found");

                return user.Adapt<UserDto>();
            });
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            string cacheKey = "all_users";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var users = await _unitOfWork.Users.GetAllAsync();
                return users.Adapt<IEnumerable<UserDto>>();

            });
        }

        public async Task<int> CreateUserAsync(UserDto userDto)
        {
            var user = userDto.Adapt<User>();
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Cache'i temizle
            await _cacheService.RemoveAsync("all_users");

            return user.Id;
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userDto.Id);
            if (user == null)
                throw new NotFoundException($"User with id {userDto.Id} not found");

            userDto.Adapt(user);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // İlgili cache'leri temizle
            await _cacheService.RemoveAsync($"user_{user.Id}");
            await _cacheService.RemoveAsync($"user_email_{user.Email}");
            await _cacheService.RemoveAsync("all_users");
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException($"User with id {id} not found");

            await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // İlgili cache'leri temizle
            await _cacheService.RemoveAsync($"user_{id}");
            await _cacheService.RemoveAsync($"user_email_{user.Email}");
            await _cacheService.RemoveAsync("all_users");
        }

        public async Task<IEnumerable<ContentDto>> GetUserContentsAsync(int userId)
        {
            string cacheKey = $"user_{userId}_contents";
            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var contents = await _unitOfWork.Users.GetUserContentsAsync(userId);
                return contents.Adapt<IEnumerable<ContentDto>>();
            });
        }

        public async Task AssignContentToUserAsync(int userId, int contentId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with id {userId} not found");

            var content = await _unitOfWork.Contents.GetByIdAsync(contentId);
            if (content == null)
                throw new NotFoundException($"Content with id {contentId} not found");

            await _unitOfWork.SaveChangesAsync();

            // İlgili cache'leri temizle
            await _cacheService.RemoveAsync($"user_{userId}_contents");
        }

        public Task<int> UserAsync(UserDto userDto)
        {
            throw new NotImplementedException();
        }
    }
}
