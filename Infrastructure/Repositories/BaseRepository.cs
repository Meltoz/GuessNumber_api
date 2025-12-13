using Application.Interfaces.Repository;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;
using AutoMapper;
using Application.Exceptions;

namespace Infrastructure.Repositories
{
    public class BaseRepository<TDomain, TEntity> : IRepository<TDomain>
        where TEntity : BaseEntity, new()
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly IMapper _mapper;

        public BaseRepository(DbContext context, IMapper mapper)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _mapper = mapper;
        }

        // ------------------------------
        // 🔹 GET ALL
        // ------------------------------
        public virtual async Task<IEnumerable<TDomain>> GetAllAsync()
        {
            var entities = await _dbSet.ToListAsync();
            return _mapper.Map<IEnumerable<TDomain>>(entities);
        }

        // ------------------------------
        // 🔹 PAGINATION
        // ------------------------------
        public virtual async Task<PagedResult<TDomain>> GetPaginateAsync(IQueryable<TEntity> query, int skip, int take)
        {
            var total = await query.CountAsync();
            var entities = await query.Skip(skip).Take(take).ToListAsync();


            return new PagedResult<TDomain>
            {
                Data = _mapper.Map<IEnumerable<TDomain>>(entities),
                TotalCount = total
            };
        }

        // ------------------------------
        // 🔹 GET BY ID
        // ------------------------------
        public virtual async Task<TDomain?> GetByIdAsync(Guid id)
        {
            var entity = await _dbSet.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<TDomain?>(entity);
        }

        public virtual TDomain? GetById(Guid id)
        {
            var entity = _dbSet.SingleOrDefault(x => x.Id == id);
            return _mapper.Map<TDomain?>(entity);
        }

        // ------------------------------
        // 🔹 INSERT
        // ------------------------------
        public virtual async Task<TDomain> InsertAsync(TDomain domain)
        {
            var entity = _mapper.Map<TEntity>(domain);
            entity.Created = entity.Updated = DateTime.UtcNow;

            _dbSet.Add(entity);
            await SaveAsync();

            return _mapper.Map<TDomain>(entity);
        }

        public virtual TDomain? Insert(TDomain domain)
        {
            var entity = _mapper.Map<TEntity>(domain);
            entity.Created = entity.Updated = DateTime.UtcNow;

            _dbSet.Add(entity);
            Save();

            return _mapper.Map<TDomain>(entity);
        }

        public virtual async Task InsertWithOutSave(TDomain domain)
        {
            var entity = _mapper.Map<TEntity>(domain);
            entity.Created = entity.Updated = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);
        }

        // ------------------------------
        // 🔹 UPDATE
        // ------------------------------
        public virtual async Task<TDomain> UpdateAsync(TDomain domainToUpdate)
        {
            var entityToUpdate = _mapper.Map<TEntity>(domainToUpdate);
            var existingEntity = await _dbSet.FindAsync(entityToUpdate.Id);

            if (existingEntity == null)
                throw new EntityNotFoundException();

            _context.Entry(existingEntity).CurrentValues.SetValues(entityToUpdate);
            existingEntity.Updated = DateTime.UtcNow;

            await SaveAsync();

            return _mapper.Map<TDomain>(existingEntity);
        }

        public virtual async Task UpdateWithOutSaveAsync(TDomain domainToUpdate)
        {
            var entityToUpdate = _mapper.Map<TEntity>(domainToUpdate);
            var existingEntity = await _dbSet.FindAsync(entityToUpdate.Id);

            if (existingEntity == null)
                throw new EntityNotFoundException();

            _context.Entry(existingEntity).CurrentValues.SetValues(entityToUpdate);
            existingEntity.Updated = DateTime.UtcNow;
            _context.Entry(existingEntity).State = EntityState.Modified;
        }

        // ------------------------------
        // 🔹 UPSERT
        // ------------------------------
        public virtual async Task<TDomain> UpsertAsync(TDomain domain)
        {
            var entity = _mapper.Map<TEntity>(domain);
            var existingEntity = await _dbSet.FindAsync(entity.Id);

            if (existingEntity == null)
            {
                entity.Created = entity.Updated = DateTime.UtcNow;
                await _dbSet.AddAsync(entity);
            }
            else
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                existingEntity.Updated = DateTime.UtcNow;
                _context.Entry(existingEntity).State = EntityState.Modified;
            }

            await SaveAsync();
            return _mapper.Map<TDomain>(entity);
        }

        public virtual async Task<TDomain> UpsertWithOutSaveAsync(TDomain domain)
        {
            var entity = _mapper.Map<TEntity>(domain);
            var existingEntity = await _dbSet.FindAsync(entity.Id);

            if (existingEntity == null)
            {
                entity.Created = entity.Updated = DateTime.UtcNow;
                await _dbSet.AddAsync(entity);
                return _mapper.Map<TDomain>(entity);
            }
            else
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                existingEntity.Updated = DateTime.UtcNow;
                _context.Entry(existingEntity).State = EntityState.Modified;
                return _mapper.Map<TDomain>(existingEntity);
            }
        }

        // ------------------------------
        // 🔹 DELETE
        // ------------------------------
        public virtual void Delete(Guid id)
        {
            var entity = _dbSet.Single(e => e.Id == id);
            DeleteEntity(entity);
        }

        public virtual void Delete(TDomain domain)
        {
            var entity = _mapper.Map<TEntity>(domain);
            DeleteEntity(entity);
        }

        private void DeleteEntity(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _dbSet.Attach(entity);

            _dbSet.Remove(entity);
            Save();
        }

        // ------------------------------
        // 🔹 SAVE
        // ------------------------------
        public virtual void Save() => _context.SaveChanges();

        public virtual async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
