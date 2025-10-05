using Application.Dtos;
using Application.IServices;
using Application.Common.Helpers;
using Core.Enums;
using Application.IRepositories;
using Core.Entities;

namespace Infrastructure.Services
{
    public class BarcodeService : IBarcodeService
    {
        private readonly IGenericRepository<Warehouse> _warehouseRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Supplier> _supplierRepository;

        public BarcodeService(IGenericRepository<Warehouse> warehouseRepository, IGenericRepository<Product> productRepository, IGenericRepository<Supplier> supplierRepository)
        {
            this._warehouseRepository = warehouseRepository;
            this._productRepository = productRepository;
            this._supplierRepository = supplierRepository;
        }

        public async Task<BarcodeResponseDto> GenerateOneAsync(BarcodeRequestDto request, CancellationToken cancellationToken)
        {
            var base64 = BarcodeHelper.GenerateByHelper(request);
            if (string.IsNullOrEmpty(base64)) throw new Exception("Failed to generate barcode");
            return new BarcodeResponseDto
            {
                Name = request.Name,
                Data = base64,
                Content = request.Content,
                BarcodeType = request.BarcodeType,
                Uid = request.Content,
            };
        }

        public async Task<IEnumerable<BarcodeResponseDto>> GenerateBatchAsync(BarcodeRequestDto request, CancellationToken cancellationToken)
        {
            switch (request.BarcodeForType)
            {
                case BarcodeForType.Warehouse:
                    var warehouses = await _warehouseRepository.FindManyAsync(x => x.Status && x.ApprovalStatus == ApprovalStatus.Approved);
                    if (warehouses == null || !warehouses.Any()) throw new KeyNotFoundException("Warehouse not found");
                    return await Task.WhenAll(warehouses.Select(warehouse => GenerateOneAsync(new BarcodeRequestDto
                    {
                        BarcodeType = request.BarcodeType,
                        Size = request.Size,
                        BarcodeForType = request.BarcodeForType,
                        Content = warehouse.Uid,
                        Name = warehouse.Name,
                        UserId = request.UserId
                    }, cancellationToken)));
                case BarcodeForType.Product:
                    var products = await _productRepository.FindManyAsync(x => x.Status && x.ApprovalStatus == ApprovalStatus.Approved);
                    if (products == null || !products.Any()) throw new KeyNotFoundException("Product not found");
                    return await Task.WhenAll(products.Select(product => GenerateOneAsync(new BarcodeRequestDto
                    {
                        BarcodeType = request.BarcodeType,
                        Size = request.Size,
                        BarcodeForType = request.BarcodeForType,
                        Content = product.Uid,
                        Name = product.Name,
                        UserId = request.UserId
                    }, cancellationToken)));
                case BarcodeForType.Supplier:
                    var suppliers = await _supplierRepository.FindManyAsync(x => x.Status);
                    if (suppliers == null || !suppliers.Any()) throw new KeyNotFoundException("Supplier not found");
                    return await Task.WhenAll(suppliers.Select(supplier => GenerateOneAsync(new BarcodeRequestDto
                    {
                        BarcodeType = request.BarcodeType,
                        Size = request.Size,
                        BarcodeForType = request.BarcodeForType,
                        Content = supplier.Uid,
                        Name = supplier.Name,
                        UserId = request.UserId
                    }, cancellationToken)));
                default:
                    throw new NotSupportedException("Unsupported barcode for type");
            }
        }
    }
}