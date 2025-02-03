using MyBaseRepository.Abstract;

namespace Cagnaz.Family.Application.Repositorys;

public interface IUnitOfWork:IBaseUnitOfWork
{
    public IFamilyRepository FamilyRepository { get; }
}