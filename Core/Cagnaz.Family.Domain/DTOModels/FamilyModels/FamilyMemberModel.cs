using System.ComponentModel.DataAnnotations;
using Cagnaz.Family.Domain.Enums;

namespace Cagnaz.Family.Domain.DTOModels.FamilyModels;

public class FamilyMemberModel
{
    public Guid ID { get; set; }

    public Guid FamilyID { get; set; }
    
    public Guid? UserID { get; set; }
    [Required(ErrorMessage = "Ad zorulun alan")]
    public string MemberName { get; set; }
    [Required(ErrorMessage = "Soyad zorulun alan")]
    public string MemberSurname { get; set; }
    
    public MemberStatus MemberStatus { get; set; }
    
    public DateTime? BirdDate { get; set; }
}