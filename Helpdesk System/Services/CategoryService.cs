using Helpdesk_System.Data;
using Helpdesk_System.Models.Entities;
using Helpdesk_System.Services.Interfaces;
using Helpdesk_System.ViewModels.Categories;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_System.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly HelpdeskSystemDbContext _context;

        public CategoryService(HelpdeskSystemDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryIndexViewModel>> GetAllAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryIndexViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<CategoryEditViewModel?> GetByIdToEditAsync(int id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryEditViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CategoryDeleteViewModel?> GetByIdToDeleteAsync(int id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryDeleteViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(CategoryCreateViewModel model)
        {
            var category = new Category
            {
                Name = model.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CategoryEditViewModel model)
        {
            var category = await _context.Categories.FindAsync(model.Id);

            if (category == null)
                return;

            category.Name = model.Name;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}