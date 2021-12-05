﻿using System;
using Microsoft.EntityFrameworkCore;
using WellBot.Domain.Chats.Entities;
using WellBot.Domain.Users.Entities;

namespace WellBot.Infrastructure.Abstractions.Interfaces
{
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

        #endregion
    }
}
