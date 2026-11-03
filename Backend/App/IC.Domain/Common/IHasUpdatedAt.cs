using System;

namespace IC.Domain.Common
{
    public interface IHasUpdatedAt
    {
        DateTime UpdatedAt { get; set; }
    }
}
