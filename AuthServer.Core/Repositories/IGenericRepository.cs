using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    public interface IGenericRepository<T>where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();  // IQuaryable sorgulu bir şekilde tüm dataları getirmek için oluşturduk

        IQueryable<T> Where(Expression<Func<T,bool>>predicate); // ToList() yapmadan liste gelmez.
        Task AddAsync(T entity);
        void Remove(T entity);
        void Update(T entity);

    }
}
