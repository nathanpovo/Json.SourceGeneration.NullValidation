using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace System.Text.Json.SourceGeneration.NullValidation;

[Generator]
public class NullValidationGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
        // Register a syntax receiver that will be created for each generation pass
        => context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

    public void Execute(GeneratorExecutionContext context)
    {
        // retrieve the populated receiver
        if (context.SyntaxContextReceiver is not SyntaxReceiver receiver)
        {
            return;
        }

        INamedTypeSymbol? attributeSymbol = context.Compilation
            .GetTypeByMetadataName("System.Text.Json.Serialization.ValidateNullabilityAttribute");

        if (attributeSymbol is null)
        {
            return;
        }

        INamedTypeSymbol? notifySymbol =
            context.Compilation.GetTypeByMetadataName("System.Text.Json.Serialization.IJsonOnDeserialized");

        if (notifySymbol is null)
        {
            return;
        }

        IEnumerable<IGrouping<INamedTypeSymbol, IPropertySymbol>> groupings = receiver
            .Properties
            .GroupBy<IPropertySymbol, INamedTypeSymbol>(f => f.ContainingType, SymbolEqualityComparer.Default);

        foreach (IGrouping<INamedTypeSymbol, IPropertySymbol> grouping in groupings)
        {
            string? classSource = ProcessClass(grouping.Key, grouping.ToList(), notifySymbol);

            if (classSource is null)
            {
                continue;
            }

            context.AddSource($"{grouping.Key.Name}_validateJson.cs", SourceText.From(classSource, Encoding.UTF8));
        }
    }

    private static string? ProcessClass(ITypeSymbol classSymbol, List<IPropertySymbol> fields, ISymbol notifySymbol)
    {
        if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
        {
            return null;
        }

        string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

        // begin building the generated source
        string type = classSymbol.IsRecord ? "record" : "class";
        StringBuilder source = new($@"
// <auto-generated/>

namespace {namespaceName}
{{
    internal partial {type} {classSymbol.Name} : {notifySymbol.ToDisplayString()}
    {{
");

        // if the class doesn't implement INotifyPropertyChanged already, add it
        if (!classSymbol.Interfaces.Any(symbol => SymbolEqualityComparer.Default.Equals(notifySymbol, symbol)))
        {
            source.Append($@"
void {notifySymbol.ToDisplayString()}.OnDeserialized() => Validate();
");
        }

        source.Append(@"
void Validate()
{
");

        // create properties for each field
        foreach (IPropertySymbol fieldSymbol in fields)
        {
            ProcessField(source, fieldSymbol);
        }

        source.Append(" }");
        source.Append("} }");
        return source.ToString();
    }

    private static void ProcessField(StringBuilder source, ISymbol fieldSymbol)
    {
        // get the name and type of the field
        string name = fieldSymbol.Name;

        source.Append($@"
if ({name} is null)
{{
    throw new System.InvalidOperationException(""The '{name}' property cannot be 'null'."");
}}
");
    }

    /// <summary>
    /// Created on demand before each generation pass
    /// </summary>
    private class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<IFieldSymbol> Fields { get; } = new();
        public List<IPropertySymbol> Properties { get; } = new();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            // any field with at least one attribute is a candidate for property generation
            if (context.Node is not ClassDeclarationSyntax
            {
                AttributeLists: { Count: > 0 }
            } classDeclarationSyntax)
            {
                return;
            }

            IEnumerable<PropertyDeclarationSyntax> properties = classDeclarationSyntax.Members
                .Select(member => member as PropertyDeclarationSyntax)
                .Where(member => member is not null)
                .Select(member => member!);

            foreach (PropertyDeclarationSyntax declarationSyntax in properties)
            {
                int spanStart = declarationSyntax.SpanStart;

                if (context.SemanticModel.GetDeclaredSymbol(declarationSyntax) is not IPropertySymbol symbol)
                {
                    continue;
                }

                ITypeSymbol typeSymbol = symbol.Type;

                if (typeSymbol.IsReferenceType && typeSymbol.NullableAnnotation.HasFlag(NullableAnnotation.Annotated))
                {
                    continue;
                }

                if (typeSymbol.IsValueType || typeSymbol.IsReadOnly)
                {
                    continue;
                }

                NullableContext nullableContext = context.SemanticModel.GetNullableContext(spanStart);

                if (!nullableContext.HasFlag(NullableContext.Enabled))
                {
                    continue;
                }

                Properties.Add(symbol);
            }
        }
    }
}
