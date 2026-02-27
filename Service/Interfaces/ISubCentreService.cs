// Service/SubCentreService.cs

using Vikalp.Models.DTO;

public interface ISubCentreService
{
    Task AddAsync(SubCentreDto dto);
    Task DeleteAsync(int id);
    Task<string?> GetAllAsync();
    Task<IEnumerable<object>> GetByBlockAsync(int blockId, int? facilityId);
    //Task<string?> GetByIdAsync(int id);
    Task<SubCentreDto?> GetByIdAsync(int id);
    Task UpdateAsync(SubCentreDto dto);

    Task SaveFacilityProfileAsync(FacilityProfileDto model);
    Task<FacilityProfileDto> GetFacilityProfileAsync(int profileId);
    Task SaveHrStatusAsync(HrStatusDto model);
    Task GetSubCentreProfileAsync(int subCenterId);
    Task SaveSubCentreProfileAsync(SubCentreProfileDto model);

  
}