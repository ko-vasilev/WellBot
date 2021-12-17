using System;
using System.Collections.Generic;
using MediatR;

namespace WellBot.UseCases.Chats.Data.SearchData
{
    /// <summary>
    /// Search for data.
    /// </summary>
    public record SearchDataQuery : IRequest<IEnumerable<DataItem>>
    {
        /// <summary>
        /// Query to search items by.
        /// </summary>
        public string SearchText { get; init; }

        /// <summary>
        /// Max number of items to retrieve.
        /// </summary>
        public int Limit { get; init; } = 20;
    }
}
