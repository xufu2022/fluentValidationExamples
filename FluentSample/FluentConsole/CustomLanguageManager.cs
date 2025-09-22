namespace FluentConsole;

public class CustomLanguageManager : FluentValidation.Resources.LanguageManager
{
    public CustomLanguageManager()
    {
        AddTranslation("en", "NotNullValidator", "'{PropertyName}' is required.");
        AddTranslation("en-US", "NotNullValidator", "'{PropertyName}' is required.");
        AddTranslation("en-GB", "NotNullValidator", "'{PropertyName}' is required.");
    }
}