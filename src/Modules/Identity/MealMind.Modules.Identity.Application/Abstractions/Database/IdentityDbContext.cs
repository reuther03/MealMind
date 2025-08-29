using MealMind.Modules.Identity.Domain.IdentityUser;
using Microsoft.EntityFrameworkCore;


namespace MealMind.Modules.Identity.Application.Abstractions.Database;

public interface IIdentityDbContext
{
    DbSet<IdentityUser> IdentityUsers { get; }
}