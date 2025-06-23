using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CqrsSourceGenerator.CqrsGeneration
{
    public static class QueryGenerator
    {
        public static SourceText GenerateGetByIdQuery(string entityName, INamedTypeSymbol entitySymbol)
        {
            var namespaceName = $"CqrsDemo.Api.Features.Queries.{entityName}Queries";
            var idProperty = entitySymbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));

            var idType = idProperty?.Type.ToDisplayString() ?? "int";

            var source = $@"
using MediatR;
using CqrsDemo.Api.Features.Results.{entityName}Results;
using System.Collections.Generic;

namespace {namespaceName}
{{
    public class Get{entityName}ByIdQuery : IRequest<Get{entityName}QueryResult>
    {{
        public {idType} Id {{ get; set; }}
        
        public Get{entityName}ByIdQuery({idType} id)
        {{
            Id = id;
        }}
    }}
}}";
            return SourceText.From(source, Encoding.UTF8);
        }

        public static SourceText GenerateGetAllQuery(string entityName)
        {
            var namespaceName = $"CqrsDemo.Api.Features.Queries.{entityName}Queries";
            var source = $@"
using MediatR;
using CqrsDemo.Api.Features.Results.{entityName}Results;

namespace {namespaceName}
{{
    public class GetAll{entityName}sQuery : IRequest<IEnumerable<Get{entityName}QueryResult>>
    {{
    }}

}}";
            return SourceText.From(source, Encoding.UTF8);
        }
    }
}
