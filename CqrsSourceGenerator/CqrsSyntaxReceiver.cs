using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CqrsSourceGenerator
{
    /// <summary>
    /// This class enables Roslyn to inspect syntax nodes during compilation.
    /// Its purpose is to collect all classes annotated with [GenerateCqrs] for code generation.
    /// It is registered within the Initialize method of the ISourceGenerator implementation.
    /// </summary>
    public class CqrsSyntaxReceiver : ISyntaxReceiver
    {
        // This list stores class declarations that are marked with the [GenerateCqrs] attribute.
        // These are later processed in the Execute method to generate source code.
        public List<ClassDeclarationSyntax> Candidates { get; } = new List<ClassDeclarationSyntax>();


        // This method is called for every syntax node during traversal.
        // If the node is a class declaration and contains an attribute with "GenerateCqrs" in its name,
        // it is added to the Candidates list.
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax cls && cls.AttributeLists
                .SelectMany(al => al.Attributes).Any(attr => attr.Name.ToString().Contains("GenerateCqrs")))
            {
                Candidates.Add(cls);
            }
        }
    }
}
