using Ceng382_25_26_202311031.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Ceng382_25_26_202311031.Hubs
{
    [Authorize]
    public class OrderCallHub : Hub
    {
        private readonly OrderAccessService _orderAccessService;
        private readonly LogService _logService;

        public OrderCallHub(
            OrderAccessService orderAccessService,
            LogService logService)
        {
            _orderAccessService = orderAccessService;
            _logService = logService;
        }

        public async Task JoinOrderCall(int orderId)
        {
            await EnsureOrderAccess(orderId);

            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(orderId));

            await _logService.LogAsync(
                "Live Call",
                Context.User?.Identity?.Name,
                $"A participant joined the live call for Order #{orderId}.");

            await Clients.OthersInGroup(GroupName(orderId))
                .SendAsync("ParticipantJoined", Context.User?.Identity?.Name ?? "Participant");
        }

        public async Task SendOffer(int orderId, string offer)
        {
            await EnsureOrderAccess(orderId);

            await Clients.OthersInGroup(GroupName(orderId))
                .SendAsync("ReceiveOffer", offer);
        }

        public async Task SendAnswer(int orderId, string answer)
        {
            await EnsureOrderAccess(orderId);

            await Clients.OthersInGroup(GroupName(orderId))
                .SendAsync("ReceiveAnswer", answer);
        }

        public async Task SendIceCandidate(int orderId, string candidate)
        {
            await EnsureOrderAccess(orderId);

            await Clients.OthersInGroup(GroupName(orderId))
                .SendAsync("ReceiveIceCandidate", candidate);
        }

        public async Task LeaveOrderCall(int orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(orderId));

            await _logService.LogAsync(
                "Live Call",
                Context.User?.Identity?.Name,
                $"A participant left the live call for Order #{orderId}.");

            await Clients.OthersInGroup(GroupName(orderId))
                .SendAsync("ParticipantLeft");
        }

        private async Task EnsureOrderAccess(int orderId)
        {
            if (!await _orderAccessService.CanAccessPaidOrderAsync(Context.User!, orderId))
                throw new HubException("This completed order is not available for your account.");
        }

        private static string GroupName(int orderId)
        {
            return $"order-call-{orderId}";
        }
    }
}
