using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Amin.Controllers
{
    [Route("api/ToDo/{ToDoId}/Categories/{CategoryId}")]
    [ApiController]
    public class ToDoCategoryController : ControllerBase
    {
        [HttpPost]
         public IActionResult POST(int ToDo , int CategoryId)
         {
            return Ok();    
         }
    }
}
