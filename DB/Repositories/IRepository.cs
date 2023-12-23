using System.Linq.Expressions;

namespace API.DB.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        #region SqlCommand
        Task<IEnumerable<TEntity>> GetWithRawSql(string query, object[] parameters);


        #endregion

        #region Insert
        TEntity Insert(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity);
        void Insert(List<TEntity> entities);
        Task InsertAsync(List<TEntity> entities);
        #endregion

        #region Update
        //void Update(TEntity entity);
        
        Task<TEntity> UpdateAsync(TEntity entityToUpdate);
        //Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entityToUpdate);

        #endregion

        #region Delete
        void Delete(Expression<Func<TEntity, bool>> filter = null);
        bool Delete(TEntity entity);
        void Delete(IEnumerable<TEntity> entities);
        Task<TEntity> DeleteAsync(object id);
        void Delete(object id);
        Task<TEntity> SoftDelete(object id);
        Task<List<TEntity>> SoftDelete(Expression<Func<TEntity, bool>> filter);
        void SoftDeleteComposite(params object[] parms);
        #endregion

        #region Retreive

        
        #region GetById
        Task<TEntity> GetByIdAsync(object id);
        Task<TEntity> GetByIdIfNotDeleted(int Id);
        TEntity GetById(object id);

        #endregion

        #region GetList
        Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
                                                          string includeProperties = "",
                                                          Expression<Func<TEntity, object>> sortingExpression = null);

        Task<List<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        params Expression<Func<TEntity, object>>[] includeProperties);

        Task<List<TEntity>> GetAsyncWithFilters(
       List<Expression<Func<TEntity, bool>>> filters = null,
       Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
       params Expression<Func<TEntity, object>>[] includeProperties);


        #endregion

        #region Get Paged
        Task<List<TEntity>> GetPagedAsync<TKey>(int PageNumber,
                                               int PageSize,
                                               Expression<Func<TEntity, bool>> filter,
                                               Expression<Func<TEntity, TKey>> sortingExpression,
                                               string includeProperties = "");
        Task<(IEnumerable<TEntity> items, int totalCount)> GetPagedAsync(int PageNumber, int pageSize,
         Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         params Expression<Func<TEntity, object>>[] includeProperties);
        Task<List<TEntity>> GetPagedAsyncWithFilters<TKey>(int PageNumber,
                                               int PageSize,
                                               List<Expression<Func<TEntity, bool>>> filter,
                                               Expression<Func<TEntity, TKey>> sortingExpression,
                                               string includeProperties = "");
        Task<(IEnumerable<TEntity> items, int totalCount)> GetPagedAsyncWithFilters(int PageNumber, int pageSize,
         List<Expression<Func<TEntity, bool>>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         params Expression<Func<TEntity, object>>[] includeProperties);
        #endregion

        #region Get Individuals
        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null,
                                         string includeProperties = "");
        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter = null);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> filter, string includeProperties = "");
        Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");
        Task<TEntity> Max(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");
        Task<TEntity> Min(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");

        #endregion

        #region Clone Entity

        
        TEntity ClonePropertiesValues<TId>(TEntity obj, TId newId, params string[] propertyNames);

        Task<TEntity> ClonePropertiesValues<TId>(object id, TId newId, params string[] propertyNames);
        

        #endregion

        #endregion

    }
}
