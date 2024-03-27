using SharedLibrary;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    // IGenericRepository interfacesine göre farklılık vardır. Update ve Remove metodlarının dönüş tipleri vs. gibi. Burada Response sınıfından dönüş yaptık. BestPractices
    public interface IGenericService<T, TDto>where T : class where TDto : class
    {
        Task<Response<TDto>> GetByIdAsync(int id);
        Task<Response<IEnumerable<TDto>>>  GetAllAsync();  // IQuaryable sorgulu bir şekilde tüm dataları getirmek için oluşturduk

        Task<Response<IEnumerable<TDto>>> Where(Expression<Func<T, bool>> predicate); // ToList() yapmadan liste gelmez.
        Task<Response<TDto>> AddAsync(TDto tDto);

        // Silme işleminde Response sınıfındaki Data, StatusCode ve Errors propertylerinden Data propertysinde bir veri dönmek istemedik. Statuscode ve ve varsa Errorsları dönmek istedik. 
        Task<Response<NoDataDto>> Remove(int id);  
        Task<Response<NoDataDto>> Update(TDto tDto);
    }
}
