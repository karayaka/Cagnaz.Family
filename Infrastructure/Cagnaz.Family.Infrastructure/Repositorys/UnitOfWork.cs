using AuthService.Interfaces;
using AuthService.Models;
using Cagnaz.Family.Application.Repositorys;
using Cagnaz.Family.Persistence.DataContexts;
using MQService.Services.Interfaces;
using MyBaseRepository.Concrete;

namespace Cagnaz.Family.Infrastructure.Repositorys;

public class UnitOfWork:BaseUnitOfWork,IUnitOfWork
{
    private readonly MySqlFamilyDataContext _context;
    private readonly SessionModel _session;

    private readonly IFamilyService _familyService;
    
    public UnitOfWork(MySqlFamilyDataContext context,IIdentityService identityService,IFamilyService familyService) : base(context)
    {
        _context = context;
        _session = identityService.GetSesion();
        _familyService=familyService;
    }

    private IFamilyRepository _familyRepository { get; set; }
    public IFamilyRepository FamilyRepository => _familyRepository ??= new FamilyRepository(_context,_session,_familyService,this);
}