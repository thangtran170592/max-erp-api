using System.Text;
using Application.IServices;

namespace Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        public byte[] Get(Guid id)
        {
            var message = $"data: Thông báo cho #{id} lúc {DateTime.Now}\n\n";

            var bytes = Encoding.UTF8.GetBytes(message);
            return bytes;
        }
    }
}