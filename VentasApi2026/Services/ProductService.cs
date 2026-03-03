using AutoMapper;
using VentasApi2026.Common;
using VentasApi2026.DTOs;
using VentasApi2026.Exceptions;
using VentasApi2026.Models;
using VentasApi2026.Repositories.Interfaces;
using VentasApi2026.Services.Interfaces;

namespace VentasApi2026.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper) {
            this._repository = repository;
            this._mapper = mapper;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _repository.AddAsync(product);
            return _mapper.Map<ProductDto>(product);

        }
        public async Task<PagedResult<ProductDto>> GetPagedAsync(PaginationParams pagination)
        {
            var result = await _repository.GetPagedAsync(pagination);

            return new PagedResult<ProductDto>
            {
                Items = _mapper.Map<IEnumerable<ProductDto>>(result.Items),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalRecords = result.TotalRecords
            };
        }

        public async Task<ProductDto> getByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                throw new NotFoundException("Product not found");

            return _mapper.Map<ProductDto>(product);
           
        }

        public async Task<ProductDto> UpdateProduct(int id, UpdateProductDto data)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                throw new NotFoundException("Product not found");

            _mapper.Map(data, product);

            await _repository.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteById(int id)
        {
            var product = await _repository.DeleteById(id);

            if (!product)
                throw new NotFoundException("Product not found");

            return product;
        }
    }
}
