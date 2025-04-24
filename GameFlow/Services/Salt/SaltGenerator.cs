namespace GameFlow.Services.Salt;

public class SaltGenerator : ISaltGeneratorService
{
    public string GenerateSalt(int l)
    {
        Random r = new();
        string res = "";
        for (int i = 0; i < l; i++)
        {
            int v = r.Next(9);
            res += v.ToString();
        }

        return res;
    }
}