using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using Web.Api.Amin.Models.Dto;
using Web.Api.Amin.Models.Entities;
using Web.Api.Amin.Models.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web.Api.Amin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly TodoRepository todoRepository;
        public ToDoController(TodoRepository todoRepository)
        {
            this.todoRepository = todoRepository;
        }

    
        // GET: api/<ToDoController>
        [HttpGet]
        public IActionResult Get()
        {
            var todoList = todoRepository.GetAll().Select(p => new ToDoItemDto
            {
                Id = p.Id,
                Text = p.Text,
                inserttime = p.InsertTime,
                Links = new List<Links>()
                {
                     new Links
                     {
                          Href=Url.Action(nameof(Get),"ToDo",new {p.Id},Request.Scheme),
                          Rel="Self",
                          Method="Get"
                     },

                    new Links
                     {
                          Href=Url.Action(nameof(Delete),"ToDo",new {p.Id},Request.Scheme),
                          Rel="Delete",
                          Method="Delete"
                     },

                     new Links
                     {
                          Href=Url.Action(nameof(Put),"ToDo",Request.Scheme),
                          Rel="Update",
                          Method="Put"
                     },
                }
            }).ToList();
            return Ok(todoList);

        }

        // GET api/<ToDoController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var todo = todoRepository.Get(id);


            return Ok(new ToDoItemDto
            {
                Id=todo.Id,
                Text = todo.Text,
                inserttime = todo.InsertTime,

            });
        }

        // POST api/<ToDoController>
        [HttpPost]
        public IActionResult Post([FromBody] ItemDto item)
        {
            var result = todoRepository.Add(new AddToDoDto()
            {
                Todo = new TodoDto
                {
                    Text = item.Text,
                }
            });

           string url= Url.Action(nameof(Get) , "ToDo" , new {Id = result.Todo.Id} , Request.Scheme);

            return Created(url,true);  
        }

        // PUT api/<ToDoController>/5
        [HttpPut]
        public IActionResult Put([FromBody] EditToDoDto editToDo)
        {
            var result = todoRepository.Edit(editToDo);
            return Ok(result);
        }

        // DELETE api/<ToDoController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            todoRepository.Delete(id);
            return Ok();
        }
    }
}
