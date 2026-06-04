using Helpdesk_System.ViewModels.Departments;

namespace Helpdesk_System.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentIndexViewModel>> GetAllAsync();
        Task CreateAsync(DepartmentCreateViewModel model);
        Task<DepartmentEditViewModel?> GetByIdToEditAsync(int id);
        Task UpdateAsync(DepartmentEditViewModel model);
        Task<DepartmentDeleteViewModel?> GetByIdToDeleteAsync(int id);
        Task DeleteAsync(int id);
    }
}