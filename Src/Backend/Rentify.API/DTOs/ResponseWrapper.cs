namespace Rentify.API.DTOs
{
    public class ResponseWrapper<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public IEnumerable<string>? ErrorList { get; set; }

        public static ResponseWrapper<T> SuccessResponse(T data)
        {
            return new ResponseWrapper<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static ResponseWrapper<T> ErrorResponse(IEnumerable<string> errors)
        {
            return new ResponseWrapper<T>
            {
                IsSuccess = false,
                ErrorList = new List<string>(errors)
            };
        }
    }
}
