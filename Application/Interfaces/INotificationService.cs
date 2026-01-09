using Domain.Entities;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task RegisterNotificationAsync(Notification notification);
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId);
    }
}
