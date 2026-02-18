using Vikalp.Models.DTO;

namespace Vikalp.Service.Interfaces
{
    public interface ILineListingSurveyService
    {
        IEnumerable<LineListingSurveyDto> GetAllSurveys(int userid);
        LineListingSurveyDto? GetSurveyById(int id);

        Task<bool> InsertLineListingAsync(LineListingSurveyCreateDto model, int userId);
        Task<bool> UpdateSurvey(LineListingSurveyUpdateDto model, int userId);
        void DeleteSurvey(int id, int userid);

        Task<LineListingConsentDto?> GetConsentPrefillAsync(string guid);
        Task<bool> SaveConsentAsync(LineListingConsentDto dto, int userid);
        Task<LineListingSurveyDto> GetLineListingByIdAsync(int id);

    }
}
