using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Models.Email;

namespace SupervisorMobility.API.Services.EmailService
{
    public interface IEmailDeliveryResultService
    {
        Task<EmailDeliveryResultDto> SaveEmailResultAsync(CreateEmailDeliveryResultDto createDto);
        // Task<EmailDeliveryResultDto> UpdateEmailResultAsync(int id, UpdateEmailDeliveryResultDto updateDto);
        // Task<EmailDeliveryResultDto?> GetEmailResultByIdAsync(int id);
        // Task<IEnumerable<EmailDeliveryResultDto>> GetEmailResultsByUserAsync(int userId);
        // Task<IEnumerable<EmailDeliveryResultDto>> GetEmailResultsByStatusAsync(string status);
        // Task<IEnumerable<EmailDeliveryResultDto>> GetEmailResultsByEntityAsync(string entityType, int entityId);
        // Task<IEnumerable<EmailDeliveryResultDto>> GetEmailResultsByPIRAsync(int pirId);
        // Task<IEnumerable<EmailDeliveryResultDto>> GetFailedEmailsForRetryAsync();
        // Task<bool> MarkAsReadAsync(int id);
        // Task<bool> IncrementRetryAttemptsAsync(int id, DateTime? nextRetryDateTime = null);
    }

    public class EmailDeliveryResultService : IEmailDeliveryResultService
    {
        
        private readonly ISupervisorMobilityRepository _repository;
        private readonly IMapper _mapper;

        public EmailDeliveryResultService(
            ISupervisorMobilityRepository repository,   
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EmailDeliveryResultDto> SaveEmailResultAsync(CreateEmailDeliveryResultDto createDto)
        {
            var emailResult = _mapper.Map<EmailDeliveryResult>(createDto);
            emailResult.SentDateTime = DateTime.Now;

            if (emailResult.IsDelivered && emailResult.DeliveryDateTime == null)
            {
                emailResult.DeliveryDateTime = DateTime.Now;
            }

            var resultEmail = await _repository.AddEmailDeliveryResultAsync(emailResult);
            if(resultEmail == null)
            {
                throw new InvalidOperationException("Failed to save email delivery result");
            }
            
            return await GetEmailResultByIdAsync(resultEmail.EmailDeliveryResultID) 
                ?? throw new InvalidOperationException("Failed to retrieve saved email result");
        }

    //     public async Task<EmailDeliveryResultDto> UpdateEmailResultAsync(int id, UpdateEmailDeliveryResultDto updateDto)
    //     {
    //         var existingEmail = await _context.EmailDeliveryResults
    //             .FirstOrDefaultAsync(e => e.EmailDeliveryResultID == id);

    //         if (existingEmail == null)
    //         {
    //             throw new ArgumentException($"Email delivery result with ID {id} not found");
    //         }

    //         _mapper.Map(updateDto, existingEmail);

    //         if (updateDto.IsDelivered == true && existingEmail.DeliveryDateTime == null)
    //         {
    //             existingEmail.DeliveryDateTime = DateTime.Now;
    //         }

    //         if (updateDto.IsRead == true && existingEmail.ReadDateTime == null)
    //         {
    //             existingEmail.ReadDateTime = DateTime.Now;
    //         }

    //         await _context.SaveChangesAsync();

    //         return await GetEmailResultByIdAsync(id) 
    //             ?? throw new InvalidOperationException("Failed to retrieve updated email result");
    //     }

        public async Task<EmailDeliveryResultDto?> GetEmailResultByIdAsync(int id)
        {
            var emailResult = _repository.GetEmailDeliveryResultByIdAsync(id).Result;
            return emailResult == null ? null : _mapper.Map<EmailDeliveryResultDto>(emailResult);
        }

    //     public async Task<IEnumerable<EmailDeliveryResultDto>> GetEmailResultsByUserAsync(int userId)
    //     {
    //         var emailResults = await _context.EmailDeliveryResults
    //             .Include(e => e.SentByUser)
    //             .Where(e => e.SentByUserID == userId)
    //             .OrderByDescending(e => e.SentDateTime)
    //             .ToListAsync();

    //         return _mapper.Map<IEnumerable<EmailDeliveryResultDto>>(emailResults);
    //     }

    //     public async Task<IEnumerable<EmailDeliveryResultDto>> GetEmailResultsByStatusAsync(string status)
    //     {
    //         var emailResults = await _context.EmailDeliveryResults
    //             .Include(e => e.SentByUser)
    //             .Where(e => e.DeliveryStatus.ToLower() == status.ToLower())
    //             .OrderByDescending(e => e.SentDateTime)
    //             .ToListAsync();

    //         return _mapper.Map<IEnumerable<EmailDeliveryResultDto>>(emailResults);
    //     }

    //     public async Task<IEnumerable<EmailDeliveryResultDto>> GetEmailResultsByEntityAsync(string entityType, int entityId)
    //     {
    //         var emailResults = await _context.EmailDeliveryResults
    //             .Include(e => e.SentByUser)
    //             .Where(e => e.ReferenceEntity != null && 
    //                        e.ReferenceEntity.ToLower() == entityType.ToLower() &&
    //                        e.ReferenceEntityID == entityId)
    //             .OrderByDescending(e => e.SentDateTime)
    //             .ToListAsync();

    //         return _mapper.Map<IEnumerable<EmailDeliveryResultDto>>(emailResults);
    //     }

    //     public async Task<IEnumerable<EmailDeliveryResultDto>> GetEmailResultsByPIRAsync(int pirId)
    //     {
    //         var emailResults = await _context.EmailDeliveryResults
    //             .Include(e => e.SentByUser)
    //             .Where(e => e.ReferenceEntityID == pirId)
    //             .OrderByDescending(e => e.SentDateTime)
    //             .ToListAsync();

    //         return _mapper.Map<IEnumerable<EmailDeliveryResultDto>>(emailResults);
    //     }

    //     public async Task<IEnumerable<EmailDeliveryResultDto>> GetFailedEmailsForRetryAsync()
    //     {
    //         var failedEmails = await _context.EmailDeliveryResults
    //             .Include(e => e.SentByUser)
    //             .Where(e => !e.IsDelivered && 
    //                        e.DeliveryStatus.ToLower() == "failed" &&
    //                        (e.NextRetryDateTime == null || e.NextRetryDateTime <= DateTime.Now) &&
    //                        (e.RetryAttempts == null || e.RetryAttempts < 3))
    //             .OrderBy(e => e.SentDateTime)
    //             .ToListAsync();

    //         return _mapper.Map<IEnumerable<EmailDeliveryResultDto>>(failedEmails);
    //     }

    //     public async Task<bool> MarkAsReadAsync(int id)
    //     {
    //         var emailResult = await _context.EmailDeliveryResults
    //             .FirstOrDefaultAsync(e => e.EmailDeliveryResultID == id);

    //         if (emailResult == null)
    //             return false;

    //         emailResult.IsRead = true;
    //         emailResult.ReadDateTime = DateTime.Now;

    //         await _context.SaveChangesAsync();
    //         return true;
    //     }

    //     public async Task<bool> IncrementRetryAttemptsAsync(int id, DateTime? nextRetryDateTime = null)
    //     {
    //         var emailResult = await _context.EmailDeliveryResults
    //             .FirstOrDefaultAsync(e => e.EmailDeliveryResultID == id);

    //         if (emailResult == null)
    //             return false;

    //         emailResult.RetryAttempts = (emailResult.RetryAttempts ?? 0) + 1;
    //         emailResult.NextRetryDateTime = nextRetryDateTime ?? DateTime.Now.AddHours(1);

    //         await _context.SaveChangesAsync();
    //         return true;
    //     }
    }
}