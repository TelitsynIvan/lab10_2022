namespace ConsoleApp1;
using Microsoft.EntityFrameworkCore;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public ApplicationContext() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //SQLitePCL.raw.SetProvider();
        optionsBuilder.UseSqlite("Data Source = ../../../medb.db");
    }
}

class Program
{
    static void Main()
    {
        using (ApplicationContext db = new ApplicationContext())
        {
          User tom = new User { name = "Tom", Age = 10 };
          User bob = new User { name = "bob", Age = 5 };

          db.Users.Add(tom);
          db.Users.Add(bob);
          db.SaveChanges();
          var users = db.Users.ToList();
          Console.WriteLine("Список объектов:");
          foreach (User u in users)
          {
              Console.WriteLine($"{u.id}.{u.name} - {u.Age}");
          }
        }
        
        
    }
}

