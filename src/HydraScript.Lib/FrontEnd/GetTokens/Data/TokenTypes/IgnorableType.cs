namespace HydraScript.Lib.FrontEnd.GetTokens.Data.TokenTypes;

public record IgnorableType(string Tag, string Pattern, int Priority)
    : TokenType(Tag, Pattern, Priority)
{
    public override bool CanIgnore() => true;
}