using Microsoft.EntityFrameworkCore;

namespace TestStore.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Good> Goods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Position> Positions { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            User testUser = new User
            {
                UserId = 1,
                Initials = "Тестовый пользователь",
                Login = "Login",
                Password = "Password"
            };

            Good testGood = new Good
            {
                GoodId = 1,
                Price = 10.25m,
                Description = "Тестовый товар"
            };

            modelBuilder.Entity<User>().HasData(new User[] { testUser });
            modelBuilder.Entity<Good>().HasData(new Good[] { testGood });

            base.OnModelCreating(modelBuilder);
        }
    }
}

