using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Persistence.ReposMocks
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;
        private DbSet<TEntity> _entities;
        public Repository(DbContext context)
        {
            this.Context = context;
            this._entities = context.Set<TEntity>();
        }
        public async Task Add(TEntity entity)
        {
            await this._entities.AddAsync(entity);
        }

        public async Task AddRange(IEnumerable<TEntity> entities)
        {
            await this._entities.AddRangeAsync(entities);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return this._entities.Where(predicate);
        }

        public async Task<TEntity> Get(Guid id)
        {
            return await this._entities.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await this._entities.ToListAsync();
        }
        public async Task Load(Expression<Func<TEntity, bool>> predicate)
        {
            await this._entities.Where(predicate).LoadAsync();
        }
        public void Remove(TEntity entity)
        {
            this._entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            this._entities.RemoveRange(entities);
        }

        public async Task<TEntity> SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return await this._entities.SingleOrDefaultAsync(predicate);
        }
    }
}