# CQRS Source Generator for .NET (MediatR + Roslyn)

This project is a Roslyn Source Generator that automatically generates CQRS (Command Query Responsibility Segregation) classes (Commands, Queries, Results, and optionally Handlers) for your entities, compatible with MediatR.

It helps you eliminate repetitive boilerplate code by simply annotating your entity with a [GenerateCqrs] attribute.

## Features

- Generates:

  - Create, Update, Delete Commands

  - GetById, GetList Queries

  - Result DTO (based on GetById)

  - Corresponding Handlers for each request (only with [GenerateCqrs(true)])

- Clean folder and namespace structure for each CQRS class

- Separate .g.cs files for better IDE navigation

## Installation

1. Clone this repository:

         git clone https://github.com/KaanDanisment/RoslynSourceGeneratorWithCQRS.MediatR
2. Add a reference to the generator project in your main application (.csproj):
   
        <ItemGroup>
        <ProjectReference Include="..\CqrsSourceGenerator\CqrsSourceGenerator.csproj"
                          OutputItemType="Analyzer"
                          ReferenceOutputAssembly="false" />
        </ItemGroup>

## How to Use

### 1. Add the GenerateCqrs Attribute

You can use the attribute in two ways depending on whether you want handler classes generated:

[GenerateCqrs] → Generates only Commands, Queries, and Results.

[GenerateCqrs(true)] → Generates Commands, Queries, Results, and Handlers.

#### Example without handlers:

    using CqrsSourceGenerator.Attributes;
    
    [GenerateCqrs]
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

#### Example with handlers:

    using CqrsSourceGenerator.Attributes;
    
    [GenerateCqrs(true)]
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

### 2. Build the Project

After build, the following will be auto-generated:

    /Features
      /Commands/ProductCommands
        CreateProductCommand.g.cs
        UpdateProductCommand.g.cs
        DeleteProductCommand.g.cs
      /Queries/ProductQueries
        GetProductByIdQuery.g.cs
        GetProductListQuery.g.cs
      /Results/ProductResults
        GetProductByIdQueryResult.g.cs
      /Handlers/ProductHandlers
        CreateProductCommandHandler.g.cs
        ... (only if [GenerateCqrs(true)] is used)

### 3. Use with MediatR

In your controller or service:

    await _mediator.Send(new CreateProductCommand { Name = "Laptop"});

## Requirements

- .NET 8.0 or later

- MediatR (add manually if not already):

      dotnet add package MediatR
  
## How It Works

This generator uses Roslyn to:

- Scan for entities marked with [GenerateCqrs]
- Create request/response classes at compile time
- Optionally generate handler classes if [GenerateCqrs(true)] is used
- Avoid manual coding of repetitive CQRS scaffolding

The following structure is used for generated code:

| Type     | Namespace                            |
| -------- | ------------------------------------ |
| Commands | `Features.Commands.{Entity}Commands` |
| Queries  | `Features.Queries.{Entity}Queries`   |
| Results  | `Features.Results.{Entity}Results`   |
| Handlers | `Features.Handlers.{Entity}Handlers` |


## License

Apache License 2.0 
