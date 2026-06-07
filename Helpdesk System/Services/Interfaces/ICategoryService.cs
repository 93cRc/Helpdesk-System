using Helpdesk_System.ViewModels.Categories;

namespace Helpdesk_System.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryIndexViewModel>> GetAllAsync();
        Task<CategoryEditViewModel?> GetByIdToEditAsync(int id);
        Task<CategoryDeleteViewModel?> GetByIdToDeleteAsync(int id);
        Task CreateAsync(CategoryCreateViewModel model);
        Task UpdateAsync(CategoryEditViewModel model);
        Task DeleteAsync(int id);
    }
}