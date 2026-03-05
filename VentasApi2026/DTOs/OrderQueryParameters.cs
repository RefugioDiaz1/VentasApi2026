using System.Text.Json.Serialization;
using VentasApi2026.Models;

namespace VentasApi2026.DTOs
{
    public class OrderQueryParameters
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus? Status { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
