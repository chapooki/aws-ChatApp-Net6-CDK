using App.Models;
using System;
using System.Threading.Tasks;

namespace App.Services.Interfaces
{
    public interface IRoomService
    {
        Task<ChatRoom> GetRoom(Guid roomId);
    }
}