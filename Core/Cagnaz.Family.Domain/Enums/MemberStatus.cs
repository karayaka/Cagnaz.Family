using System.ComponentModel;

namespace Cagnaz.Family.Domain.Enums;

public enum MemberStatus
{
    /// <summary>
    /// Kullanıcsı bulunan aile üyesş
    /// </summary>
    UserMember=0,
    /// <summary>
    /// Kullanıcısı olmayan aile üyesi (ailenin bebeği gibi)
    /// </summary>
    NonUserMember=1,
}