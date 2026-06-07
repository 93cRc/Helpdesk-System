using Helpdesk_System.Data;
using Helpdesk_System.Models;
using Helpdesk_System.Models.Entities;
using Helpdesk_System.Services.Interfaces;
using Helpdesk_System.ViewModels.Departments;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk_System.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly HelpdeskSystemDbContext _context;

        public DepartmentService(HelpdeskSystemDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentIndexViewModel>> GetAllAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentIndexViewModel
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .ToListAsync();
        }

        public async Task CreateAsync(DepartmentCreateViewModel model)
        {
            var department = new Department
            {
                Name = model.Name
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }

        public async Task<DepartmentEditViewModel?> GetByIdToEditAsync(int id)
        {
            return await _context.Departments
                .Where(d => d.Id == id)
                .Select(d => new DepartmentEditViewModel
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(DepartmentEditViewModel model)
        {
            var department = await _context.Departments.FindAsync(model.Id);

            if (department == null)
            {
                return;
            }

            department.Name = model.Name;
            await _context.SaveChangesAsync();
        }

        public async Task<DepartmentDeleteViewModel?> GetByIdToDeleteAsync(int id)
        {
            return await _context.Departments
                .Where(d => d.Id == id)
                .Select(d => new DepartmentDeleteViewModel
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return;
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }
    }
}