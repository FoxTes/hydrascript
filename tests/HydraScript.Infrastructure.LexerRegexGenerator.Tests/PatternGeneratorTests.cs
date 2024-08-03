using System.Diagnostics;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace HydraScript.Infrastructure.LexerRegexGenerator.Tests;

public class PatternGeneratorTests
{
    [Fact]
    public void Initialize_PatternContainerMarked_CorrectlyGenerated()
    {
        var inputCompilation = CreateCompilation(
            """
            using System.Text.RegularExpressions;
            using HydraScript.Domain.FrontEnd.Lexer;

            namespace HydraScript.Infrastructure;

            [PatternContainer<TestPatternContainer>("[{ \"tag\": \"Number\", \"pattern\": \"[0-9]+\", \"priority\": 2 }, { \"tag\": \"Word\", \"pattern\": \"[a-zA-Z]+\", \"priority\": 1 }]")]
            internal partial class TestPatternContainer : IGeneratedRegexContainer
            {
                public static Regex GetRegex() => throw new NotImplementedException();
            }
            """);

        const string expectedSource =
""""
// <auto-generated/>

using System.Diagnostics.CodeAnalysis;

namespace HydraScript.Infrastructure;

internal partial class TestPatternContainer
{
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string Pattern =
        """
            (?<Word>[a-zA-Z]+)|(?<Number>[0-9]+)|(?<ERROR>\S+)
        """;
}

"""";

        var generator = new PatternGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation,
            out var diagnostics);
        Debug.Assert(diagnostics.IsEmpty);
        Debug.Assert(outputCompilation.SyntaxTrees.Count() == 3);

        var runResult = driver.GetRunResult();

        var generatedFileSyntax = runResult.GeneratedTrees
            .Single(t => t.FilePath.EndsWith("TestPatternContainer.g.cs"));

        Assert.Equal(
            expectedSource,
            generatedFileSyntax.GetText().ToString(),
            ignoreLineEndingDifferences: true);
    }

    private static CSharpCompilation CreateCompilation(string source) =>
        CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText(source) },
            new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
}