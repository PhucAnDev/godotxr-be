using GodotXR.Domain.Entities;
using GodotXR.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GodotXR.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
              new Role { Id = 1, RoleName = UserRole.Admin, Description = "System Administrator", IsActive = true },
              new Role { Id = 2, RoleName = UserRole.Teacher, Description = "Teacher", IsActive = true },
              new Role { Id = 3, RoleName = UserRole.Parent, Description = "Parent", IsActive = true }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    PasswordHash = "$2a$12$fm/KNqTHvTPZ.Xp17lMgpO7C5oJzF9lUi9niXMOX5mriX6xG9VPdu",
                    FullName = "System Admin",
                    Email = "admin@godotxr.com",
                    Phone = "0123456789",
                    RoleId = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    PasswordHash = "$2a$12$fm/KNqTHvTPZ.Xp17lMgpO7C5oJzF9lUi9niXMOX5mriX6xG9VPdu",
                    FullName = "Doan Manh Hiep",
                    Email = "teacher@godotxr.com",
                    Phone = "0987654321",
                    RoleId = 2,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}