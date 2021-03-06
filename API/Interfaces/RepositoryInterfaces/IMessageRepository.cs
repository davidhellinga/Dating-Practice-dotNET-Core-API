using API.DTOs;
using API.Entities;
using API.Helpers.PaginationHelpers;
using API.Helpers.PaginationHelpers.Params;

namespace API.Interfaces.RepositoryInterfaces;

public interface IMessageRepository
{
    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection> GetConnection(string connectionId);
    Task<Group> GetGroupForConnection(string connectionId);
    Task<Group> GetMessageGroup(string groupName);
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessage(int id);
    Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessageThreadAndMarkAsRead(string currentUsername, string recipientUsername);
}