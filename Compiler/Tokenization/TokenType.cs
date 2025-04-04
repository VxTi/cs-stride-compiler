namespace Compiler.Tokenization;

public enum TokenType
{
    IgnoreToken,
    KeywordFn,
    KeywordPub,
    KeywordPkg,
    KeywordAnd,
    KeywordHas,
    KeywordAs,
    KeywordLet,
    KeywordReturn,
    KeywordIf,
    KeywordElse,
    KeywordClass,
    KeywordConst,
    KeywordAsync,
    KeywordThis,
    KeywordEnum,
    KeywordSwitch,
    KeywordCase,
    KeywordDefault,
    KeywordBreak,
    KeywordStruct,
    KeywordUse,
    KeywordExternal,
    KeywordNull,
    KeywordOverride,
    KeywordDo,
    KeywordWhile,
    KeywordFor,
    KeywordTry,
    KeywordCatch,
    KeywordThrow,
    KeywordNew,
    PrimitiveBool,
    PrimitiveString,
    PrimitiveChar,
    PrimitiveVoid,
    PrimitiveVar,
    PrimitiveInt8,
    PrimitiveInt16,
    PrimitiveInt32,
    PrimitiveInt64,
    PrimitiveFloat32,
    PrimitiveFloat64,
    Identifier,
    NumberInteger,
    BooleanLiteral,
    NumberFloat,
    StringLiteral,
    CharLiteral,
    ThreeDots,
    LSquareBracket,
    RSquareBracket,
    LBrace,
    RBrace,
    LParen,
    RParen,
    Comma,
    DashRArrow,
    LArrowDash,
    LEquals,
    GEquals,
    DoubleLArrowEquals,
    DoubleRArrowEquals,
    DoubleStarEquals,
    DoubleLArrow,
    DoubleRArrow,
    DoublePipe,
    DoublePlus,
    DoubleMinus,
    DoubleStar,
    DoubleColon,
    DoubleEquals,
    DoubleAmpersand,
    StarEquals,
    SlashEquals,
    PercentEquals,
    AmpersandEquals,
    PipeEquals,
    CaretEquals,
    BangEquals,
    PlusEquals,
    TildeEquals,
    MinusEquals,
    Colon,
    Semicolon,
    Question,
    Star,
    Slash,
    Percent,
    Ampersand,
    Pipe,
    Caret,
    Tilde,
    Bang,
    Plus,
    Minus,
    RArrow,
    LArrow,
    Equals,
    Dot,
    Keyword
}