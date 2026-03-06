using Vikalp.Models.DTO;

public interface IAshaService
{
    Task<List<AshaDto>> GetAllAsha();
    Task<AshaDto?> GetById(int id);
    Task Insert(AshaDto model, int userId);  
    Task Update(AshaDto model, int userId);  
    Task Delete(int id);
}