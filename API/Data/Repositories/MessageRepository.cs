using API.DTOs;
using API.Entities;
using API.Helpers.PaginationHelpers.Params;
using API.Helpers.PaginationHelpers;
using API.Interfaces.RepositoryInterfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
        return await _context.Connections.FindAsync(connectionId);
    }

    public async Task<Group> GetGroupForConnection(string connectionId)
    {
        return await _context.Groups
            .Include(c => c.Connections)
            .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }

    public async Task<Group> GetMessageGroup(string groupName)
    {
        return await _context.Groups
            .Include(group => group.Connections)
            .FirstOrDefaultAsync(group => group.Name == groupName);
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages.Include(u => u.Sender).Include(u => u.Recipient)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = _context.Messages
            .OrderByDescending(m => m.MessageSent)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(m => m.RecipientUsername == messageParams.Username && !m.RecipientDeleted),
            "Outbox" => query.Where(m => m.SenderUsername == messageParams.Username && !m.SenderDeleted),
            _ => query.Where(m =>
                m.RecipientUsername== messageParams.Username && !m.RecipientDeleted && m.DateRead == null)
        };
        return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThreadAndMarkAsRead(string currentUsername,
        string recipientUsername)
    {
        var messages = await _context.Messages
            .Where(m => (m.Recipient.UserName == currentUsername && m.Sender.UserName == recipientUsername &&
                         !m.RecipientDeleted) ||
                        (m.Recipient.UserName == recipientUsername && m.Sender.UserName == currentUsername &&
                         !m.SenderDeleted))
            .OrderBy(m => m.MessageSent)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var unreadMessages =
            messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();
        if (unreadMessages.Any())
        {
            foreach (var unreadMessage in unreadMessages)
            {
                unreadMessage.DateRead = DateTime.UtcNow;
            }
        }

        return messages;
    }
}