using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VentasApi2026.Common;
using VentasApi2026.Data;
using VentasApi2026.DTOs;
using VentasApi2026.Exceptions;
using VentasApi2026.Models;
using VentasApi2026.Repositories.Interfaces;
using VentasApi2026.Services.Interfaces;

namespace VentasApi2026.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService( IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto, int userId)
        {
            // Aquí va la lógica que ya te expliqué
            if (dto.Items == null || !dto.Items.Any())
                throw new BadRequestException("Order must contain at least one item");

            var productIds = dto.Items
                            .Select(x => x.ProductId)
                            .Distinct()
                            .ToList();

            var products = await _unitOfWork.Products.GetByIdsAsync(productIds);

            if (products.Count != productIds.Count)
                throw new NotFoundException("One or more products not found");

            var order = new Order
            {
                CreatedByUserId = userId,
                Date = DateTime.UtcNow
            };

            var groupOrderItem = dto.Items.GroupBy(w => w.ProductId).Select(a => new CreateOrderItemDto { ProductId = a.Key, Quantity = a.Sum(w => w.Quantity) }).ToList();

            foreach (var item in groupOrderItem)
            {
                var product = products.First(p => p.Id == item.ProductId);

                if (product.Stock < item.Quantity)
                    throw new BadRequestException(
                        $"Insufficient stock for product {product.Name}");

                var detail = new OrderDetail
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                order.Details.Add(detail);

                order.Total += product.Price * item.Quantity;

                // actualizar inventario
                product.Stock -= item.Quantity;
            }

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);

        }

        public async Task CompleteAsync(int id, int userId, string role) {

            var order = await _unitOfWork.Orders.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException("Order not found");

            // 🔥 Validación de propiedad
            if (role != "Admin" && order.CreatedByUserId != userId)
                throw new UnauthorizedAccessException("You cannot complete this order");

            if (order.Status == OrderStatus.Cancelled)
                throw new ConflictException("Cancelled order cannot be completed");

            if (order.Status == OrderStatus.Completed)
                throw new ConflictException("Order already completed");

            order.Status = OrderStatus.Completed;
            order.CompletedAt = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();      
        }

        public async Task CancelAsync(int id, int userId, string role)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException("Order not found");

            // 🔥 Validación de propiedad
            if (role != "Admin" && order.CreatedByUserId != userId)
                throw new UnauthorizedException("You cannot complete this order");

            if (order.Status == OrderStatus.Cancelled)
                throw new ConflictException("Order already cancelled");

            var productIds = order.Details
                            .Select(d => d.ProductId)
                            .ToList();

            var products = await _unitOfWork.Products.GetByIdsAsync(productIds);

            foreach (var detail in order.Details)
            {
                var product = products.First(p => p.Id == detail.ProductId);

                product.Stock += detail.Quantity;
            }

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();

        }

        public async Task<OrderDto> GetByIdAsync(int id, int userId, string role)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException("Order not found");

            // 🔥 Si NO es Admin, validar propiedad
            if (role != "Admin" && order.CreatedByUserId != userId)
                throw new UnauthorizedException("You cannot access this order");

            return _mapper.Map<OrderDto>(order);

        }

        public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId, string role)
        {
            IEnumerable<Order> orders;

            if (role == "Admin")
            {
                orders = await _unitOfWork.Orders.GetAllOrders();
            }
            else {

                orders = await _unitOfWork.Orders.GetMyOrdersAsync(userId);
            }

            return _mapper.Map<IEnumerable<OrderDto>>(orders);

        }

        public async Task<IEnumerable<OrderDto>> GetAllOrders()
        {
            var order = await _unitOfWork.Orders.GetAllOrders();

            return _mapper.Map<IEnumerable<OrderDto>>(order);

        }

        //Aqui se aplican filtros en el query
        public async Task<PagedResult<OrderDto>> GetFilteredAsync(
    OrderQueryParameters query,
    int userId,
    string role)
        {
            var ordersQuery = _unitOfWork.Orders.Query();
            // Este método debe devolver IQueryable<Order>

            // 🔐 Seguridad por propiedad
            if (role != "Admin")
                ordersQuery = ordersQuery.Where(o => o.CreatedByUserId == userId);

            // 🔎 Filtro por status
            if (query.Status.HasValue)
                ordersQuery = ordersQuery.Where(o => o.Status == query.Status.Value);

            // 📅 Filtro por fecha desde
            if (query.From.HasValue)
                ordersQuery = ordersQuery.Where(o => o.Date >= query.From.Value);
                
            // 📅 Filtro por fecha hasta
            if (query.To.HasValue)
                ordersQuery = ordersQuery.Where(o => o.Date <= query.To.Value.Date.AddDays(1).AddTicks(-1));

            var totalCount = await ordersQuery.CountAsync();

            var items = await ordersQuery
                .OrderByDescending(o => o.Date)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<OrderDto>
            {
                TotalRecords = totalCount,
                Page = query.Page,
                PageSize = query.PageSize,
                Items = _mapper.Map<List<OrderDto>>(items)
            };
        }


    }
}
