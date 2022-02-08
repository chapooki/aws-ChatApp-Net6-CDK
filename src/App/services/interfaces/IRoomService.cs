using App.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Services.Interfaces
{
    public interface IRoomService
    {
        Task<ChatRoom> GetById(Guid roomId);
        Task<List<ChatRoom>> GetByIdsList(List<Guid> roomIds);
        Task<Guid> Create(NewChatRoom room);
        Task ReceiveANewMessage(ChatMessage message);
    }
}