using Vikalp.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vikalp.Service.Interfaces
{
    public interface IUserService
    {
        Task<string> GetUsersAsync();
        List<UserDto> GetAll();
        UserDto? GetById(int id);
        Guid Create(UserDto user);
        bool Update(UserDto user);
        bool Delete(int id);

    }
}
