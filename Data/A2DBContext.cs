using Microsoft.EntityFrameworkCore;

namespace A2.data;

public class A2DBContext : DbContext
{
    public A2DBContext(DbContextOptions<A2DBContext> options) : base(options) {}
}