namespace Newspoint.Application.Services;

public static class ServiceMessages
{
    // General
    public const string Error = "Nastala chyba.";
    
    // Article
    public const string ArticleNotFound = "Tento článek nebyl nalezen.";
    public const string ArticleError = "Nastala chyba.";
    public const string ArticleTitleRequired = "Název článku je povinný.";
    public const string ArticleContentRequired = "Obsah článku je povinný.";

    // Comment
    public const string CommentNotFound = "Tento komentář nebyl nalezen.";
    public const string CommentError = "Nastala chyba.";
    public const string CommentContentRequired = "Obsah komentáře je povinný.";

    // Category
    public const string CategoryNotFound = "Tato kategorie nebyla nalezena.";

    // Author
    public const string AuthorNotFound = "Tento autor nebyl nalezen.";
    
    // User
    public const string UserEmailExist = "Tento email je již zaregistrovany.";
    public const string UserRegisterError = "Nastala chyba.";
    public const string UserNotFound = "Tento uživatel nebyl nalezen.";
    public const string UserEmailRequired = "Email je povinný.";
    public const string UserEmailInvalid = "Email není ve správném formátu.";
    public const string UserFirstNameRequired = "Jméno je povinné.";
    public const string UserLastNameRequired = "Příjmení je povinné.";
    public const string UserPasswordRequired = "Heslo je povinné.";
}