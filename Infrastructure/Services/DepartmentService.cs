using Application.Dtos;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class DepartmentService : IDepartmentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public DepartmentService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DepartmentResponseDto>> GetAllAsync()
    {
        var list = await _dbContext.Departments.AsNoTracking().OrderBy(d => d.Name).ToListAsync();
        return _mapper.Map<IEnumerable<DepartmentResponseDto>>(list);
    }

    public async Task<DepartmentResponseDto> CreateAsync(DepartmentRequestDto dto)
    {
        if (await _dbContext.Departments.AnyAsync(d => d.Uid == dto.Uid))
            throw new Exception("Department UID already exists");
        var entity = _mapper.Map<Department>(dto);
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = dto.CreatedBy;
        _dbContext.Departments.Add(entity);
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<DepartmentResponseDto>(entity);
    }
}
