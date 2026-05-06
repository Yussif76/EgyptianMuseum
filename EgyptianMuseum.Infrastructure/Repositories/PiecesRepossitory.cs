using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EgyptianMuseum.Infrastructure.Data;
using EgyptianMuseum.Infrastructure.Helpers;


namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class PiecesRepository<T> : IPiecesRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _dbContext;

        public PiecesRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<T> CreateAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;

        }
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbContext.Set<T>().ToListAsync();

        public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _dbContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id);


        public async Task<bool> UpdateAsync(T entity)
        {
            var model = await GetByIdAsync(entity.Id);
            if (model == null)
                return false;
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return true;

        }
        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }
        public async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
        {
            var query = _dbContext.Set<T>().AsQueryable();

            query = PaginationHelper.ApplyPagination(query, page, pageSize);

            return await query.ToListAsync();
        }
        public async Task<Pieces?> GetByCodeWithTranslationsAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _dbContext.pieces
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<List<Pieces>> GetPagedWithTranslationsAsync(int page, int pageSize)
        {
            return await _dbContext.pieces
                .Include(x => x.Translations)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
