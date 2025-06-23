using System;
using System.Collections.Generic;
using System.Text;

namespace CqrsAttributes
{
    /// <summary>
    /// This attribute is used to mark classes for which the CQRS Source Generator should generate code.
    /// It can only be applied to classes (AttributeTargets.Class).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GenerateCqrsAttribute: Attribute
    {
        public bool GenerateHandlers { get; } = false;// Indicates whether handlers should be generated. Default false.
        public GenerateCqrsAttribute(bool generateHandlers)  // This constructor is used when the attribute is applied like [GenerateCqrs(true)].
        {
            GenerateHandlers = generateHandlers;
        }

        public GenerateCqrsAttribute() // Parameterless constructor for usage like [GenerateCqrs]. GenerateHandlers remains false.
        {
        }
    }
}
