using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesManagementService.Domain.Common;

namespace SalesManagementService.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        public Repository(DbContext context)
        {
            this._context = context;
        }

        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }
        public virtual async Task<ICollection<TEntity>> GetAllAsyn()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public IEnumerable<TEntity> GetAll()
        {
            lock (this)
            {
                return _context.Set<TEntity>().ToList();
            }
        }
        public IEnumerable<TEntity> GetAllNoTracking()
        {
            lock (this)
            {
                return _context.Set<TEntity>().AsNoTracking().ToList();
            }
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().AsNoTracking().Where(predicate);
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Where(predicate).Count();
        }


        public virtual TEntity FindSingle(Expression<Func<TEntity, bool>> match)
        {
            lock (this)
            {

                return _context.Set<TEntity>().SingleOrDefault(match);
            }
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> match)
        {
            lock (this)
            {

                return _context.Set<TEntity>().Any(match);
            }
        }

        public TEntity Get(int id)
        {
            lock (this)
            {
                return _context.Set<TEntity>().Find(id);
            }
        }

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }
        public void RemoveKey(int key)
        {
            _context.Set<TEntity>().Remove(_context.Set<TEntity>().Find(key));
        }
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }
        public virtual TEntity Update(TEntity t, object key)
        {
            if (t == null)
                return null;
            TEntity exist = _context.Set<TEntity>().Find(key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(t);
                _context.SaveChanges();
            }
            return exist;
        }
        public virtual TEntity UpdateTrans(TEntity t, object key)
        {
            if (t == null)
                return null;
            TEntity exist = _context.Set<TEntity>().Find(key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(t);
            }
            return exist;
        }
        public virtual async Task<TEntity> AddAsyn(TEntity t)
        {

            _context.Set<TEntity>().Add(t);
            await _context.SaveChangesAsync();
            return t;

        }
        public virtual void UpdateColumn(TEntity entity, params Expression<Func<TEntity, object>>[] updatedProperties)
        {
            //dbEntityEntry.State = EntityState.Modified; --- I cannot do this.

            //Ensure only modified fields are updated.
            var dbEntityEntry = _context.Entry(entity);
            if (updatedProperties.Any())
            {
                //update explicitly mentioned properties
                foreach (var property in updatedProperties)
                {
                    dbEntityEntry.Property(property).IsModified = true;
                }
            }
            else
            {
                //no items mentioned, so find out the updated entries
                foreach (var property in dbEntityEntry.OriginalValues.Properties)
                {
                    var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
                    var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
                    if (original != null && !original.Equals(current))
                        dbEntityEntry.Property(property.Name).IsModified = true;
                }
            }
        }
    }
}
