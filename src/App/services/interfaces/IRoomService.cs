using App.models;
using System;
using System.Threading.Tasks;

namespace App.services.interfaces
{
    public interface IRoomService
    {
        Task<ChatRoom> GetRoom(Guid roomId);
    }
}