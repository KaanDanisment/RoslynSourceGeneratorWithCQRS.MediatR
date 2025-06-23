using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CqrsSourceGenerator.CqrsGeneration
{
    public static class CommandGenerator
    {
        public static SourceText GenerateCreateCommand(string entityName, INamedTypeSymbol entitySymbol)
        {
            var namespaceName = $"CqrsDemo.Api.Features.Commands.{entityName}Commands";

            var properiesBuilder = new StringBuilder();

            foreach (var property in entitySymbol.GetMembers().OfType<IPropertySymbol>())
            {
                if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)) continue; // Skip Id property
                if (!IsSimpleType(property.Type)) continue; // Skip complex types

                var typeName = property.Type.ToDisplayString();
                properiesBuilder.AppendLine($"    public {typeName} {property.Name} {{ get; set; }}");

            }
            var source = $@"
using System;
using MediatR;

namespace {namespaceName}
{{
    public class Create{entityName}Command : IRequest<Guid>
    {{
        {properiesBuilder}
    }}
}}";

            return SourceText.From(source, Encoding.UTF8);
        }
        public static SourceText GenerateUpdateCommand(string entityName, INamedTypeSymbol entiySymbol)
        {
            var namespaceName = $"CqrsDemo.Api.Features.Commands.{entityName}Commands";
            var propertiesBuilder = new StringBuilder();

            foreach (var property in entiySymbol.GetMembers().OfType<IPropertySymbol>())
            {
                if (!IsSimpleType(property.Type)) continue; // Skip complex types

                var typeName = property.Type.ToDisplayString();
                propertiesBuilder.AppendLine($"    public {typeName} {property.Name} {{ get; set; }}");
            }
            var source = $@"
using MediatR;

namespace {namespaceName}
{{
    public class Update{entityName}Command : IRequest<Unit>
    {{
        {propertiesBuilder}
    }}
}}";
            return SourceText.From(source, Encoding.UTF8);
        }
        private static bool IsSimpleType(ITypeSymbol typeSymbol)
        {
            var named = typeSymbol as INamedTypeSymbol;
            if (named == null) return false;

            return
                named.SpecialType != SpecialType.None ||
                named.OriginalDefinition.ToDisplayString() == "System.Nullable<T>" ||
                named.ToDisplayString() == "System.Guid" ||
                named.ToDisplayString() == "System.DateTime" ||
                named.ToDisplayString() == "System.DateTimeOffset" ||
                named.ToDisplayString() == "System.Decimal";
        }

        public static SourceText GenerateRemoveCommand(string entityName, INamedTypeSymbol entiySymbol)
        {
            var namespaceName = $"CqrsDemo.Api.Features.Commands.{entityName}Commands";
            var propertiesBuilder = new StringBuilder();

            var idPRoperty = entiySymbol.GetMembers()
                .OfType<IPropertySymbol>()
                .FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            var idType = idPRoperty?.Type.ToDisplayString() ?? "int"; // Default to int if not found

            propertiesBuilder.AppendLine($"    public {idType} Id {{ get; set; }}"); // Id property is required for remove
            propertiesBuilder.AppendLine($"    public Remove{entityName}Command({idType} id){{ Id = id;}}");// Constructor to set Id

            var source = $@"
using MediatR;

namespace {namespaceName}
{{
    public class Remove{entityName}Command : IRequest<Unit>
    {{
        {propertiesBuilder}
    }}
}}";
            return SourceText.From(source, Encoding.UTF8);
        }
    }
}
