using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    public abstract class ApiControllerBase : ControllerBase
    {
    }
}