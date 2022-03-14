using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

ConfigureAuthentication();
ConfigureMvc();
ConfigureServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
AppUses();
LoadConfiguration();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

void LoadConfiguration()
{
    Configuration.ApiKeyName = app.Configuration.GetValue<string>(key: "ApiKeyName");
    Configuration.ApiKey = app.Configuration.GetValue<string>(key: "ApiKey");
    var smpt = new Configuration.SmtpConfiguration();
    app.Configuration.GetSection(key: "Smtp").Bind(smpt);
    Configuration.Smtp = smpt;
    Configuration.LinkImagesPath = app.Configuration.GetValue<string>(key: "SaveImagesPath");
}

void ConfigureAuthentication()
{
    var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

    builder.Services.AddAuthentication(configureOptions: x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
}

void ConfigureMvc()
{
    builder.Services.AddMemoryCache();

    // In case of using response compression
    // builder.Services.AddResponseCompression(opt =>
    // {
    //     opt.Providers.Add<GzipCompressionProvider>();
    // });
    // builder.Services.Configure<GzipCompressionProviderOptions>(opt =>
    // {
    //     opt.Level = CompressionLevel.Optimal;
    // });

    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
            options.SuppressModelStateInvalidFilter = true)
        .AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            // x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        });
}

void ConfigureServices()
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); 
    
    builder.Services.AddDbContext<BlogDataContext>(opt => opt.UseSqlServer(connectionString));
    builder.Services.AddTransient<TokenService>();
    builder.Services.AddTransient<EmailService>();
    
    // AddTransient - It aways create a new instance
    // AddScoped    - It aways create a new instance for a new request
    // AddSingleton - It creates one instance by App
}

void AppUses()
{
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    // In case of using response compression
    // app.UseResponseCompression();
    app.UseStaticFiles();
    app.MapControllers();
}

// dotnet build -c Release
// dotnet add package Swashbuckle.AspNetCore