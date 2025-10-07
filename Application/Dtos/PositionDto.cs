using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

public record class PositionRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Uid { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    public bool Status { get; set; } = true;
    public Guid? CreatedBy { get; set; }
}

public record class PositionResponseDto : BaseDto
{
    public string Uid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Status { get; set; }
}
