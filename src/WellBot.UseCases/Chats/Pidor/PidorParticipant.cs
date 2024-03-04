using Telegram.Bot.Types;
using WellBot.Domain.Chats;

namespace WellBot.UseCases.Chats.Pidor
{
    /// <summary>
    /// Contains info about a user participating in the pidor of the day game.
    /// </summary>
    public class PidorParticipant
    {
        /// <summary>
        /// Info about a telegram user.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Info about the registration.
        /// </summary>
        public PidorRegistration PidorRegistration { get; set; }
    }
}
