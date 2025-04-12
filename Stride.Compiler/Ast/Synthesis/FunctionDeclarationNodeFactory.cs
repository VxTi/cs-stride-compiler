using Stride.Common.Logging;
using Stride.Compiler.Ast.Generic;
using Stride.Compiler.Ast.Nodes;
using Stride.Compiler.Exceptions;
using Stride.Compiler.Generic;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public class FunctionDeclarationNodeFactory : AbstractTreeNodeFactory
{
    public override void Synthesize(TokenSet set, AstNode rootNode)
    {
        var accessibility = GetFunctionAccessibility(set);
        Logger.Info($"Accessibility: {accessibility}");
        
        var functionName = set.RequiresNext(TokenType.Identifier).Value;
        set.Consume(TokenType.LParenthesis);

        var arguments = ExtractFunctionArguments(set);

        set.Consume(TokenType.RParenthesis);
        set.Consume(TokenType.Colon);

        var returnType = InternalType.ExtractFromSet(set);

        var parentNode = new FunctionDeclarationNode(
            functionName,
            arguments,
            returnType,
            accessibility
        );

        // Creates a shallow copy of the provided set, and parses the subset as block
        var subset = LexicalScope.CreateSubsetForBlockScope(set);
        AstNodeFactory.GenerateAst(parentNode, subset);
        set.Next(subset.Remaining()); // Skip remaining tokens in subset to avoid duplicate parsing
        
        Logger.Info($"Remaining: {set.Remaining()}, {subset.Remaining()}");
        
        rootNode.Children.Add(parentNode);
    }

    private static List<FunctionArgumentNode> ExtractFunctionArguments(TokenSet set)
    {
        List<FunctionArgumentNode> arguments = new();
        // To make secondary argument extraction easier, we'll extract the first one like this,
        // and then consume TokenType.Comma and another function argument, since this is periodic.
        arguments.Add(ExtractFunctionArgument(set));
        
        while (set.PeekEqual(TokenType.Comma)&& set.Remaining() > 0)
        {
            set.Consume(TokenType.Comma);
            arguments.Add(ExtractFunctionArgument(set));
        };

        return arguments;
    }

    private static FunctionArgumentNode ExtractFunctionArgument(TokenSet set)
    {
        var type = InternalType.ExtractFromSet(set);
        var name = set.RequiresNext(TokenType.Identifier).Value;
        return new(name, type);
    }

    private static Accessibility GetFunctionAccessibility(TokenSet set)
    {
        var token = set.Next();
        if (token == null)
            throw new CompilationException("Reached end of file");
        
        return token?.Type switch
        {
            TokenType.KeywordPub => Accessibility.Public,
            TokenType.KeywordProt => Accessibility.PackagePrivate,
            TokenType.KeywordFn => Accessibility.Private,
            _ => throw new NotSupportedException($"Function accessibility {token} is not supported.")
        };
    }

    public override PermittedLexicalScope GetLexicalScope()
    {
        return PermittedLexicalScope.Global;
    }

    public override bool CanConsumeToken(Token nextToken, TokenSet set)
    {
        return nextToken.Type is TokenType.KeywordFn or TokenType.KeywordPub or TokenType.KeywordProt;
    }
}