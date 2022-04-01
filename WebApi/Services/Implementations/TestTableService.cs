using System.Collections.Generic;
using System.Linq;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Services.Interfaces;
using WebApi.ViewModels.Common;
using WebApi.ViewModels.TestTable;
using WebApi.Wrappers;

namespace WebApi.Services.Implementations
{
    public class TestTableService : ITestTableService
    {
        private readonly ApplicationDbContext _dbContext;
        public TestTableService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TestTableViewModel Add(TestTableViewModel vm)
        {
            var entity = new TestTable
            {
                Name = vm.Name,
                Created = System.DateTime.Now,
                UserId = vm.UserId
            };

            _dbContext.TestTables.Add(entity);
            _dbContext.SaveChanges();
            vm.Id = entity.Id;
            vm.UserId = entity.UserId;
            vm.Created = entity.Created;
            vm.CreatedBy = entity.CreatedBy;

            return vm;
        }

        public bool Delete(int id)
        {
            var entity = _dbContext.TestTables.Find(id);
            if (entity == null)
                return false;

            _dbContext.TestTables.Remove(entity);
            return _dbContext.SaveChanges() > 0;
        }

        public PagedResponse<List<TestTableViewModel>> GetAll(RequestParameter vm)
        {
            var query = _dbContext.TestTables;
            var totalCount = query.Count();

            var data = query.Skip((vm.PageNumber - 1) * vm.PageSize).Take(vm.PageSize).Select(x => new TestTableViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Created = x.Created,
                CreatedBy = x.CreatedBy,
                LastModified = x.LastModified,
                LastModifiedBy = x.LastModifiedBy,
                UserId = x.UserId

            }).ToList();

            return new PagedResponse<List<TestTableViewModel>>(data, totalCount, vm.PageNumber, vm.PageSize);
        }

        public Response<TestTableViewModel> GetById(int id)
        {
            var entity = _dbContext.TestTables.FirstOrDefault(x => x.Id == id);
            if (entity == null)
                return null;

            var vm = new TestTableViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Created = entity.Created,
                CreatedBy = entity.CreatedBy,
                LastModified = entity.LastModified,
                LastModifiedBy = entity.LastModifiedBy,
                UserId = entity.UserId
            };

            return new Response<TestTableViewModel>(vm);
        }

        public TestTableViewModel Update(TestTableViewModel vm)
        {
            var entity = _dbContext.TestTables.FirstOrDefault(x => x.Id == vm.Id);
            if (entity == null)
                return null;

            entity.Name = vm.Name;
            entity.LastModified = System.DateTime.Now;
            entity.LastModifiedBy = vm.LastModifiedBy;
            _dbContext.TestTables.Update(entity);
            _dbContext.SaveChanges();

            return vm;
        }
    }
}
