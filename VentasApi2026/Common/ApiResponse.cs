namespace VentasApi2026.Common
{

//    🧠 Qué acabamos de crear

//Un contrato universal para tu API.

//Ahora TODO responde igual.

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data)
            => new() { Success = true, Data = data };

        public static ApiResponse<T> Fail(string message)
            => new() { Success = false, Message = message };
    }
}
