using Application.Dtos;

namespace Application.IServices;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentResponseDto>> GetAllAsync();
    Task<DepartmentResponseDto> CreateAsync(DepartmentRequestDto dto);
}
