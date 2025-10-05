using Application.Dtos;

namespace Application.IServices
{
    public interface IBarcodeService
    {
        Task<BarcodeResponseDto> GenerateOneAsync(BarcodeRequestDto request, CancellationToken cancellationToken = default);
        Task<IEnumerable<BarcodeResponseDto>> GenerateBatchAsync(BarcodeRequestDto request, CancellationToken cancellationToken = default);
    }
}