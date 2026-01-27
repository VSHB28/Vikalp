namespace Vikalp.Service.Interfaces
{
    public interface ILocationBlockService
    {
        Task<IEnumerable<LocationBlockDto>> GetAllAsync();
        Task<LocationBlockDto?> GetByIdAsync(int blockId);
        Task AddAsync(LocationBlockDto blockDto);
        Task UpdateAsync(LocationBlockDto blockDto);
        Task DeleteAsync(int blockId);
        Task<IEnumerable<LocationBlockDto>> GetBlocksByDistrictAsync(int districtId);
    }
}


