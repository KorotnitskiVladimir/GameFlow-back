namespace GameFlow.Services.KDF;

public interface IKDFService
{
    string DerivedKey(string password, string salt);
}