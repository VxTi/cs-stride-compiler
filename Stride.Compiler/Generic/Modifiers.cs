using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Generic;

public enum AccessModifier
{
    Public = 1,
    Private = 2,
    PackagePrivate = 4,
}

public enum BehavioralModifier
{
    Async = 32,
    Mutable = 512,
}

public static class Modifiers
{
    public static bool IsAccessModifier(TokenType type)
    {
        return type is TokenType.KeywordPub or TokenType.KeywordProt;
    }

    public static bool IsAccessModifier(int modifier)
    {
        const int inverted = ~(
            (int)AccessModifier.Public
            | (int)AccessModifier.Private
            | (int)AccessModifier.PackagePrivate
        );
        return (modifier & inverted) == 0;
    }

    public static bool IsValidModifier(int modifiers)
    {
        const int inverted = ~(
            (int)AccessModifier.Public
            | (int)AccessModifier.Private
            | (int)AccessModifier.PackagePrivate
            | (int)BehavioralModifier.Async
            | (int)BehavioralModifier.Mutable
        );
        return (modifiers & inverted) == 0;
    }

    public static bool IsModifier(TokenType type)
    {
        return type switch
        {
            TokenType.KeywordPub
                or TokenType.KeywordProt
                or TokenType.KeywordAsync
                or TokenType.KeywordMut
                => true,
            _ => false
        };
    }

    public static int FromToken(TokenType token)
    {
        return token switch
        {
            TokenType.KeywordPub => (int)AccessModifier.Public,
            TokenType.KeywordProt => (int)AccessModifier.PackagePrivate,
            TokenType.KeywordAsync => (int)BehavioralModifier.Async,
            TokenType.KeywordMut => (int)BehavioralModifier.Mutable,
            _ => 0,
        };
    }
}