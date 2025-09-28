using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface INotificationService
    {
        byte[] Get(Guid id);
    }
}