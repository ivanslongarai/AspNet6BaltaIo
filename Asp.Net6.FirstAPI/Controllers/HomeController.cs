using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Models;

namespace MVC.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public IActionResult GetAll([FromServices] AppDbContext ctx) =>
            Ok(ctx.Todos.ToList());

        [HttpGet("/{id:int}")]
        public IActionResult
        GetById([FromRoute] int id, [FromServices] AppDbContext ctx)
        {
            var model = ctx.Todos.FirstOrDefault(x => x.Id == id);
            if (model == null) return NotFound();
            return Ok(model);
        }

        [HttpPost("/")]
        public IActionResult
        Insert([FromBody] TodoModel todo, [FromServices] AppDbContext ctx)
        {
            ctx.Todos.Add (todo);
            ctx.SaveChanges();
            return Created($"/{todo.Id}", todo);
        }

        [HttpPut("/{id:int}")]
        public IActionResult
        Edit(
            [FromRoute] int id,
            [FromBody] TodoModel todo,
            [FromServices] AppDbContext ctx
        )
        {
            var model = ctx.Todos.FirstOrDefault(x => x.Id == id);
            if (model == null) return NotFound();
            model.Title = todo.Title;
            model.Done = todo.Done;
            ctx.Todos.Update (model);
            ctx.SaveChanges();
            return Ok(model);
        }

        [HttpDelete("/{id:int}")]
        public IActionResult
        Delete([FromRoute] int id, [FromServices] AppDbContext ctx)
        {
            var model = ctx.Todos.FirstOrDefault(x => x.Id == id);
            if (model == null) return NotFound();
            ctx.Todos.Remove (model);
            ctx.SaveChanges();
            return Ok();
        }
    }
}
