using System.Collections.Generic;

namespace FetchRateMyProf.Models;

internal interface IPageable<out T>
{
    public bool HasNextPage { get; }
    public string? EndCursor { get; }
    public IEnumerable<T> Items { get; }
}