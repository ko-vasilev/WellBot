using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WellBot.Domain.Chats.Entities;
using WellBot.Domain.Users.Entities;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.Infrastructure.DataAccess
{
    /// <summary>
    /// Application unit of work.
    /// </summary>
    public class AppDbContext : IdentityDbContext<User, AppIdentityRole, int>, IAppDbContext
    {
        #region Chats

        /// <inheritdoc />
        public DbSet<Chat> Chats { get; protected set; }

        /// <inheritdoc />
        public DbSet<PidorRegistration> PidorRegistrations { get; protected set; }

        /// <inheritdoc />
        public DbSet<ChatPidor> ChatPidors { get; protected set; }

        /// <inheritdoc />
        public DbSet<PidorMessage> PidorResultMessages { get; protected set; }

        /// <inheritdoc />
        public DbSet<ChatData> ChatDatas { get; protected set; }

        /// <inheritdoc />
        public DbSet<SlapOption> SlapOptions { get; protected set; }

        /// <inheritdoc />
        public DbSet<PassiveReplyOption> PassiveReplyOptions { get; protected set; }

        /// <inheritdoc />
        public DbSet<MemeChannelInfo> MemeChannels { get; protected set; }

        /// <inheritdoc />
        public DbSet<PassiveTopic> PassiveTopics { get; protected set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="Microsoft.EntityFrameworkCore.DbContext" />.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            RestrictCascadeDelete(modelBuilder);
            ForceHavingAllStringsAsVarchars(modelBuilder);

            // Indexes.
            modelBuilder.Entity<Chat>()
                .HasIndex(c => c.TelegramId);

            modelBuilder.Entity<PidorRegistration>()
                .HasOne(reg => reg.Chat)
                .WithMany(ch => ch.PidorRegistrations);
            modelBuilder.Entity<PidorRegistration>()
                .HasIndex(r => r.TelegramUserId);

            modelBuilder.Entity<ChatPidor>()
                .HasOne(p => p.Chat)
                .WithMany(ch => ch.Pidors);
            modelBuilder.Entity<ChatPidor>()
                .HasOne(p => p.Registration)
                .WithMany(r => r.Wins);

            modelBuilder.Entity<ChatPidor>()
                .HasOne(p => p.UsedMessage)
                .WithMany(m => m.UsedWins);

            modelBuilder.Entity<ChatData>()
                .HasOne(d => d.Chat)
                .WithMany(c => c.Data);
            modelBuilder.Entity<ChatData>()
                .HasIndex(d => d.Key);
        }

        private static void RestrictCascadeDelete(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private static void ForceHavingAllStringsAsVarchars(ModelBuilder modelBuilder)
        {
            var stringColumns = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetProperties())
                .Where(p => p.ClrType == typeof(string));
            foreach (IMutableProperty mutableProperty in stringColumns)
            {
                mutableProperty.SetIsUnicode(false);
            }
        }
    }
}
