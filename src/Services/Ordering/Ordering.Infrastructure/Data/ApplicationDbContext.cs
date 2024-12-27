using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Models;
using Ordering.Application.Data;
using System.Reflection;

namespace Ordering.Infrastructure.Data;
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
//Migration command  (open PMConsole and move to infra project)
//Add-Migration InitialCreate -OutputDir Data/Migration -Project Ordering.Infrastructure -StartupProject Ordering.API
}
