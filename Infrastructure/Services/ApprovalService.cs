using Application.Dtos;
using Application.IServices;
using Core.Entities;
using Core.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly ApplicationDbContext _dbContext;
        public ApprovalService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    public async Task<bool> ApproveAsync(ApproveInstanceRequestDto dto)
        {
            var instance = await _dbContext.ApprovalRequests
                 .Include(x => x.ApprovalFeature).ThenInclude(f => f.ApprovalSteps)
                 .Include(x => x.ApprovalHistories)
                 .FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (instance == null) throw new Exception("Approval instance not found");

            var currentStep = instance.ApprovalFeature.ApprovalSteps.FirstOrDefault(x => x.StepOrder == instance.CurrentStepOrder);
            if (currentStep == null) throw new Exception("Step not found");

            var user = await _dbContext.Users.FindAsync(dto.UserId);

            if (user == null) throw new Exception("User not found");

            if (!UserHasApprovePermission(user, currentStep)) throw new Exception("No permission");

            if (dto.Status == ApprovalStatus.Approved)
            {
                var nextStep = instance.ApprovalFeature.ApprovalSteps.FirstOrDefault(x => x.StepOrder == instance.CurrentStepOrder + 1);
                if (nextStep != null)
                {
                    instance.CurrentStepOrder = nextStep.StepOrder;
                    instance.Status = ApprovalStatus.Pending;
                }
                else
                {
                    instance.Status = ApprovalStatus.Approved;
                }
            }
            else if (dto.Status == ApprovalStatus.Rejected)
            {
                instance.Status = ApprovalStatus.Rejected;
                instance.ReasonRejection = dto.Reason;
            }
            else
            {
                throw new Exception("Invalid status");
            }

            var action = new ApprovalHistory
            {
                ApprovalRequestId = instance.Id,
                StepOrder = currentStep.StepOrder,
                ApproverId = dto.UserId,
                ApprovedAt = DateTime.UtcNow,
                Status = dto.Status,
                Reason = dto.Reason
            };
            _dbContext.ApprovalHistories.Add(action);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private bool UserHasApprovePermission(ApplicationUser user, ApprovalStep step)
        {
            if (step.TargetType == ApprovalTargetType.User)
            {
                return user.Id == step.TargetId;
            }
            else if (step.TargetType == ApprovalTargetType.Position)
            {
                return user.PositionId == step.TargetId;
            }
            else if (step.TargetType == ApprovalTargetType.Department)
            {
                return user.DepartmentId == step.TargetId;
            }
            return false;
        }
    }
}