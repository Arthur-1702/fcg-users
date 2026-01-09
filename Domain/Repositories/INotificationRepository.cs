using Domain.Entities;

namespace Domain.Repositories
{
    public interface INotificationRepository
    {
        Task RegisterNotificationAsync(Notification notification);
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId);
    }
}
