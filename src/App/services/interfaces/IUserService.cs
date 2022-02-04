using App.Models;
using System;
using System.Threading.Tasks;

namespace App.Services.Interfaces
{
    public interface IUserService
    {
        Task<ChatUser> GetById(Guid userId);
    }
}