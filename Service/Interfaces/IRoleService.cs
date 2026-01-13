using Vikalp.Models.DTO;
using System.Collections.Generic;

namespace Vikalp.Service
{
    public interface IRoleService
    {
        List<RoleDto> GetAll();
        RoleDto? GetById(int id);
        int Create(RoleDto role);
        bool Update(RoleDto role);
        bool Delete(int id);
    }
}