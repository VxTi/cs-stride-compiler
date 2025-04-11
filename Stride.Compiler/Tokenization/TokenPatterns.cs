using System.Text.RegularExpressions;

namespace Stride.Compiler.Tokenization;

public static class TokenPatterns
{
    private static Regex CompilePattern(string pattern) => new("^" + pattern);

    public static readonly Dictionary<Regex, TokenType> Patterns = new()
    {
        { CompilePattern(@"#.+"), TokenType.IgnoreToken },
        { CompilePattern(@"\s"), TokenType.IgnoreToken }, // Ignore whitespace
        { CompilePattern("""
                         "[^"]*"
                         """), TokenType.StringLiteral },
        { CompilePattern(@"'(\\[^']|\\'|[^'])'"), TokenType.CharLiteral },
        { CompilePattern(@"fn\b"), TokenType.KeywordFn },
        { CompilePattern(@"public\b"), TokenType.KeywordPub },
        { CompilePattern(@"package\b"), TokenType.KeywordPackage },
        { CompilePattern(@"as\b"), TokenType.KeywordAs },
        { CompilePattern(@"let\b"), TokenType.KeywordLet },
        { CompilePattern(@"return\b"), TokenType.KeywordReturn },
        { CompilePattern(@"if\b"), TokenType.KeywordIf },
        { CompilePattern(@"else\b"), TokenType.KeywordElse },
        { CompilePattern(@"class\b"), TokenType.KeywordClass },
        { CompilePattern(@"const\b"), TokenType.KeywordConst },
        { CompilePattern(@"async\b"), TokenType.KeywordAsync },
        { CompilePattern(@"this\b"), TokenType.KeywordThis },
        { CompilePattern(@"enum\b"), TokenType.KeywordEnum },
        { CompilePattern(@"switch\b"), TokenType.KeywordSwitch },
        { CompilePattern(@"case\b"), TokenType.KeywordCase },
        { CompilePattern(@"default\b"), TokenType.KeywordDefault },
        { CompilePattern(@"break\b"), TokenType.KeywordBreak },
        { CompilePattern(@"struct\b"), TokenType.KeywordStruct },
        { CompilePattern(@"use\b"), TokenType.KeywordUse },
        { CompilePattern(@"external\b"), TokenType.KeywordExternal },
        { CompilePattern(@"null\b"), TokenType.KeywordNull },
        { CompilePattern(@"override\b"), TokenType.KeywordOverride },
        { CompilePattern(@"do\b"), TokenType.KeywordDo },
        { CompilePattern(@"while\b"), TokenType.KeywordWhile },
        { CompilePattern(@"for\b"), TokenType.KeywordFor },
        { CompilePattern(@"try\b"), TokenType.KeywordTry },
        { CompilePattern(@"catch\b"), TokenType.KeywordCatch },
        { CompilePattern(@"throw\b"), TokenType.KeywordThrow },
        { CompilePattern(@"new\b"), TokenType.KeywordNew },
        { CompilePattern(@"bool\b"), TokenType.PrimitiveBool },
        { CompilePattern(@"string\b"), TokenType.PrimitiveString },
        { CompilePattern(@"char\b"), TokenType.PrimitiveChar },
        { CompilePattern(@"void\b"), TokenType.PrimitiveVoid },
        { CompilePattern(@"var\b"), TokenType.PrimitiveVar },
        { CompilePattern(@"i8\b"), TokenType.PrimitiveInt8 },
        { CompilePattern(@"i16\b"), TokenType.PrimitiveInt16 },
        { CompilePattern(@"i32\b"), TokenType.PrimitiveInt32 },
        { CompilePattern(@"i64\b"), TokenType.PrimitiveInt64 },
        { CompilePattern(@"f32\b"), TokenType.PrimitiveFloat32 },
        { CompilePattern(@"f64\b"), TokenType.PrimitiveFloat64 },
        { CompilePattern(@"[a-zA-Z_$][a-zA-Z0-9_$]*\b"), TokenType.Identifier },
        { CompilePattern(@"[\+\-]?[0-9]+([eE][0-9]+)?"), TokenType.NumberInteger },
        { CompilePattern(@"(true|false)\b"), TokenType.BooleanLiteral },
        {
            CompilePattern(@"[\+\-]?([0-9]+\.[eE][-+]?[0-9]+|[0-9]*\.?[0-9]+[eE][\+\-]?[0-9]+|[0-9]*\.[0-9]+|[0-9]+)"),
            TokenType.NumberFloat
        },
        { CompilePattern(@"\.{3}"), TokenType.ThreeDots },
        { CompilePattern(@"\["), TokenType.LSquareBracket },
        { CompilePattern(@"\]"), TokenType.RSquareBracket },
        { CompilePattern(@"\{"), TokenType.LBrace },
        { CompilePattern("}"), TokenType.RBrace },
        { CompilePattern(@"\("), TokenType.LParen },
        { CompilePattern(@"\)"), TokenType.RParen },
        { CompilePattern(","), TokenType.Comma },
        { CompilePattern("->"), TokenType.DashRArrow },
        { CompilePattern("<-"), TokenType.LArrowDash },
        { CompilePattern("<="), TokenType.LEquals },
        { CompilePattern(">="), TokenType.GEquals },
        { CompilePattern("<<="), TokenType.DoubleLArrowEquals },
        { CompilePattern(">>="), TokenType.DoubleRArrowEquals },
        { CompilePattern(@"\*\*="), TokenType.DoubleStarEquals },
        { CompilePattern("<<"), TokenType.DoubleLArrow },
        { CompilePattern(">>"), TokenType.DoubleRArrow },
        { CompilePattern(@"\|\|"), TokenType.DoublePipe },
        { CompilePattern(@"\+\+"), TokenType.DoublePlus },
        { CompilePattern("--"), TokenType.DoubleMinus },
        { CompilePattern(@"\*\*"), TokenType.DoubleStar },
        { CompilePattern("::"), TokenType.DoubleColon },
        { CompilePattern("=="), TokenType.DoubleEquals },
        { CompilePattern("&&"), TokenType.DoubleAmpersand },
        { CompilePattern(@"\*="), TokenType.StarEquals },
        { CompilePattern("/="), TokenType.SlashEquals },
        { CompilePattern("%="), TokenType.PercentEquals },
        { CompilePattern("&="), TokenType.AmpersandEquals },
        { CompilePattern(@"\|="), TokenType.PipeEquals },
        { CompilePattern(@"\^="), TokenType.CaretEquals },
        { CompilePattern("!="), TokenType.BangEquals },
        { CompilePattern(@"\+="), TokenType.PlusEquals },
        { CompilePattern("~="), TokenType.TildeEquals },
        { CompilePattern("-="), TokenType.MinusEquals },
        { CompilePattern(":"), TokenType.Colon },
        { CompilePattern(";"), TokenType.Semicolon },
        { CompilePattern(@"\?"), TokenType.Question },
        { CompilePattern(@"\*"), TokenType.Star },
        { CompilePattern("/"), TokenType.Slash },
        { CompilePattern("%"), TokenType.Percent },
        { CompilePattern("&"), TokenType.Ampersand },
        { CompilePattern(@"\|"), TokenType.Pipe },
        { CompilePattern(@"\^"), TokenType.Caret },
        { CompilePattern("~"), TokenType.Tilde },
        { CompilePattern("!"), TokenType.Bang },
        { CompilePattern(@"\+"), TokenType.Plus },
        { CompilePattern("-"), TokenType.Minus },
        { CompilePattern(">"), TokenType.RArrow },
        { CompilePattern("<"), TokenType.LArrow },
        { CompilePattern("="), TokenType.Equals },
        { CompilePattern(@"\."), TokenType.Dot },
        { CompilePattern(@"\w+"), TokenType.Keyword },
    };
}