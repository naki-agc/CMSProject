using CMSProject.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSProject.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserAsync(int id);

        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<int> CreateUserAsync(UserDto userDto);

        Task UpdateUserAsync(UserDto userDto);

        Task DeleteUserAsync(int id);

        // Kullanıcının içeriklerini getirme
        Task<IEnumerable<ContentDto>> GetUserContentsAsync(int userId);
    }
}
