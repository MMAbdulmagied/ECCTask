using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;

namespace API.DB.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        #region Dependencies
        protected internal PortalDBContext Context { get; }
        protected internal DbSet<TEntity> DbSet { get; }
        private Expression<Func<TEntity, bool>> GlobalFilter { get; set; } = null;

        #endregion

        #region Constructor
        public Repository(PortalDBContext context)
        {
            this.Context = context;
            this.DbSet = context.Set<TEntity>();
        }
        public Repository()
        {


        }
        #endregion

        #region Public Methods
        #region SqlCommand

        public async Task<IEnumerable<TEntity>> GetWithRawSql(string query, object[] parameters)
        {
            var result = await DbSet.FromSqlRaw(query, parameters).ToListAsync();
            return result;
        }
        protected internal DbDataReader ExecSqlCommand(string commandText, CommandType commandType, params SqlParameter[] sqlParameters)
        {
            var cmd = Context.Database.GetDbConnection().CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = commandText;
            cmd.Parameters.AddRange(sqlParameters);
            Context.Database.OpenConnection();
            var reader = cmd.ExecuteReader();

            return reader;
        }


        #endregion

        #region Insert
        public TEntity Insert(TEntity entity)
        {
            return entity == null ? throw new ArgumentNullException(nameof(entity)) : DbSet.Add(entity).Entity;
        }
        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var e = entity == null ? throw new ArgumentNullException(nameof(entity)) : (await DbSet.AddAsync(entity)).Entity;
            Context.SaveChanges();
            return e;
        }
        public void Insert(List<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            DbSet.AddRange(entities);
        }
        public async Task InsertAsync(List<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await DbSet.AddRangeAsync(entities);
        }
        #endregion

        #region Update

        public async Task<TEntity> UpdateAsync(TEntity entityToUpdate)
        {
            //dbSet.Attach(entityToUpdate);
            if (Context.Entry(entityToUpdate).State == EntityState.Detached)
            {
                DbSet.Attach(entityToUpdate);
            }
            Context.Entry(entityToUpdate).State = EntityState.Modified;
            Context.SaveChanges();
            return await Task.FromResult(entityToUpdate);
        }

        //public async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entityToUpdate)
        //{
        //    if (Context.Entry(entityToUpdate).State == EntityState.Detached)
        //    {
        //        DbSet.AttachRange(entityToUpdate);
        //    }
        //    Context.Entry(entityToUpdate).State = EntityState.Modified;

        //    return await Task.FromResult(entityToUpdate);
        //}
        #endregion

        #region Delete
        public void Delete(Expression<Func<TEntity, bool>> filter = null)
        {
            DbSet.RemoveRange(DbSet.Where(filter));
        }
        public bool Delete(TEntity entity)
        {
            var deletedEntity = this.DbSet.Remove(entity);
            return this.Context.Entry(deletedEntity.Entity).State == EntityState.Deleted;
        }
        public void Delete(IEnumerable<TEntity> entities)
        {
            this.DbSet.RemoveRange(entities);
        }
        public async Task<TEntity> DeleteAsync(object id)
        {
            TEntity entityToDelete = await DbSet.FindAsync(id);
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
            Context.SaveChanges();
            return entityToDelete;
        }
        public void Delete(object id)
        {
            var entity = DbSet.Find(id);
            this.Delete(entity);
        }
        public async Task<TEntity> SoftDelete(object id)
        {
            TEntity entityToDelete = await DbSet.FindAsync(id);

            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            Context.Entry(entityToDelete).State = EntityState.Modified;
            Context.Entry(entityToDelete).Property("IsDeleted").CurrentValue = true;

            return entityToDelete;
        }

        public async Task<List<TEntity>> SoftDelete(Expression<Func<TEntity, bool>> filter)
        {
            List<TEntity> entitiesToDelete = await DbSet.Where(filter).ToListAsync();

            if (entitiesToDelete.Any())
            {
                foreach (var entityToDelete in entitiesToDelete)
                {
                    if (Context.Entry(entityToDelete).State == EntityState.Detached)
                    {
                        DbSet.Attach(entityToDelete);
                    }
                    Context.Entry(entityToDelete).State = EntityState.Modified;
                    Context.Entry(entityToDelete).Property("IsDeleted").CurrentValue = true;
                }
            }
            return entitiesToDelete;
        }
        public void SoftDeleteComposite(params object[] parms)
        {
            TEntity entityToDelete = DbSet.Find(parms);
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            Context.Entry(entityToDelete).State = EntityState.Modified;
            Context.Entry(entityToDelete).Property("IsDeleted").CurrentValue = true;

        }

        #endregion

        #region Retreive


        #region GetById
        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await DbSet.FindAsync(id);
        }
        public TEntity GetById(object id)
        {
            return DbSet.Find(id);
        }
        public async Task<TEntity> GetByIdIfNotDeleted(int Id)
        {
            TEntity record = await DbSet.FindAsync(Id);
            //context.Entry(record).State = EntityState.Detached;
            if (record != null)
            {
                var property = record.GetType().GetProperties().FirstOrDefault(a => a.Name == "IsDeleted" /*According to system naming convension here*/);
                if (property != null && (bool)property.GetValue(record))
                    return null;
                else
                    Context.Entry(record).State = EntityState.Detached;
            }
            return record;
        }

        #endregion

        #region GetList
        public async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
                                                       string includeProperties = "",
                                                       Expression<Func<TEntity, object>> sortingExpression = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {

                query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }



            return await query.AsNoTracking().ToListAsync();
        }


        public async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
                                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                    params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null && includeProperties.Any())
            {
                if (includeProperties != null)
                {
                    query = includeProperties.Aggregate(query, (current, include) => current.Include(include));
                }
            }

            return orderBy != null ? await orderBy(query).ToListAsync() : await query.ToListAsync();
        }

        public async Task<List<TEntity>> GetAsyncWithFilters(List<Expression<Func<TEntity, bool>>> filters = null,
                                                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet;

            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    if (filter != null)
                    {
                        query = query.Where(filter);
                    }
                }
            }

            if (includeProperties != null && includeProperties.Any())
            {
                if (includeProperties != null)
                {
                    query = includeProperties.Aggregate(query, (current, include) => current.Include(include));
                }
            }

            return orderBy != null ? await orderBy(query).ToListAsync() : await query.ToListAsync();
        }

        #endregion

        #region Get Paged
        public async Task<List<TEntity>> GetPagedAsync<TKey>(int PageNumber,
                                                            int PageSize,
                                                            Expression<Func<TEntity, bool>> filter,
                                                            Expression<Func<TEntity, TKey>> sortingExpression,
                                                            string includeProperties = "")
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            int skipCount = (PageNumber - 1) * PageSize;

            if (filter != null)
                query = query.Where(filter);

            query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));


            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<(IEnumerable<TEntity> items, int totalCount)> GetPagedAsync(int PageNumber,
                                                                                              int pageSize,
                                                                                              Expression<Func<TEntity, bool>> filter = null,
                                                                                              Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                                                              params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking();
            int skipCount = (PageNumber - 1) * pageSize;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            var totalCount = query.Select(a => new { c = "1" }).Count();

            if (includeProperties != null && includeProperties.Any())
            {
                if (includeProperties != null)
                {
                    query = includeProperties.Aggregate(query, (current, include) => current.Include(include));
                }
            }

            return orderBy != null
                ? pageSize > 0
                    ? ((IEnumerable<TEntity> items, int totalCount))(await orderBy(query).Skip(skipCount).Take(pageSize).ToListAsync(), totalCount)
                    : ((IEnumerable<TEntity> items, int totalCount))(await orderBy(query).ToListAsync(), totalCount)
                : pageSize > 0
                ? ((IEnumerable<TEntity> items, int totalCount))(await query.Skip(skipCount).Take(pageSize).ToListAsync(), totalCount)
                : ((IEnumerable<TEntity> items, int totalCount))(await query.ToListAsync(), totalCount);
        }
        public async Task<List<TEntity>> GetPagedAsyncWithFilters<TKey>(int PageNumber,
                                                   int PageSize,
                                                   List<Expression<Func<TEntity, bool>>> filter,
                                                   Expression<Func<TEntity, TKey>> sortingExpression,
                                                   string includeProperties = "")
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            int skipCount = (PageNumber - 1) * PageSize;

            if (filter != null)
            {
                foreach (var item in filter)
                {
                    query = query.Where(item);
                }
            }

            query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<(IEnumerable<TEntity> items, int totalCount)> GetPagedAsyncWithFilters(int PageNumber,
                                                                                              int pageSize,
                                                                                              List<Expression<Func<TEntity, bool>>> filter = null,
                                                                                              Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                                                              params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking();
            int skipCount = (PageNumber - 1) * pageSize;
            if (filter != null)
            {
                foreach (var item in filter)
                {
                    query = query.Where(item);
                }
            }
            var totalCount = query.Select(a => new { c = "1" }).Count();

            if (includeProperties != null && includeProperties.Any())
            {
                if (includeProperties != null)
                {
                    query = includeProperties.Aggregate(query, (current, include) => current.Include(include));
                }
            }

            return orderBy != null
                ? pageSize > 0
                    ? ((IEnumerable<TEntity> items, int totalCount))(await orderBy(query).Skip(skipCount).Take(pageSize).ToListAsync(), totalCount)
                    : ((IEnumerable<TEntity> items, int totalCount))(await orderBy(query).ToListAsync(), totalCount)
                : pageSize > 0
                ? ((IEnumerable<TEntity> items, int totalCount))(await query.Skip(skipCount).Take(pageSize).ToListAsync(), totalCount)
                : ((IEnumerable<TEntity> items, int totalCount))(await query.ToListAsync(), totalCount);
        }
        #endregion

        #region Get Individuals
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null,
                                            string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;
            if (filter != null)
                query = query.Where(filter);
            query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await query.AsNoTracking().CountAsync();
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return await DbSet.CountAsync(filter);
        }
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return await DbSet.AnyAsync(filter);
        }
        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            if (filter != null)
                query = query.Where(filter).AsNoTracking();
            query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            TEntity record = await query.FirstOrDefaultAsync();
            if (record != default)
                Context.Entry(record).State = EntityState.Detached;
            return record;
        }
        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null && includeProperties.Any())
            {
                if (includeProperties != null)
                {
                    query = includeProperties.Aggregate(query, (current, include) => current.Include(include));
                }
            }

            return await query.FirstOrDefaultAsync();
        }
        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> filter, string includeProperties = "")
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            if (filter != null)
                query = query.Where(filter);
            query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            TEntity record = await query.SingleOrDefaultAsync();
            if (record != default)
                Context.Entry(record).State = EntityState.Detached;
            return record;
        }
        public async Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            if (filter != null)
                query = query.Where(filter).AsNoTracking();
            query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            TEntity record = await query.LastOrDefaultAsync();
            if (record != default)
                Context.Entry(record).State = EntityState.Detached;
            return record;
        }
        public async Task<TEntity> Max(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            if (filter != null)
                query = query.Where(filter).AsNoTracking();
            query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            TEntity record = await query.MaxAsync();
            if (record != default)
                Context.Entry(record).State = EntityState.Detached;
            return record;
        }
        public async Task<TEntity> Min(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            if (filter != null)
                query = query.Where(filter).AsNoTracking();
            query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            TEntity record = await query.MinAsync();
            if (record != default)
                Context.Entry(record).State = EntityState.Detached;
            return record;
        }

        #endregion

        #region Clone Entity

        public TEntity ClonePropertiesValues<TId>(TEntity obj, TId newId, params string[] propertyNames)
        {
            var clonedObj = Activator.CreateInstance<TEntity>();
            dynamic values = null;

            if (propertyNames.Any())
            {
                values = new Dictionary<string, object>();
                var names = propertyNames.ToList();
                names.Remove("Id");
                names.ToList().ForEach(pname => values.Add(pname, Context.Entry(obj).Property(pname).CurrentValue));
            }
            else
            {
                values = Context.Entry(obj).CurrentValues.Clone();
            }

            var IdType = Context.Entry(obj).Property("Id").Metadata.ClrType;

            if (IdType != typeof(TId))
            {
                throw new Exception($"Invalid Type of {typeof(TEntity)}.Id");
            }
            values["Id"] = newId;
            Context.Entry(clonedObj).CurrentValues.SetValues(values);

            return clonedObj;
        }

        public async Task<TEntity> ClonePropertiesValues<TId>(object id, TId newId, params string[] propertyNames)
        {
            TEntity entitObj = await DbSet.FindAsync(id);
            var clonedObj = ClonePropertiesValues(entitObj, newId, propertyNames);

            return clonedObj;
        }


        #endregion

        #endregion

        #endregion
    }
}
