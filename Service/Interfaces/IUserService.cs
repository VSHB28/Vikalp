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
        (List<UserDto> Data, int TotalRecords) GetAll(int userId, int page, int pageSize, int? stateId, int? districtId);
        UserDto? GetById(int id);
        int Create(UserDto user);
        bool Update(UserDto user);
        bool Delete(int id);
        Task<List<UserDto>> SearchUser(string term);

    }
}
