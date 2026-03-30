namespace SmartCommerce.API.DTOs.Common
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }

        public string Error { get; set; }

        public T Data { get; set; }

        public static ServiceResult<T> SuccessResult(T data) => new() { Success = true, Data = data };

        public static ServiceResult<T> Failure(string error)
            => new() { Success = false, Error = error };
    }
}
