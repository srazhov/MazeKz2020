using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WebMaze.DbStuff.Model.UserAccount;
using WebMaze.Infrastructure.Enums;
using WebMaze.Models.Messenger;
using WebMaze.Services;

namespace WebMaze.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly MessengerService messengerService;

        private readonly ILogger<ChatHub> logger;

        private readonly IMapper mapper;

        private static readonly ConcurrentDictionary<string, IClientProxy> ConnectedUsers =
            new ConcurrentDictionary<string, IClientProxy>();

        public ChatHub(MessengerService messengerService, ILogger<ChatHub> logger, IMapper mapper)
        {
            this.messengerService = messengerService;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task SendMessage(string recipientLogin, string textMessage)
        {
            var senderLogin = Context.User.Identity.Name;
            await Clients.Caller.SendAsync("ReceiveMessage", senderLogin, textMessage, DateTime.Now.ToString("HH:mm, dd MMM"));
            var recipientConnected = ConnectedUsers.TryGetValue(recipientLogin, out var recipientProxy);
            
            if (recipientConnected)
            {
                await recipientProxy.SendAsync("ReceiveMessage", senderLogin, textMessage, DateTime.Now.ToString());
            }

            messengerService.SendMessage(senderLogin, recipientLogin, textMessage);
        }

        public IEnumerable<MessageViewModel> GetChatHistory(string senderLogin, string recipientLogin)
        {
            var lastMessages = messengerService.GetLastUsersMessages(senderLogin, recipientLogin);
            var messageViewModels = mapper.Map<List<MessageViewModel>>(lastMessages);

            return messageViewModels;
        }

        public override async Task OnConnectedAsync()
        {
            ConnectedUsers.AddOrUpdate(Context.User.Identity.Name, Clients.Caller,
                (key, oldValue) => Clients.Caller);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ConnectedUsers.TryRemove(Context.User.Identity.Name, out _);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
