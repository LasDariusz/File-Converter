namespace Api.Modules.Users.Domain.Models;

public class UserModel
{
    public Guid PublicId { get; set; }

    private string _email = string.Empty;
    public string Email {
        get => _email;
        private set
        {
            if (string.IsNullOrEmpty(value))
                throw new Exception();

            _email = value;
        } 
    }

    public string PasswordHash { get; set; }

    private UserModel(Guid userId, string email, string passwordHash)
    {
        PublicId = userId;
        Email = email;
        PasswordHash = passwordHash;
    }

    public static UserModel Create(string email, string passwordHash)
    {
        return new UserModel(
            Guid.CreateVersion7(),
            email, 
            passwordHash);
    }

    public static UserModel Rehydrate(Guid publicId, string email, string passwordHash)
    {
        return new UserModel(
            publicId, 
            email, 
            passwordHash);
    }

    public SafeUserDataModel CreateSafeUserData()
    {
        return new SafeUserDataModel
        {
            UserId = PublicId,
            Email = Email
        };
    }

    public bool ChangeEmail()
    {
        return false;
    }

    public bool ChangePasswordHash()
    {
        return false;
    }

}