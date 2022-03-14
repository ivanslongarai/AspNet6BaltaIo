using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers;

[ApiController]
[Route("")]
public class CategoryController : ControllerBase
{
    [HttpGet("v1/categories")]
    [Authorize]
    public async Task<IActionResult> GetAllAsync([FromServices] BlogDataContext ctx,
        [FromServices] IMemoryCache cache)
    {
        try
        {
            // Using no cache
            //var categories = await ctx.Categories.ToListAsync();
            //if (categories.Count == 0)
            //    return NoContent();
            //return Ok(new ResultViewModel<List<Category>>(categories));

            var categories = await cache.GetOrCreate("CategoriesCache", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return GetCategories(ctx);
            });

            return Ok(new ResultViewModel<List<Category>>(categories));

        }
        catch (Exception e)
        {
            return StatusCode(500, new ResultViewModel<List<Category>>("X0007 - Falha interna no servidor"));
        }
    }

    private async Task<List<Category>> GetCategories(BlogDataContext ctx)
    {
        return await ctx.Categories.AsNoTracking().ToListAsync();
    }

    [HttpGet("v1/categories/{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetAByIdAsync([FromServices] BlogDataContext ctx, [FromRoute] int id)
    {
        try
        {
            var category = await ctx.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
                return NotFound(new ResultViewModel<Category>("36408023 - Conteúdo não encontrado"));
            return Ok(category);
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Category>("35AB44F4 - Erro interno no servidor"));
        }
    }

    [HttpPost("v1/categories")]
    [Authorize(Roles = "admin, author")]
    public async Task<IActionResult> PostAsync([FromServices] BlogDataContext ctx, [FromBody] EditorCategoryViewModel model)
    {      
        
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

        try
        {
            var category = new Category
            {
                Id = 0,
                Name = model.Name,
                Slug = model.Slug.ToLower()
            };

            await ctx.Categories.AddAsync(category);
            await ctx.SaveChangesAsync();
            return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<Category>("0EFA9026 - Não foi possível incluir a categoria"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Category>("E94515D3 - Falha interna no servidor"));
        }            
    }

    [HttpPut("v1/categories/{id:int}")]
    [Authorize(Roles = "admin, author")]
    public async Task<IActionResult> PutAsync([FromServices] BlogDataContext ctx, [FromRoute] int id, [FromBody] EditorCategoryViewModel model)
    {

        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

        var category = await ctx.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (category == null)
            return NotFound(new ResultViewModel<Category>("C79C5320 - Conteúdo não encontrado"));

        category.Name = model.Name;
        category.Slug = model.Slug;

        try
        {
            ctx.Categories.Update(category);
            await ctx.SaveChangesAsync();
            return Ok(category);
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<Category>("DD8A4B5C - Não foi possível alterar a categoria"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Category>("DD8A4B5C - Erro interno no servidor"));
        }
    }

    [HttpDelete("v1/categories/{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteAsync([FromServices] BlogDataContext ctx, [FromRoute] int id)
    {
        var category = await ctx.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (category == null)
            return NotFound(new ResultViewModel<Category>("6ECBE1B4 - Conteúdo não encontrado"));

        try
        {
            ctx.Categories.Remove(category);
            await ctx.SaveChangesAsync();
            return Ok();
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<Category>("BF6BE7C7 - Não foi possível excluir o registro"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Category>("86718C83 - Erro interno no servidor"));
        }
    }
}
