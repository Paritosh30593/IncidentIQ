using System;
using System.ComponentModel.DataAnnotations;


namespace IC.Domain.Common
{
    public class BaseEntity<T>
    {
        [Key]
        public T Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}