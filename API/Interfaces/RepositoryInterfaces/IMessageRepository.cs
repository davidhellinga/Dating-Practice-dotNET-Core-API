using API.DTOs;
using API.Entities;
using API.Helpers.PaginationHelpers;
using API.Helpers.PaginationHelpers.Params;

namespace API.Interfaces.RepositoryInterfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessage(int id);
    Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
    Task<bool> SaveAllAsync();
}