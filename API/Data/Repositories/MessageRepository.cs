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
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(m => m.Recipient.Username == messageParams.Username && !m.RecipientDeleted),
            "Outbox" => query.Where(m => m.Sender.Username == messageParams.Username && !m.SenderDeleted),
            _ => query.Where(m =>
                m.Recipient.Username == messageParams.Username && !m.RecipientDeleted && m.DateRead == null)
        };

        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
        return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        var messages = await _context.Messages
            .Include(m => m.Sender).ThenInclude(u => u.Photos)
            .Include(m => m.Recipient).ThenInclude(u => u.Photos)
            .Where(m => (m.Recipient.Username == currentUsername && m.Sender.Username == recipientUsername &&
                         !m.RecipientDeleted) ||
                        (m.Recipient.Username == recipientUsername && m.Sender.Username == currentUsername &&
                         !m.SenderDeleted))
            .OrderBy(m => m.MessageSent)
            .ToListAsync();

        var unreadMessages =
            messages.Where(m => m.DateRead == null && m.Recipient.Username == currentUsername).ToList();
        if (unreadMessages.Any())
        {
            foreach (var unreadMessage in unreadMessages)
            {
                unreadMessage.DateRead = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}