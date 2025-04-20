namespace GameFlow.Services.KDF;

public class PBKDF1Service : IKDFService
{
    private int iterationCount = 3;
    private int dkLength = 20;

    public string DerivedKey(string password, string salt)
    {
        string t = password + salt;
        for (int i = 0; i < iterationCount; i++)
        {
            t = Hash(t);
        }

        return t[..dkLength];
    }

    private string Hash(string input)
    {
        return Convert.ToHexString(
            System.Security.Cryptography.SHA1.HashData(
                System.Text.Encoding.UTF8.GetBytes(input)));
    }
}