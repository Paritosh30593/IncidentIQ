using System;

namespace IC.Domain.Common
{
    public interface IHasCreatedAt
    {
        DateTime CreatedAt { get; set; }
    }
}
