using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using SharedLibrary.Dtos;
using System.Linq.Expressions;

namespace AuthServer.Service.Services
{
    public class GenericService<T, TDto> : IGenericService<T, TDto> where T : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<T> _genericRepository;
        private readonly IMapper _mapper;

        public GenericService(IUnitOfWork unitOfWork, IGenericRepository<T> genericRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
            _mapper = mapper;
        }

        public async Task<Response<TDto>> AddAsync(TDto tDto) // Client tarafından gelen bir metod
        {
            var newEntity = _mapper.Map<T>(tDto); // Client'tan gelen TDto türündeki tDto nesnesini T'ye dönüştürüp newEntity nesnesine atadık.

            await _genericRepository.AddAsync(newEntity); // newEntity nesnesini _genericRepository ile DB'ye eklemek için memory'ye aldık.
            await _unitOfWork.CommitAsync();      // _unitOfWork ile memory'deki newEntity nesini veritabanına kaydettik.

            var newDto=_mapper.Map<TDto>(newEntity);  // T türündeki newEntity nesnesini TDto türüne çevirdik ve newDto nesnesine atadık. Çünkü dönüş imzası Task<Response<TDto>>
            return Response<TDto>.Success(newDto,201); // kullanıcıya Response sınıfında tanımlanan propertyler türünde başarılı döndük.
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync() // Client'a DB'den tüm verileri liste şeklinde dönme metodu
        {
            var entities=await _genericRepository.GetAllAsync();
            var entitiesDto= _mapper.Map<List<TDto>>(entities);
            return Response<IEnumerable<TDto>>.Success(entitiesDto, 200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id) // Client'a döndüğümüz metod
        {
            var entity= await _genericRepository.GetByIdAsync(id); // ilgili id'ye sahip datayı veritabanından alıp entity nesnesine atadık
            if (entity == null)
            {
                return Response<TDto>.Fail("Data Not Found", 404,true);
            }
            var entityDto= _mapper.Map<TDto>(entity);  // T türündeki entity nesnesini TDto türündeki entityDto nesnesine atadık.
            return Response<TDto>.Success(entityDto, 200);
        }

        public async Task<Response<NoDataDto>> Remove(int id) // Client, silmek istediği veriyi gönderiyor. DB'den silme metodu. Client'a bir data dönmeye gerek yok
        {
            var response = await _genericRepository.GetByIdAsync(id); // DB'den, Client silmemi istediği verinin id'sini aldım ve response nesnesine atadım.
            if(response == null)
            {
                return Response<NoDataDto>.Fail("There is no data for remove", 404,true); // response nesnesinde veri yok ise bu cevabı dön Client'a
            }
            _genericRepository.Remove(response); // Eğer DB'de veri var ise sileceğin veriyi memory'de tutarız.
            await _unitOfWork.CommitAsync();             // Kaydetme işlemini UnitOfWork sınıfında CommitAsync metodunda tanımladık. CommitAsync metodu ile silme işlemi kaydedildi
            return Response<NoDataDto>.Success(204); 
        }

        public async Task<Response<NoDataDto>> Update(TDto tDto)
        {
            var response = _mapper.Map<T>(tDto);
            if(response == null)
            {
                return Response<NoDataDto>.Fail("There is no data for update",404,true);
            }

            _genericRepository.Update(response);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<T, bool>> predicate)
        {
            var list= _genericRepository.Where(predicate);
            return Response<IEnumerable<TDto>>.Success(_mapper.Map<IEnumerable<TDto>>(await list.ToListAsync()),200);
        }
    }
}
