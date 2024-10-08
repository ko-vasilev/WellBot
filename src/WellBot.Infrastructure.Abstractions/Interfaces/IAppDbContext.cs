﻿using Microsoft.EntityFrameworkCore;
using WellBot.Domain.Chats;
using WellBot.Domain.Users;

namespace WellBot.Infrastructure.Abstractions.Interfaces;

/// <summary>
/// Application abstraction for unit of work.
/// </summary>
public interface IAppDbContext : IDbContextWithSets, IDisposable
{
    #region Users

    /// <summary>
    /// Users.
    /// </summary>
    DbSet<User> Users { get; }

    #endregion

    #region Chats

    /// <summary>
    /// Chats set.
    /// </summary>
    DbSet<Chat> Chats { get; }

    /// <summary>
    /// List of registrations in Daily Pidor game.
    /// </summary>
    DbSet<PidorRegistration> PidorRegistrations { get; }

    /// <summary>
    /// List of daily pidor game winners.
    /// </summary>
    DbSet<ChatPidor> ChatPidors { get; }

    /// <summary>
    /// Lis tof messages that can be used to notify about the game results.
    /// </summary>
    DbSet<PidorMessage> PidorResultMessages { get; }

    /// <summary>
    /// List of saved chat key data.
    /// </summary>
    DbSet<ChatData> ChatDatas { get; }

    /// <summary>
    /// Available options for Slap command reply.
    /// </summary>
    DbSet<SlapOption> SlapOptions { get; }

    /// <summary>
    /// Available options for passive replies in chat.
    /// </summary>
    DbSet<PassiveReplyOption> PassiveReplyOptions { get; }

    /// <summary>
    /// Information about meme channels.
    /// </summary>
    DbSet<MemeChannelInfo> MemeChannels { get; }

    /// <summary>
    /// List of topics that could trigger the conversation with bot.
    /// </summary>
    DbSet<PassiveTopic> PassiveTopics { get; }

    /// <summary>
    /// Message templates that should be sent automatically.
    /// </summary>
    DbSet<AutomaticMessageTemplate> AutomaticMessageTemplates { get; }

    /// <summary>
    /// History of message or events that happen in a chat.
    /// </summary>
    DbSet<MessageLog> MessageLogs { get; }

    /// <summary>
    /// Collection of users who opted out from message log in particular chats.
    /// </summary>
    DbSet<MessageLogOptOut> MessageLogOptOuts { get; }

    #endregion
}
