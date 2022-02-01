using App.models;
using System;
using System.Threading.Tasks;

namespace App.services.interfaces
{
    public interface IUserService
    {
        string Test();
        Task<ChatUser> GetById(Guid userId);
    }
}