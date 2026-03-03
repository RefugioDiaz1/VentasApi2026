using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private readonly IOrderRepository _repository;
        private readonly IProductRepository _repositoryProduct;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository repository, IProductRepository repositoryProduct, IMapper mapper)
        {
            _repository = repository;
            this._repositoryProduct = repositoryProduct;
            _mapper = mapper;
        }

        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            // Aquí va la lógica que ya te expliqué
            if (dto.Items == null || !dto.Items.Any())
                throw new BadRequestException("Order must contain at least one item");

            var productIds = dto.Items
                            .Select(x => x.ProductId)
                            .Distinct()
                            .ToList();

            var products = await _repositoryProduct.GetByIdsAsync(productIds);

            if (products.Count != productIds.Count)
                throw new NotFoundException("One or more products not found");

            var order = new Order
            {
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

            await _repository.AddAsync(order);

            return _mapper.Map<OrderDto>(order);

        }

        public async Task CompleteAsync(int id) {

            var order = await _repository.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException("Order not found");

            if (order.Status == OrderStatus.Cancelled)
                throw new ConflictException("Cancelled order cannot be completed");

            if (order.Status == OrderStatus.Completed)
                throw new ConflictException("Order already completed");

            order.Status = OrderStatus.Completed;
            order.CompletedAt = DateTime.Now;
            await _repository.SaveChangesAsync();      
        }

        public async Task CancelAsync(int id)
        {
            var order = await _repository.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException("Order not found");

            if (order.Status == OrderStatus.Cancelled)
                throw new ConflictException("Order already cancelled");

            var productIds = order.Details
                            .Select(d => d.ProductId)
                            .ToList();

            var products = await _repositoryProduct.GetByIdsAsync(productIds);

            foreach (var detail in order.Details)
            {
                var product = products.First(p => p.Id == detail.ProductId);

                product.Stock += detail.Quantity;
            }

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.Now;

            await _repository.SaveChangesAsync();

        }

        public async Task<OrderDto> GetByIdAsync(int id)
        {
            var order = await _repository.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException("Order not found");

            return _mapper.Map<OrderDto>(order);

        }

    }
}
