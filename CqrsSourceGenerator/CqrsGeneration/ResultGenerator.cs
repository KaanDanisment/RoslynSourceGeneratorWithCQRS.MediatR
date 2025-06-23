using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace CqrsSourceGenerator.CqrsGeneration
{
    public static class ResultGenerator
    {
        public static SourceText GenerateQueryResult(string entityName, INamedTypeSymbol entitySembol)
        {
            var namespaceName = $"CqrsDemo.Api.Features.Results.{entityName}Results";

            var propertiesBuilder = new StringBuilder();
            foreach (var property in entitySembol.GetMembers().OfType<IPropertySymbol>())
            {
                if (IsSimpleType(property.Type))
                {
                    var typeName = property.Type.ToDisplayString();
                    propertiesBuilder.AppendLine($"        public {typeName} {property.Name} {{ get; set; }}");
                }
            }

            var sourece = $@"

namespace {namespaceName}
{{
    public class Get{entityName}QueryResult
    {{
        {propertiesBuilder}
    }}
}}";
            return SourceText.From(sourece, Encoding.UTF8);
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
    }
}
