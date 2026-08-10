using Microsoft.EntityFrameworkCore;

namespace EvoManage.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{

}

