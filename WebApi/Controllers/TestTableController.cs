using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Interfaces;
using WebApi.ViewModels.Common;
using WebApi.ViewModels.TestTable;

namespace WebApi.Controllers
{
    public class TestTableController : ApiControllerBase
    {
        private readonly ITestTableService _testTableService;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public TestTableController(ITestTableService testTableService, IAuthenticatedUserService authenticatedUserService)
        {
            _testTableService = testTableService;
            _authenticatedUserService = authenticatedUserService;
        }

        [HttpGet("create")]
        public IActionResult Create(TestTableViewModel vm)
        {
            vm.UserId = _authenticatedUserService.UserId;
            var result = _testTableService.Add(vm);

            return Ok(result);
        }

        [HttpPut("{id}/update")]
        public IActionResult Update([FromRoute] int id, TestTableViewModel vm)
        {
            vm.Id = id;
            var result = _testTableService.Update(vm);

            return Ok(result);
        }

        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            var result = _testTableService.Delete(id);

            return Ok(result);
        }

        [HttpGet("get-all")]
        public IActionResult GetAll(RequestParameter vm)
        {
            var result = _testTableService.GetAll(vm);

            return Ok(result);
        }
    }
}
