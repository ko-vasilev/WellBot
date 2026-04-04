using MediatR;
using Saritasa.Tools.EntityFrameworkCore;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Users;

namespace WellBot.UseCases.Users.GetUserById;

/// <summary>
/// Handler for <see cref="GetUserByIdQuery" />.
/// </summary>
internal class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDetailsDto>
{
    private readonly IAppDbContext dbContext;
    private readonly UserMapper mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="dbContext">Database context.</param>
    /// <param name="mapper">Mapper instance.</param>
    public GetUserByIdQueryHandler(IAppDbContext dbContext, UserMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<UserDetailsDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.GetAsync(u => u.Id == request.UserId, cancellationToken: cancellationToken);
        return mapper.ToUserDetailsDto(user);
    }
}
