namespace TAO3.Toast;

public interface IToastService : IDisposable
{
    void Notify(string title, string body, DateTimeOffset expirationTime);
}
