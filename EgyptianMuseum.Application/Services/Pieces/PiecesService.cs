using EgyptianMuseum.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EgyptianMuseum.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace EgyptianMuseum.Application.Services.Services
{
    public class PiecesService(
        IPiecesRepository<Pieces> repository) : IPiecesServices
    {
        public Task<Pieces> CreateAsync(Pieces entity)
            => repository.CreateAsync(entity);

        public Task<bool> DeleteAsync(int id)
            => repository.DeleteAsync(id);

        public async Task<IEnumerable<Pieces>> GetAllAsync()
            => await repository.GetAllAsync();

        public Task<Pieces> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => repository.GetByIdAsync(id);

        public Task<bool> UpdateAsync(Pieces entity)
            => repository.UpdateAsync(entity);

        public Task<Pieces> GetByCodeAsync(string code)
            => repository.GetFirstOrDefaultAsync(x => x.Code == code);

        public Task<IEnumerable<Pieces>> GetPagedAsync(int page, int pageSize)
            => repository.GetPagedAsync(page, pageSize);

        public Task<Pieces?> GetByCodeWithTranslationsAsync(string code, CancellationToken cancellationToken = default)
            => repository.GetByCodeWithTranslationsAsync(code);

        public Task<List<Pieces>> GetPagedWithTranslationsAsync(int page, int pageSize)
            => repository.GetPagedWithTranslationsAsync(page, pageSize);
    }
}
