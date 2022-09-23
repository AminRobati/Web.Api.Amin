using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Amin.Models.Services;
using static Web.Api.Amin.Models.Services.CategoryRepository;

namespace Web.Api.Amin.Controllers
{
    [Route("api/[controller]")]
    //[Route("api/v{version:apiVersion }/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryRepository categoryRepository;

        public CategoriesController(CategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]   
        public IActionResult Get()
        {
            return Ok(categoryRepository.GetAll()); 
        }

        [HttpGet("{Id}")]
        public IActionResult Get(int Id)
        {
            return Ok(categoryRepository.Find(Id));
        }

        [HttpPut]  
        public IActionResult Put(CategoryDto categoryDto)
        {
            return Ok(categoryRepository.Edit(categoryDto));
        }

        [HttpPost]
        public IActionResult Post(string Name)
        {
            var result = categoryRepository.AddCategory(Name);
            return Created(Url.Action(nameof(Get), "Categories", new { Id = result }, Request.Scheme), true);

        }

        [HttpDelete]
        public IActionResult Delete(int Id)
        {
            return Ok(categoryRepository.Delete(Id));
        }

    }
}
