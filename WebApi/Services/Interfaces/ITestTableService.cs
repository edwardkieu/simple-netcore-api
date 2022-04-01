using System.Collections.Generic;
using WebApi.ViewModels.Common;
using WebApi.ViewModels.TestTable;
using WebApi.Wrappers;

namespace WebApi.Services.Interfaces
{
    public interface ITestTableService
    {
        TestTableViewModel Add(TestTableViewModel vm);

        TestTableViewModel Update(TestTableViewModel vm);

        bool Delete(int id);

        Response<TestTableViewModel> GetById(int id);

        PagedResponse<List<TestTableViewModel>> GetAll(RequestParameter vm);
    }
}