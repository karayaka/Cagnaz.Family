using System;

namespace Cagnaz.Family.Domain.DTOModels.FamilyModels;

public class UpdateFamilyMemberModel
{
    public Guid ID { get; set; }

    public string? MemberName { get; set; }
    public string? MemberSurname { get; set; }
    
    public DateTime? BirdDate { get; set; }
}
