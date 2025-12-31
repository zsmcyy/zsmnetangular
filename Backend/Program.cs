using System.Text;
using Backend.Data;
using Backend.Interfaces;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// 将创建的 AppDbContext 类添加到 DI 容器中 
// GetConnectionString 从指定的配置中获取指定的连接字符串
builder.Services.AddDbContext<AppDbContext>(opt=>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();    // 跨域
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"]
                       ?? throw new Exception("没有发现 Token - Program.cs");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// ====================================================== //
var app = builder.Build();
// Configure the HTTP request pipeline.
// 使用跨域
app.UseCors(x =>
{
    // 允许任何请求头，允许任何请求方法，指定允许的来源
    x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200");
});

app.UseAuthentication();    //回答的问题是”你是谁？“
app.UseAuthorization();     //知道”你是谁“后 判断”你是否有权限做这件事“

app.MapControllers();

app.Run();
