using Microsoft.EntityFrameworkCore.Storage;

namespace Vikalp.Service.Interfaces;

public interface IUnitOfWork : IDisposable
{


    IGenericRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync();
    // Task BeginTransactionAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();


}
