using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Models;
using System.Security.Cryptography;
using System.Text;

namespace irevlogix_backend.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            try
            {
                await context.Database.EnsureCreatedAsync();

                if (await context.Users.AnyAsync(u => u.Email == "admin@irevlogix.ai"))
                {
                    Console.WriteLine("DataSeeder: Admin user already exists, skipping seeding.");
                    return; // Already seeded
                }

                Console.WriteLine("DataSeeder: Starting admin user and permissions seeding...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataSeeder: Error during initial setup - {ex.Message}");
                throw;
            }

            var clientId = "ADMIN_CLIENT_001";

            Role adminRole;
            Role projectManagerRole;
            Role logisticsAnalystRole;
            Role logisticsManagerRole;
            try
            {
                adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Administrator");
                
                if (adminRole == null)
                {
                    adminRole = new Role
                    {
                        Name = "Administrator",
                        Description = "System Administrator with full access to all modules",
                        IsSystemRole = true,
                        ClientId = clientId,
                        CreatedBy = 1,
                        UpdatedBy = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    };

                    context.Roles.Add(adminRole);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"DataSeeder: Created Administrator role with ID {adminRole.Id}");
                }
                else
                {
                    Console.WriteLine($"DataSeeder: Using existing Administrator role with ID {adminRole.Id}");
                }

                projectManagerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Project Manager");
                
                if (projectManagerRole == null)
                {
                    projectManagerRole = new Role
                    {
                        Name = "Project Manager",
                        Description = "Project Manager with access to project management, reverse logistics, processing, and reporting modules",
                        IsSystemRole = false,
                        ClientId = clientId,
                        CreatedBy = 1,
                        UpdatedBy = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    };

                    context.Roles.Add(projectManagerRole);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"DataSeeder: Created Project Manager role with ID {projectManagerRole.Id}");
                }
                else
                {
                    Console.WriteLine($"DataSeeder: Using existing Project Manager role with ID {projectManagerRole.Id}");
                }

                logisticsAnalystRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Logistics Analyst");
                
                if (logisticsAnalystRole == null)
                {
                    logisticsAnalystRole = new Role
                    {
                        Name = "Logistics Analyst",
                        Description = "Logistics Analyst with access to project management, reverse logistics, processing, and reporting modules",
                        IsSystemRole = false,
                        ClientId = clientId,
                        CreatedBy = 1,
                        UpdatedBy = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    };

                    context.Roles.Add(logisticsAnalystRole);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"DataSeeder: Created Logistics Analyst role with ID {logisticsAnalystRole.Id}");
                }
                else
                {
                    Console.WriteLine($"DataSeeder: Using existing Logistics Analyst role with ID {logisticsAnalystRole.Id}");
                }

                logisticsManagerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Logistics Manager");
                
                if (logisticsManagerRole == null)
                {
                    logisticsManagerRole = new Role
                    {
                        Name = "Logistics Manager",
                        Description = "Logistics Manager with access to project management, reverse logistics, processing, and reporting modules",
                        IsSystemRole = false,
                        ClientId = clientId,
                        CreatedBy = 1,
                        UpdatedBy = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    };

                    context.Roles.Add(logisticsManagerRole);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"DataSeeder: Created Logistics Manager role with ID {logisticsManagerRole.Id}");
                }
                else
                {
                    Console.WriteLine($"DataSeeder: Using existing Logistics Manager role with ID {logisticsManagerRole.Id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataSeeder: Error handling roles - {ex.Message}");
                throw;
            }

            var permissions = new List<Permission>
            {
                new Permission { Name = "Reverse Logistics Read", Module = "ReverseLogistics", Action = "Read", Description = "View reverse logistics data", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Reverse Logistics Create", Module = "ReverseLogistics", Action = "Create", Description = "Create reverse logistics entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Reverse Logistics Update", Module = "ReverseLogistics", Action = "Update", Description = "Update reverse logistics entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Reverse Logistics Delete", Module = "ReverseLogistics", Action = "Delete", Description = "Delete reverse logistics entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                
                new Permission { Name = "Processing Read", Module = "Processing", Action = "Read", Description = "View processing data", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Processing Create", Module = "Processing", Action = "Create", Description = "Create processing entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Processing Update", Module = "Processing", Action = "Update", Description = "Update processing entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Processing Delete", Module = "Processing", Action = "Delete", Description = "Delete processing entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                
                new Permission { Name = "Downstream Materials Read", Module = "DownstreamMaterials", Action = "Read", Description = "View downstream materials data", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Downstream Materials Create", Module = "DownstreamMaterials", Action = "Create", Description = "Create downstream materials entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Downstream Materials Update", Module = "DownstreamMaterials", Action = "Update", Description = "Update downstream materials entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Downstream Materials Delete", Module = "DownstreamMaterials", Action = "Delete", Description = "Delete downstream materials entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                
                new Permission { Name = "Asset Recovery Read", Module = "AssetRecovery", Action = "Read", Description = "View asset recovery data", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Asset Recovery Create", Module = "AssetRecovery", Action = "Create", Description = "Create asset recovery entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Asset Recovery Update", Module = "AssetRecovery", Action = "Update", Description = "Update asset recovery entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Asset Recovery Delete", Module = "AssetRecovery", Action = "Delete", Description = "Delete asset recovery entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                
                new Permission { Name = "Reporting Read", Module = "Reporting", Action = "Read", Description = "View reports", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Reporting Create", Module = "Reporting", Action = "Create", Description = "Create reports", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Reporting Update", Module = "Reporting", Action = "Update", Description = "Update reports", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Reporting Delete", Module = "Reporting", Action = "Delete", Description = "Delete reports", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                
                new Permission { Name = "Administration Read", Module = "Administration", Action = "Read", Description = "View administration settings", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Administration Create", Module = "Administration", Action = "Create", Description = "Create administration entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Administration Update", Module = "Administration", Action = "Update", Description = "Update administration entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Administration Delete", Module = "Administration", Action = "Delete", Description = "Delete administration entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                
                new Permission { Name = "Knowledge Base Read", Module = "KnowledgeBase", Action = "Read", Description = "View knowledge base articles", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Knowledge Base Create", Module = "KnowledgeBase", Action = "Create", Description = "Create knowledge base articles", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Knowledge Base Update", Module = "KnowledgeBase", Action = "Update", Description = "Update knowledge base articles", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Knowledge Base Delete", Module = "KnowledgeBase", Action = "Delete", Description = "Delete knowledge base articles", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                
                new Permission { Name = "Training Read", Module = "Training", Action = "Read", Description = "View training materials", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Training Create", Module = "Training", Action = "Create", Description = "Create training materials", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Training Update", Module = "Training", Action = "Update", Description = "Update training materials", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Training Delete", Module = "Training", Action = "Delete", Description = "Delete training materials", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                
                new Permission { Name = "Project Management Read", Module = "ProjectManagement", Action = "Read", Description = "View project management data", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Project Management Create", Module = "ProjectManagement", Action = "Create", Description = "Create project management entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Project Management Update", Module = "ProjectManagement", Action = "Update", Description = "Update project management entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 },
                new Permission { Name = "Project Management Delete", Module = "ProjectManagement", Action = "Delete", Description = "Delete project management entries", ClientId = clientId, CreatedBy = 1, UpdatedBy = 1 }
            };

            foreach (var permission in permissions)
            {
                permission.DateCreated = DateTime.UtcNow;
                permission.DateUpdated = DateTime.UtcNow;
            }

            try
            {
                var existingPermissionNames = await context.Permissions
                    .Select(p => p.Name)
                    .ToListAsync();
                
                var newPermissions = permissions
                    .Where(p => !existingPermissionNames.Contains(p.Name))
                    .ToList();
                
                if (newPermissions.Any())
                {
                    context.Permissions.AddRange(newPermissions);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"DataSeeder: Created {newPermissions.Count} new permissions");
                }
                else
                {
                    Console.WriteLine("DataSeeder: All permissions already exist");
                }
                
                var allPermissions = await context.Permissions.ToListAsync();
                permissions = allPermissions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataSeeder: Error creating permissions - {ex.Message}");
                throw;
            }

            try
            {
                var existingAdminRolePermissions = await context.RolePermissions
                    .Where(rp => rp.RoleId == adminRole.Id)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();
                
                var newAdminRolePermissions = permissions
                    .Where(p => !existingAdminRolePermissions.Contains(p.Id))
                    .Select(p => new RolePermission
                    {
                        RoleId = adminRole.Id,
                        PermissionId = p.Id,
                        ClientId = clientId,
                        CreatedBy = 1,
                        UpdatedBy = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    }).ToList();
                
                if (newAdminRolePermissions.Any())
                {
                    context.RolePermissions.AddRange(newAdminRolePermissions);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"DataSeeder: Created {newAdminRolePermissions.Count} new admin role permissions");
                }
                else
                {
                    Console.WriteLine("DataSeeder: All admin role permissions already exist");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataSeeder: Error creating admin role permissions - {ex.Message}");
                throw;
            }

            try
            {
                var projectManagerPermissions = permissions.Where(p => 
                    p.Module != "Administration" &&
                    p.Module != "Authentication"
                ).ToList();

                var existingPMRolePermissions = await context.RolePermissions
                    .Where(rp => rp.RoleId == projectManagerRole.Id)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();
                
                var newPMRolePermissions = projectManagerPermissions
                    .Where(p => !existingPMRolePermissions.Contains(p.Id))
                    .Select(p => new RolePermission
                    {
                        RoleId = projectManagerRole.Id,
                        PermissionId = p.Id,
                        ClientId = clientId,
                        CreatedBy = 1,
                        UpdatedBy = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    }).ToList();
                
                if (newPMRolePermissions.Any())
                {
                    context.RolePermissions.AddRange(newPMRolePermissions);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"DataSeeder: Created {newPMRolePermissions.Count} new project manager role permissions");
                }
                else
                {
                    Console.WriteLine("DataSeeder: All project manager role permissions already exist");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataSeeder: Error creating project manager role permissions - {ex.Message}");
                throw;
            }

            try
            {
                var logisticsAnalystPermissions = permissions.Where(p => 
                    p.Module != "Administration" &&
                    p.Module != "Authentication"
                ).ToList();

                var existingLARolePermissions = await context.RolePermissions
                    .Where(rp => rp.RoleId == logisticsAnalystRole.Id)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();
                
                var newLARolePermissions = logisticsAnalystPermissions
                    .Where(p => !existingLARolePermissions.Contains(p.Id))
                    .Select(p => new RolePermission
                    {
                        RoleId = logisticsAnalystRole.Id,
                        PermissionId = p.Id,
                        ClientId = clientId,
                        CreatedBy = 1,
                        UpdatedBy = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    }).ToList();
                
                if (newLARolePermissions.Any())
                {
                    context.RolePermissions.AddRange(newLARolePermissions);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"DataSeeder: Created {newLARolePermissions.Count} new logistics analyst role permissions");
                }
                else
                {
                    Console.WriteLine("DataSeeder: All logistics analyst role permissions already exist");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataSeeder: Error creating logistics analyst role permissions - {ex.Message}");
                throw;
            }

            try
            {
                var logisticsManagerPermissions = permissions.Where(p => 
                    p.Module != "Administration" &&
                    p.Module != "Authentication"
                ).ToList();

                var existingLMRolePermissions = await context.RolePermissions
                    .Where(rp => rp.RoleId == logisticsManagerRole.Id)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();
                
                var newLMRolePermissions = logisticsManagerPermissions
                    .Where(p => !existingLMRolePermissions.Contains(p.Id))
                    .Select(p => new RolePermission
                    {
                        RoleId = logisticsManagerRole.Id,
                        PermissionId = p.Id,
                        ClientId = clientId,
                        CreatedBy = 1,
                        UpdatedBy = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    }).ToList();
                
                if (newLMRolePermissions.Any())
                {
                    context.RolePermissions.AddRange(newLMRolePermissions);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"DataSeeder: Created {newLMRolePermissions.Count} new logistics manager role permissions");
                }
                else
                {
                    Console.WriteLine("DataSeeder: All logistics manager role permissions already exist");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataSeeder: Error creating logistics manager role permissions - {ex.Message}");
                throw;
            }

            User adminUser;
            try
            {
                adminUser = new User
                {
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "admin@irevlogix.ai",
                    PasswordHash = HashPassword("AdminPass123!"),
                    ClientId = clientId,
                    CreatedBy = 1,
                    UpdatedBy = 1,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    IsActive = true,
                    FailedLoginAttempts = 0,
                    IsEmailConfirmed = true
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
                Console.WriteLine($"DataSeeder: Created admin user with ID {adminUser.Id} and email {adminUser.Email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataSeeder: Error creating admin user - {ex.Message}");
                throw;
            }

            try
            {
                var userRole = new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    ClientId = clientId,
                    CreatedBy = 1,
                    UpdatedBy = 1,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };

                context.UserRoles.Add(userRole);
                await context.SaveChangesAsync();
                Console.WriteLine($"DataSeeder: Assigned Administrator role to admin user");
                Console.WriteLine("DataSeeder: Admin user and permissions seeding completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataSeeder: Error creating user role assignment - {ex.Message}");
                throw;
            }
        }

        private static string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);
            
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);
            
            var hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);
            
            return Convert.ToBase64String(hashBytes);
        }
    }
}
