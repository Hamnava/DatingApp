using Business.PublicClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseAPIController : ControllerBase
    {
    }
}
