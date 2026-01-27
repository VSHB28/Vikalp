using Vikalp.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface ILocationDistrictService
    {
        List<LocationDistrictDTO> GetAll();
        LocationDistrictDTO GetById(int districtId);
        string Insert(LocationDistrictDTO model);
        string Update(LocationDistrictDTO model);
        string Delete(int districtId);
        List<LocationDistrictDTO> GetByStateId(int value);
    }
}
