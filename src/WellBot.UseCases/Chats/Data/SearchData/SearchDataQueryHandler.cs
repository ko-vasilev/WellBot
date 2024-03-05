using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Data.SearchData;

/// <summary>
/// Handler for <see cref="SearchDataQuery"/>.
/// </summary>
internal class SearchDataQueryHandler : IRequestHandler<SearchDataQuery, IEnumerable<DataItem>>
{
    private readonly IAppDbContext dbContext;
    private readonly IMapper mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    public SearchDataQueryHandler(IAppDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DataItem>> Handle(SearchDataQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.SearchText))
        {
            return Enumerable.Empty<DataItem>();
        }

        var searchString = $"%{request.SearchText}%";
        var dataQuery = dbContext.ChatDatas
            .Where(d => EF.Functions.Like(d.Key, searchString) || EF.Functions.Like(d.Text, searchString))
            .Take(request.Limit);

        var items = await mapper.ProjectTo<DataItem>(dataQuery)
            .ToListAsync(cancellationToken);
        return items;
    }
}
