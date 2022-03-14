using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Text.RegularExpressions;

namespace Blog.Controllers;

[ApiController]
public class AccountController : ControllerBase
{

    [HttpPost("v1/accounts")]
    public async Task<IActionResult> Post([FromServices] TokenService tokenService,
        [FromServices] BlogDataContext ctx, [FromBody] RegisterViewModel model,
        [FromServices] EmailService emailService)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var password = PasswordGenerator.Generate(25, includeSpecialChars: true, upperCase: false);

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email.Replace("@", "-").Replace(".", "-"),
            PasswordHash = PasswordHasher.Hash(password)
        };

        try
        {
            await ctx.Users.AddAsync(user);
            ctx.SaveChanges();

            if (Configuration.Smtp.SendPasswordEmail)
                emailService.Send(
                    user.Name,
                    user.Email,
                    "Bem vindo ao nosso blog",
                    $"Seu password é <strong>{password}</strong>"
                  );


            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email,
                password // In a real situation it would be sent just on the email, it returns here for testing
            }));
        }
        catch (DbUpdateException)
        {
            return StatusCode(400, new ResultViewModel<string>("5F64BACA - E-mail já cadastrado"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("0BD16727 - Falha interna no servidor"));
        }
    }

    [HttpPost("v1/accounts/login")]
    public async Task<IActionResult> Login([FromServices] TokenService tokenService,
        [FromServices] BlogDataContext ctx, [FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user =
            await ctx.Users
                .AsNoTracking()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Email.ToLower() == model.Email.ToLower());

        if (user == null)
            return StatusCode(401, new ResultViewModel<string>("3374BDF2 - Usuário ou senha inválidos"));

        if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return StatusCode(401, new ResultViewModel<string>("3374BDF2 - Usuário ou senha inválidos"));

        try
        {
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token, null));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("8C50406F - Erro interno no servidor"));
        }
    }

    [Authorize]
    [HttpPost("v1/accounts/upload-image")]
    public async Task<IActionResult> UploadImage([FromBody] UploadImageViewModel model, [FromServices] BlogDataContext ctx)
    {
        var fileName = $"{Guid.NewGuid().ToString()}.jpg";
        var data = new Regex(@"^data:image\/[a-z]+;base64,")
            .Replace(model.Base64Image, "");
        var bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);

            var user = await ctx.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if(user == null)
                return NotFound(new ResultViewModel<string>("F18EEB2A - Usuário não encontrado"));

            user.Image = $"{Configuration.LinkImagesPath}{fileName}";
            await ctx.SaveChangesAsync();

            return Ok(new ResultViewModel<string>("Imagem alterada com sucesso", null));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("339EB581 - Falha interna no servidor"));
        }

    }

}
