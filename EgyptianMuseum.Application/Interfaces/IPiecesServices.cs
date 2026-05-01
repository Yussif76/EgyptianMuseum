using EgyptianMuseum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IPiecesServices
    {

        Task<IEnumerable<Pieces>> GetAllAsync();
        Task<Pieces> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Pieces> GetByCodeAsync(string code);
        Task<Pieces> CreateAsync(Pieces entity);
        Task<IEnumerable<Pieces>> GetPagedAsync(int page, int pageSize);
        Task<bool> UpdateAsync(Pieces entity);
        Task<bool> DeleteAsync(int id);
        Task<Pieces?> GetByCodeWithTranslationsAsync(string code, CancellationToken cancellationToken = default);

        Task<List<Pieces>> GetPagedWithTranslationsAsync(int page, int pageSize);
    }
}
