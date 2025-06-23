using CqrsAttributes;

namespace CqrsDemo.Api.Models
{
    [GenerateCqrs]
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
