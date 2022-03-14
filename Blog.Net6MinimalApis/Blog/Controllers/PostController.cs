using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
public class PostController : ControllerBase
{
    [HttpGet("v1/posts")]
    public async Task<IActionResult> GetAllAsync([FromServices] BlogDataContext ctx,
        [FromQuery] int page,
        [FromQuery] int pageSize = 25)
    {

        var count = await ctx.Posts.CountAsync();

        var posts = await ctx.Posts
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Author)
            .Select(x => new ListPostsViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Slug = x.Slug,
                LastUpdateDate = x.LastUpdateDate,
                Category = x.Category.Name,
                Author = $"{x.Author.Name} - {x.Author.Email}"
            })
            .Skip(page * pageSize)
            .Take(pageSize)
            .OrderByDescending(x => x.LastUpdateDate)
            .ToListAsync();

        return Ok(new ResultViewModel<PaginateViewModel>(new PaginateViewModel
        {
            Total = count,
            NextPage = ++page,
            EndOfList = ((++page) * pageSize) >= count,
            PageSize = pageSize,
            Posts = posts
        }));
    }

    [HttpGet("v1/posts/{id:int}")]
    public async Task<IActionResult> GetByIdAsync([FromServices] BlogDataContext ctx,
        [FromRoute] int id)
    {
        try
        {
            var post = await ctx.Posts
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .ThenInclude(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
                return NotFound(new ResultViewModel<string>("4AC827EF - Registro não encontrado"));
            return Ok(post);
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("AE2E9912 - Falha interna no servidor"));
        }
    }

    [HttpGet("v1/posts/category/{category}")]
    public async Task<IActionResult> GetByCategoryIdAsync([FromServices] BlogDataContext ctx,
        [FromRoute] string category,
        [FromQuery] int page,
        [FromQuery] int pageSize = 25)
    {

        var count = await ctx.Posts.CountAsync();

        var posts = await ctx.Posts
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Author)
            .Where(x => x.Category.Slug == category)
            .Select(x => new ListPostsViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Slug = x.Slug,
                LastUpdateDate = x.LastUpdateDate,
                Category = x.Category.Name,
                Author = $"{x.Author.Name} - {x.Author.Email}"
            })
            .Skip(page * pageSize)
            .Take(pageSize)
            .OrderByDescending(x => x.LastUpdateDate)
            .ToListAsync();

        return Ok(new ResultViewModel<PaginateViewModel>(new PaginateViewModel
        {
            Total = count,
            NextPage = ++page,
            EndOfList = ((++page) * pageSize) >= count,
            PageSize = pageSize,
            Posts = posts
        }));
    }

}
