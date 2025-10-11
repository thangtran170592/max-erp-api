using Application.Dtos;
using Application.IServices;
using Core.Entities;
using Core.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Helpers
{
    public static class ApprovalHandleHelper
    {
        public static async Task InitializeApprovalProcess(ApplicationDbContext dbContext, InitializeApprovalRequestDto request)
        {
            // Lay chuc nang duyet theo cau hinh
            var feature = await dbContext.ApprovalFeatures
            .Include(x => x.ApprovalSteps)
            .FirstAsync(x => x.Id == request.ApprovalFeatureId && x.Status);

            if (feature == null) throw new Exception("Approval feature not found");

            // Tao ban ghi document
            var existedDocument = await dbContext.ApprovalDocuments.AnyAsync(x => x.ApprovalFeatureId == request.ApprovalFeatureId && x.DocumentId == request.DocumentId && x.ApprovalStatus != ApprovalStatus.Rejected);

            if (existedDocument)
            {
                throw new Exception("An active approval document already exists for this feature and data item");
            }

            var document = new ApprovalDocument
            {
                Id = Guid.NewGuid(),
                ApprovalFeatureId = request.ApprovalFeatureId,
                DocumentId = request.DocumentId,
                CurrentStepOrder = 1,
                ApprovalStatus = ApprovalStatus.Pending,
                Editable = true,
                CreatedBy = request.CreatedBy,
            };

            // Tao lich su duyet
            var firstStep = feature.ApprovalSteps.OrderBy(x => x.StepOrder).FirstOrDefault();
            if (firstStep == null)
            {
                throw new Exception("Have no step here");
            }

            var history = new ApprovalHistory
            {
                Id = Guid.NewGuid(),
                ApprovalDocument = document,
                StepOrder = firstStep.StepOrder,
                ApproverId = firstStep.TargetId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy,
                SubmittedAt = DateTime.UtcNow,
                SubmittedBy = request.CreatedBy,
                Comment = string.Empty,
                ApprovalStatus = ApprovalStatus.Pending,
            };
            await dbContext.ApprovalDocuments.AddAsync(document);
            await dbContext.ApprovalHistories.AddAsync(history);
            await dbContext.SaveChangesAsync();
        }

        public static async Task ApprovalDocumentHandler(ApplicationDbContext dbContext, ApprovalHandlerRequestDto request)
        {
            // Tim document
            var document = await dbContext.ApprovalDocuments
                .Include(document => document.ApprovalHistories)
                .FirstAsync(document => document.Id == request.DocumentId);

            if (document == null)
            {
                throw new Exception("Document not found!");
            }

            // Tim history cua document
            var documentHistory = document.ApprovalHistories
                .Where(history => history.ApprovalStatus == ApprovalStatus.Pending && history.ApproverId == request.ApproverId)
                .FirstOrDefault();

            if (documentHistory == null)
            {
                throw new Exception("You haven't permission approve");
            }

            // Kiem tra xem con buoc tiep theo khong
            var currentFeature = await dbContext.ApprovalFeatures
                .Include(feature => feature.ApprovalSteps)
                .FirstAsync(feature => feature.Id == document.ApprovalFeatureId && feature.Status);
            if (currentFeature == null)
            {
                throw new Exception("Feature not found!");
            }

            var newDocumentHistory = new ApprovalHistory
            {
                Id = Guid.NewGuid(),
                ApprovalDocument = document,
                CreatedAt = documentHistory.CreatedAt,
                CreatedBy = documentHistory.CreatedBy,
                SubmittedAt = documentHistory.SubmittedAt,
                SubmittedBy = documentHistory.SubmittedBy,
                StepOrder = documentHistory.StepOrder,
                ApproverId = documentHistory.ApproverId,
                ApprovedAt = DateTime.UtcNow,
                Comment = request.Comment,
                ApprovalStatus = ApprovalStatus.Pending,
            };

            var nextStep = currentFeature.ApprovalSteps
                .OrderBy(step => step.StepOrder)
                .First(step => step.StepOrder > documentHistory.StepOrder);

            if (request.ApprovalStatus == ApprovalStatus.Rejected)
            {
                document.ApprovalStatus = ApprovalStatus.Rejected;
                document.Editable = true;
                newDocumentHistory.ApprovalStatus = ApprovalStatus.Rejected;
            }
            else if (request.ApprovalStatus == ApprovalStatus.Approved)
            {
                if (nextStep != null)
                {
                    document.CurrentStepOrder = nextStep.StepOrder;
                    document.ApprovalStatus = ApprovalStatus.Pending;
                    newDocumentHistory.ApprovalStatus = ApprovalStatus.Pending;
                }
                else
                {
                    document.ApprovalStatus = ApprovalStatus.Approved;
                    document.Editable = false;
                    newDocumentHistory.ApprovalStatus = ApprovalStatus.Approved;
                }
            }
            else
            {
                document.ApprovalStatus = ApprovalStatus.Draft;
                document.Editable = true;

                newDocumentHistory.ApprovalStatus = ApprovalStatus.Draft;
                newDocumentHistory.SubmittedAt = null;
                newDocumentHistory.SubmittedBy = null;
                newDocumentHistory.StepOrder = 1;
                newDocumentHistory.ApproverId = null;
                newDocumentHistory.ApprovedAt = null;
            }

            dbContext.ApprovalDocuments.Update(document);
            await dbContext.ApprovalHistories.AddAsync(newDocumentHistory);
        }
    }
}