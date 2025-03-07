using AuthService.Models;
using Cagnaz.Family.Application.Repositorys;
using Cagnaz.Family.Domain.DTOModels.FamilyModels;
using Cagnaz.Family.Domain.EntityModels.FamilyModels;
using Cagnaz.Family.Domain.Enums;
using Cagnaz.Family.Persistence.DataContexts;
using ExceptionHandling.Exceptions;
using Microsoft.EntityFrameworkCore;
using MQService.Services.Interfaces;
using MyBaseRepository.Concrete;
using MyBaseRepository.Enums;

namespace Cagnaz.Family.Infrastructure.Repositorys;

public class FamilyRepository:Repository,IFamilyRepository
{
    private readonly IUnitOfWork _uow;  
    private readonly SessionModel _session;

    private readonly IFamilyService _familyService;
    public FamilyRepository(MySqlFamilyDataContext context, SessionModel session,IFamilyService familyService,IUnitOfWork uow) : base(context, session.ID)
    {
        _uow = uow;
        _session = session;
        _familyService=familyService;
    }

    public async Task<Guid> AddFamily(CreateFamilyModel family)
    {
        try
        {
            if(AnyNonDeletedAndActive<FamilyMember>(t=>t.UserID==_session.ID))
                throw new CustomException("sadece bir aileye üye olabilirsinz");
            var fm = new FamilyModel()
            {
                Name = family.Name,
                CreatedBy = _session.ID,
                LastModifiedBy = _session.ID,
            };
            var member = new FamilyMember()
            {
                UserID = _session.ID,
                Family = fm,
                MemberName = _session.Name,
                MemberSurname = _session.Surname,
                MemberStatus = MemberStatus.UserMember,
                CreatedBy = _session.ID,
                LastModifiedBy = _session.ID,
            };
            await Add(member);
            await _uow.SaveChanges();
            var members= new List<MQService.Models.FamilyModels.FamilyMember>
            {
                new()
                {
                    ID = member.ID,
                    MemberName = member.MemberName,
                    MemberSurname = member.MemberSurname,
                    UserID = member.UserID,

                }
            };
            await _familyService.FamilyEvent(new(){
                ID=fm.ID,
                Token=_session.Token,
                FamilyID=fm.ID,
                ActionType=MQService.Consts.ActionType.Create,
                FamilyName=fm.Name,
                FamilyMembers=members
                
            });
            return fm.ID;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<FamilyListModel?> GetFamilyByUserID(Guid usrID)
    {
        try
        {
            return await GetNonDeletedAndActive<FamilyModel>(t => t.Members.Any(t => t.UserID == usrID&&t.ObjectStatus==ObjectStatus.NonDeleted)).Select(s =>
                new FamilyListModel()
                {
                    ID = s.ID,
                    Name = s.Name,
                    Members = s.Members.Select(m => new FamilyMemberModel()
                    {
                        ID = m.ID,
                        FamilyID = m.FamilyID,
                        MemberName = m.MemberName,
                        MemberSurname = m.MemberSurname,
                        UserID = m.UserID,
                        MemberStatus = m.MemberStatus,
                        BirdDate = m.BirdDate,
                    }).ToList(),
                }).FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task JoinFamily(JoinFamilyModel family)
    {
        try
        {
            if(AnyNonDeletedAndActive<FamilyMember>(t=>t.FamilyID==family.ID&&t.UserID==_session.ID))
                throw new CustomException("001");
            var member = new FamilyMember()
            {
                UserID = _session.ID,
                FamilyID = family.ID,
                MemberName = _session.Name,
                MemberSurname = _session.Surname,
                MemberStatus = MemberStatus.UserMember,
                CreatedBy = _session.ID,
                LastModifiedBy = _session.ID,
            };
            await Add(member);
            await _uow.SaveChanges();
            await AddFamilyToQueue(family.ID);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task LeaveFamily(Guid memberID)
    {
        try
        {
            var familyMember=await FindNonDeletedAndActive<FamilyMember>(t=>t.ID == memberID);
            if (familyMember == null)
                throw new CustomException("Aile bilgisi bulunamdı");
            if (!AnyNonDeletedAndActive<FamilyMember>(t =>
                    t.FamilyID == familyMember.FamilyID && t.MemberStatus == MemberStatus.UserMember&&t.ID!=memberID))
            {
                await DeleteRange<FamilyMember>(t=>t.FamilyID == familyMember.FamilyID);
                await DeleteRange<FamilyModel>(t=>t.ID == familyMember.FamilyID);
                await _familyService.FamilyEvent(new(){ID=familyMember.FamilyID,FamilyID=familyMember.FamilyID,ActionType=MQService.Consts.ActionType.Delete});
                
            }
            else
            {
                Delete(familyMember);
                await _uow.SaveChanges();
                await AddFamilyToQueue(familyMember.FamilyID);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task AddNotUserFamilyMember(FamilyMemberModel model)
    {
        try
        {

            await Add<FamilyMember>(new()
            {
                UserID = _session.ID,
                FamilyID = model.FamilyID,
                MemberName = model.MemberName,
                MemberSurname = model.MemberSurname,
                MemberStatus = MemberStatus.NonUserMember,
                BirdDate = model.BirdDate,
            });
            await _uow.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task UpdateFamilyMember(UpdateFamilyMemberModel model)
    {
        try
        {
            var member=await FindNonDeletedAndActive<FamilyMember>(t=>t.ID == model.ID);
            if(member==null)
                throw new CustomException("Aile üyesi bulunamadı");
            member.BirdDate = model.BirdDate;
            if (member.MemberStatus == MemberStatus.NonUserMember)
            {
                member.MemberName = model.MemberName??"";
                member.MemberSurname = model.MemberSurname??"";
            }

            Update(member);
            await _uow.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    private async Task AddFamilyToQueue(Guid familyID){
        try
        {
            var family= await GetNonDeletedAndActive<FamilyModel>(t=>t.ID==familyID).Select(s=>new MQService.Models.FamilyModels.FamilyEventModel(){
                ID=s.ID,
                FamilyName=s.Name,
                FamilyID=s.ID,
                ActionType=MQService.Consts.ActionType.Update,
                FamilyMembers=s.Members.Where(t=>t.ObjectStatus==ObjectStatus.NonDeleted).Select(fm=>new MQService.Models.FamilyModels.FamilyMember(){
                    ID=fm.ID,
                    MemberName=fm.MemberName,
                    MemberSurname=fm.MemberSurname,
                    UserID=fm.UserID,
                    BirdDate=fm.BirdDate
                }).ToList(),
            }).FirstOrDefaultAsync();
            await _familyService.FamilyEvent(family);
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task<FamilyDasbordModel?> GetFamilyDasbordModel()
    {
        try
        {
            return await GetNonDeletedAndActive<FamilyModel>(t=>t.Members.Any(t=>t.UserID==_session.ID)).Select(s=>new FamilyDasbordModel(){
                ID=s.ID,
                FamilyName=s.Name,
                MemberCount=s.Members.Where(t=>t.ObjectStatus==ObjectStatus.NonDeleted).Count()
            }).FirstOrDefaultAsync();
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }
}