using Microsoft.EntityFrameworkCore;
using Cagnaz.Family.Domain.EntityModels.FamilyModels;

namespace Cagnaz.Family.Persistence.DataContexts;

public class MySqlFamilyDataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<FamilyModel> Familyes { get; set; }
    
    public DbSet<FamilyMember> FamilyMembers { get; set; }
}