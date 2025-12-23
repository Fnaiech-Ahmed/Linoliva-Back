using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetNotificationsAsync();
        Task<Notification> GetNotificationAsync(int id);
        Task<int> CreateNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(int id, Notification notification);
        Task DeleteNotificationAsync(int id);
    }
}
