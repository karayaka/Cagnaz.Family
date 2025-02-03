using MyBaseRepository.Models;

namespace Cagnaz.Family.Domain.EntityModels.FamilyModels;

public class FamilyModel:BaseModel
{
    public string Name { get; set; }

    public ICollection<FamilyMember> Members { get; set; }
}