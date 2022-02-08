using App.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Services.Interfaces
{
    public interface IUserService
    {
        Task<ChatUser> GetById(Guid userId);
        Task<List<ChatUser>> GetByIdsList(List<Guid> userIds);
        Task AddToRoom(Guid userId, Guid roomId);
    }
}