using Application.Dtos;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class PositionService : IPositionService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public PositionService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PositionResponseDto>> GetAllAsync()
    {
        var list = await _dbContext.Positions.AsNoTracking().OrderBy(p => p.Name).ToListAsync();
        return _mapper.Map<IEnumerable<PositionResponseDto>>(list);
    }

    public async Task<PositionResponseDto> CreateAsync(PositionRequestDto dto)
    {
        if (await _dbContext.Positions.AnyAsync(p => p.Uid == dto.Uid))
            throw new Exception("Position UID already exists");
        var entity = _mapper.Map<Position>(dto);
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = dto.CreatedBy;
        _dbContext.Positions.Add(entity);
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<PositionResponseDto>(entity);
    }
}
