using Application.Dtos;

namespace Application.IServices;

public interface IPositionService
{
    Task<IEnumerable<PositionResponseDto>> GetAllAsync();
    Task<PositionResponseDto> CreateAsync(PositionRequestDto dto);
}
