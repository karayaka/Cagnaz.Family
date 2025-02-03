namespace Cagnaz.Family.Domain.DTOModels.FamilyModels;

public class FamilyListModel
{
    public Guid ID { get; set; }

    public string Name { get; set; }

    public List<FamilyMemberModel> Members { get; set; }
}