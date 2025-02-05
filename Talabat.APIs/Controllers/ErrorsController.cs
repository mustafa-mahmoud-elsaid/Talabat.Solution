using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Controllers
{
    [Route("errors/{statusCode}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        public ActionResult error(int statusCode)
        {
            if (statusCode == 404)
                return NotFound(new ApiResponse(404));
            else if (statusCode == 401)
                return NotFound(new ApiResponse(401));
            else
                return StatusCode(statusCode);
        }
    }
}
