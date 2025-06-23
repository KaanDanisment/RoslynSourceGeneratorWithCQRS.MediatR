using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace CqrsSourceGenerator.CqrsGeneration
{
    public static class HandlerGenerator
    {
        public static SourceText GenerateCreateCommandHandler(string entityName)
        {
            var namespaceName = $"CqrsDemo.Api.Features.Handlers.{entityName}Handlers";


            var code = $@"
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CqrsDemo.Api.DataAccess;
using CqrsDemo.Api.Features.Commands.{entityName}Commands;
using CqrsDemo.Api.Models;

namespace {namespaceName}
{{
    public class Create{entityName}CommandHandler : IRequestHandler<Create{entityName}Command, Guid>
    {{
        private readonly ApplicationDbContext _context;
        
        public Create{entityName}CommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<Guid> Handle(Create{entityName}Command request, CancellationToken cancellationToken)
        {{
            try
            {{
                cancellationToken.ThrowIfCancellationRequested();
                var entity = new {entityName}();
                foreach (var entityProp in typeof({entityName}).GetProperties())
                {{
                    if (entityProp.Name.Equals(""Id"", StringComparison.OrdinalIgnoreCase)) continue; // Skip Id property

                    var requestProp = typeof(Create{entityName}Command).GetProperty(entityProp.Name);
                    if (requestProp != null && requestProp.PropertyType == entityProp.PropertyType)
                    {{
                        entityProp.SetValue(entity, requestProp.GetValue(request));
                    }}
                }}
                await _context.Set<{entityName}>().AddAsync(entity, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return entity.Id;
            }}
            catch (Exception ex)
            {{
                throw new ApplicationException($""Error Create{entityName}CommandHandler: "" + ex.Message, ex);
            }}
        }}
    }}
}}
";
            return SourceText.From(code, Encoding.UTF8);
        }

        public static SourceText GenerateUpdateCommandHandler(string entityName)
        {
            var namespaceName = $"CqrsDemo.Api.Features.Handlers.{entityName}Handlers";

            var code = $@"
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CqrsDemo.Api.DataAccess;
using CqrsDemo.Api.Features.Commands.{entityName}Commands;
using CqrsDemo.Api.Models;

namespace {namespaceName}
{{
    public class Update{entityName}CommandHandler: IRequestHandler<Update{entityName}Command,Unit>
    {{
        private readonly ApplicationDbContext _context;
        
        public Update{entityName}CommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<Unit> Handle(Update{entityName}Command request, CancellationToken token)
        {{
            try
            {{
                token.ThrowIfCancellationRequested();
                
                var entity = await _context.Set<{entityName}>().FirstOrDefaultAsync(e => e.Id == request.Id, token);
                if (entity == null)
                {{{{
                    throw new ApplicationException($""No Data with the Id you requested"");
                }}}}

                foreach (var prop in typeof(Update{entityName}Command).GetProperties())
                {{{{
                    if (prop.Name == ""Id"") continue;
                    var entityProp = typeof({entityName}).GetProperty(prop.Name);
                    if (entityProp != null && entityProp.CanWrite)
                    {{{{
                        entityProp.SetValue(entity, prop.GetValue(request));
                    }}}}
                }}}}

                await _context.SaveChangesAsync(token);

                return Unit.Value;
            }}
            catch(Exception ex)
            {{
                throw new ApplicationException($""Error Update{entityName}CommandHandler: "" + ex.Message, ex);    
            }}
        }}
    }}
}}
";
            return SourceText.From(code, Encoding.UTF8);
        }

        public static SourceText GenerateRemoveCommandHandler(string entityName)
        {
            var namespaceName = $"CqrsDemo.Api.Features.Handlers.{entityName}Handlers";

            var code = $@"
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CqrsDemo.Api.DataAccess;
using CqrsDemo.Api.Features.Commands.{entityName}Commands;
using CqrsDemo.Api.Models;

namespace {namespaceName}
{{
    public class Remove{entityName}CommandHandler : IRequestHandler<Remove{entityName}Command, Unit>
    {{
        private readonly ApplicationDbContext _context;

        public Remove{entityName}CommandHandler(ApplicationDbContext context) => _context = context;
        
        public async Task<Unit> Handle(Remove{entityName}Command request, CancellationToken token)
        {{
            try
            {{
                token.ThrowIfCancellationRequested();
                var entity = await _context.Set<{entityName}>().FirstOrDefaultAsync(e => e.Id == request.Id, token);
                if(entity == null)
                {{
                    throw new ApplicationException($""No Data with the Id you requested."");
                }}
                _context.Set<{entityName}>().Remove(entity);
                await _context.SaveChangesAsync(token);

                return Unit.Value;
            }}
            catch(Exception ex)
            {{
                throw new ApplicationException($""Error Remove{entityName}CommandHandler: "" + ex.Message, ex);    
            }}
        }}
    }}
}}
";
            return SourceText.From(code, Encoding.UTF8);
        }

        public static SourceText GenerateGetByIdQueryHandler(string entityName)
        {
            var namespaceName = $"CqrsDemo.Api.Features.Handlers.{entityName}Handlers";
            var code = $@"
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CqrsDemo.Api.DataAccess;
using CqrsDemo.Api.Features.Queries.{entityName}Queries;
using CqrsDemo.Api.Features.Results.{entityName}Results;
using CqrsDemo.Api.Models;

namespace {namespaceName}
{{
    public class Get{entityName}ByIdQueryHandler: IRequestHandler<Get{entityName}ByIdQuery, Get{entityName}QueryResult>
    {{
        private readonly ApplicationDbContext _context;

        public Get{entityName}ByIdQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<Get{entityName}QueryResult> Handle(Get{entityName}ByIdQuery request, CancellationToken cancellationToken)
        {{
            try
            {{
                cancellationToken.ThrowIfCancellationRequested();
                var entity = await _context.Set<{entityName}>().FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
                if (entity == null)
                {{
                    throw new ApplicationException(""No Data with the Id you requested"");
                }}
                
                Get{entityName}QueryResult result = new Get{entityName}QueryResult();
                foreach (var prop in typeof({entityName}).GetProperties())
                {{
                    var resultProp = typeof(Get{entityName}QueryResult).GetProperty(prop.Name);
                    if (resultProp != null && resultProp.CanWrite)
                    {{
                        resultProp.SetValue(result, prop.GetValue(entity));
                    }}
                }}
                return result;
            }}
            catch (Exception ex)
            {{
                throw new ApplicationException($""Error Get{entityName}ByIdQueryHandler: "" + ex.Message, ex);
            }}
        }}
    }}
}}
";
            return SourceText.From(code, Encoding.UTF8);
        }

        public static SourceText GenerateGetAllQueryHandler(string entityName)
        {
                       var namespaceName = $"CqrsDemo.Api.Features.Handlers.{entityName}Handlers";
            var code = $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CqrsDemo.Api.DataAccess;
using CqrsDemo.Api.Features.Queries.{entityName}Queries;
using CqrsDemo.Api.Features.Results.{entityName}Results;
using CqrsDemo.Api.Models;

namespace {namespaceName}
{{
    public class GetAll{entityName}sQueryHandler : IRequestHandler<GetAll{entityName}sQuery, IEnumerable<Get{entityName}QueryResult>>
    {{
        private readonly ApplicationDbContext _context;
        public GetAll{entityName}sQueryHandler(ApplicationDbContext context) => _context = context;
        public async Task<IEnumerable<Get{entityName}QueryResult>> Handle(GetAll{entityName}sQuery request, CancellationToken cancellationToken)
        {{
            try
            {{
                cancellationToken.ThrowIfCancellationRequested();
                var entities = await _context.Set<{entityName}>().AsNoTracking().ToListAsync(cancellationToken);

                var entityProps = typeof({entityName}).GetProperties();
                var resultProps = typeof(Get{entityName}QueryResult).GetProperties().ToDictionary(p => p.Name);

                return entities.Select(e => 
                {{
                    var result = new Get{entityName}QueryResult();
                    foreach (var prop in entityProps)
                    {{
                        
                        if (resultProps.TryGetValue(prop.Name, out var resultProp) && resultProp.CanWrite)
                        {{
                            resultProp.SetValue(result, prop.GetValue(e));
                        }}
                    }}
                    return result;
                }});
            }}
            catch (Exception ex)
            {{
                throw new ApplicationException($""Error GetAll{entityName}sQueryHandler: "" + ex.Message, ex);
            }}
        }}
    }}
}}
";
            return SourceText.From(code, Encoding.UTF8);
        }
    }
}
