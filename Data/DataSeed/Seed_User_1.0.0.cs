using JwtAuthDemo.Data;
using Microsoft.AspNetCore.Identity;

public class Seed_User_1 :IDataSeeder
{
    private readonly DemoAppDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    public Seed_User_1(DemoAppDbContext dbContex, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContex;
        _passwordHasher = passwordHasher;
    }

    public void Seed()
    {
        string className = GetType().Name;
        string [] fileName = className.Split('_');
        string entity = fileName[1];
        string version = fileName[2];

        if (_dbContext.SeederLogs.Any(x => x.EntityName == entity && x.Version == version))
            return;

        if (!_dbContext.Users.Any())
        {
            var user1 = new User
            {
                Name = "Akhilesh",
                Username = "akizone",
                Role = "Admin"
            };
            user1.Password = _passwordHasher.HashPassword(user1, "akizone");

            var user2 = new User
            {
                Name = "Muskan",
                Username = "mkizone",
                Role = "Admin"
            };
            user2.Password = _passwordHasher.HashPassword(user2, "mkizone");

            _dbContext.Users.AddRange(user1, user2);
            _dbContext.SeederLogs.Add(new SeederLog
            {
                EntityName = entity,
                Version = version,
                AppliedOn = DateTime.UtcNow,
            });

            Console.WriteLine($"User1: {user1.Username}, Password: {user1.Password}");
            Console.WriteLine($"User2: {user2.Username}, Password: {user2.Password}");

            _dbContext.SaveChanges();
        }
    }
}
