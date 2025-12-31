using System.Security.Cryptography;
using System.Text;
using Backend.Data;
using Backend.DTOs;
using Backend.Entities;
using Backend.Extensions;
using Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

public class AccountController(AppDbContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]  
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)  
    {
        if (await EmailExists(registerDto.Email))  
        {
            return BadRequest("Email already exists");  
        }
        using var hmac = new HMACSHA512();  
        var user = new AppUser  
        {  
            Email = registerDto.Email,  
            DisplayName = registerDto.DisplayName,  
            PassWordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),  
            PassWordSalt = hmac.Key  
        };  
        context.Users.Add(user);  
        await context.SaveChangesAsync();  
        return user.ToDto(tokenService);  
    }  
    [HttpPost("login")]  
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)  
    {        // 检查Emain：Single 单一的  
        var user = await context.Users.SingleOrDefaultAsync(x => x.Email == loginDto.Email);  
        if (user == null) return Unauthorized("无效的 Emain 地址");    // 返回401 未授权  
        // 检查密码  
        using var hmac = new HMACSHA512(user.PassWordSalt);  
        var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));  
        for (var i = 0; i < computeHash.Length; i++)  
        {
            if (computeHash[i] != user.PassWordHash[i]) return Unauthorized("密码错误");  
        }
        return user.ToDto(tokenService);  
    }
    // 检查电子邮件是否已存在  
    private async Task<bool> EmailExists(string email)  
    {
        return await context.Users.AnyAsync(  
            x=>x.Email.ToLower() == email.ToLower());  
    }
}