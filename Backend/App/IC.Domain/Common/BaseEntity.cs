using System.ComponentModel.DataAnnotations;

namespace IC.Domain.Common
{
    public class BaseEntity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
