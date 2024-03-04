using MediatR;
using Telegram.Bot;

namespace WellBot.UseCases.Chats.Pidor.PidorRules;

/// <summary>
/// Handler for <see cref="PidorRulesCommand"/>.
/// </summary>
internal class PidorRulesCommandHandler : AsyncRequestHandler<PidorRulesCommand>
{
    private readonly ITelegramBotClient botClient;

    /// <summary>
    /// Constructor.
    /// </summary>
    public PidorRulesCommandHandler(ITelegramBotClient botClient)
    {
        this.botClient = botClient;
    }

    /// <inheritdoc/>
    protected override async Task Handle(PidorRulesCommand request, CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(request.ChatId, @"Правила игры *Пидор Дня* (только для групповых чатов):

*1*. Зарегистрируйтесь в игру по команде */pidoreg*
*2*. Подождите пока зарегиструются все (или большинство :)
*3*. Запустите розыгрыш по команде */pidor*
*4*. Просмотр статистики канала по команде */pidorstats*, */pidorstats all*
*5*. *(!!! Только для администраторов чатов)*: удалить из игры может только Админ канала, сначала выведя по команде список игроков: /pidorlist
Удалить же игрока можно по команде /pidorlist del @username

*Важно*, розыгрыш проходит только *раз в день*, повторная команда выведет *результат* игры.
Сброс розыгрыша происходит каждый день в 12 часов ночи по UTC+3 (полночь по Москве).",
Telegram.Bot.Types.Enums.ParseMode.Markdown,
cancellationToken: cancellationToken);
    }
}
