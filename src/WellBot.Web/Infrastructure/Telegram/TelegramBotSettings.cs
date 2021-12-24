﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.Web.Infrastructure.Telegram
{
    /// <summary>
    /// Contains bot-specific settings.
    /// </summary>
    public class TelegramBotSettings : ITelegramBotSettings
    {
        /// <inheritdoc />
        public string TelegramBotUsername { get; set; }

        /// <inheritdoc />
        public int RegularPassiveRepliesProbability { get; init; }
    }
}
