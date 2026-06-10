namespace Web.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RequireOwnerAttribute(string codeParamName = "code") : Attribute
{
    public string CodeParamName { get; } = codeParamName;
}
