namespace Backend.DTOs;

public class LoginDto
{
    // 因为要与数据库中的数据进行比较，所以没有必要添加验证  
    public string Email { get; set; } = "";  
    public string Password { get; set; } = ""; 
}