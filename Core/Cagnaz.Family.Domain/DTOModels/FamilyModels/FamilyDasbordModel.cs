using System;

namespace Cagnaz.Family.Domain.DTOModels.FamilyModels;

public class FamilyDasbordModel
{
    public Guid ID { get; set; }

    public string FamilyName { get; set; }

    public int MemberCount { get; set; }
}
