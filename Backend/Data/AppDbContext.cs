using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class AppDbContext(DbContextOptions options):DbContext(options)
{
    // AppUser 实体类  Users 表名
    public DbSet<AppUser> Users { get; set; }
}