using SharedLibrary.Dtos;
using System.Text.Json.Serialization;

namespace SharedLibrary
{
    public class Response<T> where T : class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }
        public ErrorDto Error { get; private set; }

        [JsonIgnore]
        public bool IsSuccesful { get; private set; } // Client'lara göstermek istemediğimiz bir property. Kendi iç yapımızda kullanacağız. API, Mini API gibi




        // METODLAR
        public static Response<T> Success(T data,int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccesful = true };
        }

        // İstek başaralı ama data dönmek istemediğimiz yerde bu metodu kullanacağız. Data=default yaptık, status code se yukarıdan gelecek.Silme ve güncelleme işlemlerinden sonra data dönmeye gerek yok. Yani sildiğin veya güncellediğin datayı tekrar dönme. Ekleme işleminde, eklediğin datayı geri dönmemiz lazım
        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default, StatusCode = statusCode, IsSuccesful = true };
        }

        public static Response<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new Response<T> { Error = errorDto, StatusCode = statusCode, IsSuccesful = false };
        }

        // Sadece bir tane hata aldığımızda bu metodu kullan
        public static Response<T> Fail(string errorMessage,int statusCode, bool isShow)
        {
            var errorDto = new ErrorDto(errorMessage, isShow);
            return new Response<T> { Error = errorDto,StatusCode = statusCode, IsSuccesful = false };
        }
    }
}
