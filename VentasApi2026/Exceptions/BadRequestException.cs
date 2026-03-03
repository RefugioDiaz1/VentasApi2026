namespace VentasApi2026.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message)
       : base(message)
        {
        }
    }
}
