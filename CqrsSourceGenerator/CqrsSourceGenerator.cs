using CqrsSourceGenerator.CqrsGeneration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CqrsSourceGenerator
{
    /// <summary>
    /// A Roslyn Source Generator that scans for classes marked with [GenerateCqrs] and generates 
    /// CQRS components (Commands, Queries, Results) at compile time. If the attribute is used with 
    /// 'true' (i.e., [GenerateCqrs(true)]), it also generates the corresponding MediatR handlers.
    /// </summary>

    [Generator]
    public class CqrsSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            /// <summary>
            /// Registers a syntax receiver that collects candidate class declarations for CQRS code generation.
            /// This receiver will run during the compilation and identify classes annotated with [GenerateCqrs].
            /// </summary>
             
            context.RegisterForSyntaxNotifications(() => new CqrsSyntaxReceiver()); // CqrsSyntaxReceiver is our SyntaxReceiver class that implements ISyntaxReceiver
        }

        public void Execute(GeneratorExecutionContext context)
        {
            /// <summary>
            /// Executes during compilation to generate CQRS-related source code for each class
            /// marked with the [GenerateCqrs] attribute. It generates Commands, Queries, Results,
            /// and optionally Handlers if requested via the attribute's constructor parameter.
            /// </summary>

            // Check if the syntax receiver is of the expected type and contains candidates
            if (!(context.SyntaxReceiver is CqrsSyntaxReceiver receiver)) return;

            foreach (var classDec in receiver.Candidates)
            {
                var model = context.Compilation.GetSemanticModel(classDec.SyntaxTree);
                // We obtain the SemanticModel of the candidate class to access its symbol information (such as its type, members, attributes, etc.).

                var symbol = model.GetDeclaredSymbol(classDec) as INamedTypeSymbol;
                // GetDeclaredSymbol retrieves the INamedTypeSymbol representing the class declaration.
                // This allows us to access the class's name, properties, attributes, and other metadata.

                if (symbol == null) continue; // If the symbol is null, we skip this class as it does not represent a valid type.

                var entityName = symbol.Name; // The name of the class is used to generate the CQRS components, so we store it in a variable.

                var generateAttr = symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "GenerateCqrsAttribute");
                if(generateAttr == null) continue;
                // This line checks if the class has the GenerateCqrsAttribute among its attributes.
                // Although CqrsSyntaxReceiver already filters and adds only those classes with [GenerateCqrs] to the Candidates list,
                // we perform this check again here as a safety measure to ensure robustness and prevent edge-case failures.


                // Generate Commands
                context.AddSource($"Create{entityName}Command.g.cs", CommandGenerator.GenerateCreateCommand(entityName, symbol));
                context.AddSource($"Update{entityName}Command.g.cs", CommandGenerator.GenerateUpdateCommand(entityName, symbol));
                context.AddSource($"Remove{entityName}Command.g.cs", CommandGenerator.GenerateRemoveCommand(entityName,symbol));

                // Generate Queries
                context.AddSource($"Get{entityName}ByIdQuery.g.cs", QueryGenerator.GenerateGetByIdQuery(entityName,symbol));
                context.AddSource($"GetAll{entityName}sQuery.g.cs", QueryGenerator.GenerateGetAllQuery(entityName));

                // Generate Result
                context.AddSource($"Get{entityName}QueryResult.g.cs", ResultGenerator.GenerateQueryResult(entityName, symbol));

                bool generateHandlers = false;
                if (generateAttr.ConstructorArguments.Length == 1)
                {
                    generateHandlers = generateAttr.ConstructorArguments[0].Value as bool? ?? false;
                }
                // If the attribute's constructor has one argument and it's a boolean,
                // we use its value to determine whether handlers should be generated.
                // So, when [GenerateCqrs(true)] is used, it indicates that handlers should be created.
                // If the parameter is not provided or cannot be read, the default value is false.

                if (generateHandlers)
                {
                    // Generate Command Handlers
                    context.AddSource($"Create{entityName}CommandHandler.g.cs", HandlerGenerator.GenerateCreateCommandHandler(entityName));
                    context.AddSource($"Update{entityName}CommandHandler.g.cs", HandlerGenerator.GenerateUpdateCommandHandler(entityName));
                    context.AddSource($"Remove{entityName}CommandHandler.g.cs", HandlerGenerator.GenerateRemoveCommandHandler(entityName));

                    // Generate Query Handlers
                    context.AddSource($"Get{entityName}ByIdQueryHandler.g.cs", HandlerGenerator.GenerateGetByIdQueryHandler(entityName));
                    context.AddSource($"GetAll{entityName}sQueryHandler.g.cs", HandlerGenerator.GenerateGetAllQueryHandler(entityName));

                }
            }
        }
    }
}
