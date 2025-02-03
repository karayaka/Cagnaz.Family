using AuthService.Interfaces;
using AuthService.Models;
using Cagnaz.Family.Application.Repositorys;
using Cagnaz.Family.Persistence.DataContexts;
using MyBaseRepository.Concrete;

namespace Cagnaz.Family.Infrastructure.Repositorys;

public class UnitOfWork:BaseUnitOfWork,IUnitOfWork
{
    private readonly MySqlFamilyDataContext _context;
    private readonly SessionModel _session;
    
    public UnitOfWork(MySqlFamilyDataContext context,IIdentityService identityService) : base(context)
    {
        _context = context;
        _session = identityService.GetSesion();
    }

    private IFamilyRepository _familyRepository { get; set; }
    public IFamilyRepository FamilyRepository => _familyRepository ??= new FamilyRepository(_context,_session,this);
}