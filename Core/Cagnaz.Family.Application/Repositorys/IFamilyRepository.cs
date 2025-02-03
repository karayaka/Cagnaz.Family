using Cagnaz.Family.Domain.DTOModels.FamilyModels;
using MyBaseRepository.Abstract;

namespace Cagnaz.Family.Application.Repositorys;

public interface IFamilyRepository:IRepository
{
    //Aile ekle aileden ayrıl aile üyelerinden kime kalmadı ise aile kaydını da sil aileye katıl aileye katıl eventi bir kişi sadece bir aile de olabilir! 
    //grps methodları kullanıcı ıdsinden aile bilgilerini dönen bir endpoint yazılacak
    //Cocuk ekleme kullanıcı aolmayan aile üyesi ekleme methodu
    //aile bilgilerini aile id den dönen bir endpointe yzılaca
    /// <summary>
    /// Aile ekleyene method
    /// </summary>
    /// <param name="family"></param>
    /// <returns></returns>
    Task<Guid> AddFamily(CreateFamilyModel family);
    /// <summary>
    /// kullanıcının aile bilgilerini dönen method
    /// </summary>
    /// <param name="usrID"></param>
    /// <returns></returns>
    Task<FamilyListModel?> GetFamilyByUserID(Guid usrID);
    /// <summary>
    /// bir kişinin familyID ile aile katılmasını sağlayan method bu kişinin bir ailesi varsa dahl olamaz önce diğer aileden çıkmalı
    /// </summary>
    /// <param name="family"></param>
    /// <returns></returns>
    Task JoinFamily(JoinFamilyModel family);
    /// <summary>
    /// kişi ayrıla bilir yada başka bir aile üyesi onu atabilir
    /// eğer ailede başka kimse kalmad ise aile kaydı yok edilir
    /// </summary>
    /// <param name="usrID"></param>
    /// <returns></returns>
    Task LeaveFamily(Guid memberID);
    /// <summary>
    /// aileye kullanıcı olamayan aile üyesi ekleme(ailenin cocuğu gibi)
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task AddNotUserFamilyMember(FamilyMemberModel model);
    /// <summary>
    /// Aile üyesini günceleler
    /// Aile üyesi user ise sadece doğum tarihi güncellenir aile üyesi kullanıcı değilse bütün bilgiler güncellenir
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task UpdateFamilyMember(UpdateFamilyMemberModel model);
}