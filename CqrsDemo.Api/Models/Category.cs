using CqrsAttributes;

namespace CqrsDemo.Api.Models
{
    [GenerateCqrs(true)]
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
