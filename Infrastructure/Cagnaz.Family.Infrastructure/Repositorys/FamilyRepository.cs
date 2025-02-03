using AuthService.Models;
using Cagnaz.Family.Application.Repositorys;
using Cagnaz.Family.Domain.DTOModels.FamilyModels;
using Cagnaz.Family.Domain.EntityModels.FamilyModels;
using Cagnaz.Family.Domain.Enums;
using Cagnaz.Family.Persistence.DataContexts;
using ExceptionHandling.Exceptions;
using Microsoft.EntityFrameworkCore;
using MyBaseRepository.Concrete;

namespace Cagnaz.Family.Infrastructure.Repositorys;

public class FamilyRepository:Repository,IFamilyRepository
{
    private readonly IUnitOfWork _uow;  
    private readonly SessionModel _session;
    public FamilyRepository(MySqlFamilyDataContext context, SessionModel session,IUnitOfWork uow) : base(context, session.ID)
    {
        _uow = uow;
        _session = session; 
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
            return await GetNonDeletedAndActive<FamilyModel>(t => t.Members.Any(t => t.UserID == usrID)).Select(s =>
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
                    t.FamilyID == familyMember.FamilyID && t.MemberStatus == MemberStatus.UserMember))
            {
                await DeleteRange<FamilyMember>(t=>t.FamilyID == familyMember.FamilyID);
                await DeleteRange<FamilyModel>(t=>t.ID == familyMember.FamilyID);
            }
            else
            {
                Delete(familyMember);
                await _uow.SaveChanges();
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
}