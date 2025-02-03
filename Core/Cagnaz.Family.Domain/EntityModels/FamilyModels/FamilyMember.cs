using Cagnaz.Family.Domain.Enums;
using MyBaseRepository.Models;

namespace Cagnaz.Family.Domain.EntityModels.FamilyModels;

public class FamilyMember:BaseModel
{
    public Guid FamilyID { get; set; }
    public FamilyModel Family { get; set; }

    public Guid? UserID { get; set; }
    
    public string MemberName { get; set; }
    
    public string MemberSurname { get; set; }
    
    public MemberStatus MemberStatus { get; set; }
    
    public DateTime? BirdDate { get; set; }
}