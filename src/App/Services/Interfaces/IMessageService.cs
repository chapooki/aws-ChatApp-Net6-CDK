using App.Models;
using System;
using System.Threading.Tasks;

namespace App.Services.Interfaces
{
    public interface IMessageService
    {
        Task<Guid> SendMessage(ChatMessage message);
    }
}
