using EgyptianMuseum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IPiecesRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<Pieces?> GetByCodeWithTranslationsAsync(string code, CancellationToken cancellationToken = default);
        Task<List<Pieces>> GetPagedWithTranslationsAsync(int page, int pageSize);
    }
}
