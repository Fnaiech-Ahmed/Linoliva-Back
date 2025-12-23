using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class NotificationService:INotificationService
    {
        private readonly MyDbContext _context;

        public NotificationService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetNotificationsAsync()
        {
            return await _context.Notifications.ToListAsync();
        }

        public async Task<Notification> GetNotificationAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task<int> CreateNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification.Id;
        }

        public async Task UpdateNotificationAsync(int id, Notification notification)
        {
            var existingNotification = await _context.Notifications.FindAsync(id);

            if (existingNotification == null)
            {
                throw new ArgumentException("Notification not found.");
            }

            existingNotification.Id = notification.Id;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
            {
                throw new ArgumentException("Notification not found.");
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }

    }
}
