using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Microsoft.AspNetCore.Http;

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