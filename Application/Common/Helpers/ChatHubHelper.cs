using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Common.Helpers
{
    public static class ChatHubHelper
    {
        public static string GetGroupName(Guid sender1, Guid sender2)
        {
            var sorted = new[] { sender1, sender2 }.OrderBy(x => x).ToArray();
            return $"Private-{sorted[0]}-{sorted[1]}";
        }

        public static string SetRoomCode(Guid hostId, Guid customerId, string groupName, bool isGroup = false)
        {
            var roomCode = string.Empty;
            if (isGroup)
            {
                roomCode = $"Group-{hostId}-{StringHelper.ToFriendlyUrl(groupName)}";
            }
            else
            {
                var sorted = new[] { hostId, customerId }.OrderBy(x => x).ToArray();
                roomCode = $"Private-{sorted[0]}-{sorted[1]}";
            }
            return roomCode;
        }
    }
}