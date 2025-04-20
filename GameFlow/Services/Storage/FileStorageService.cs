namespace GameFlow.Services.Storage;

public class FileStorageService : IstorageService
{
    private const string storagePath = "C:\\gf\\";

    public string GetRealPath(string name)
    {
        return storagePath + name;
    }

    public string SaveFile(IFormFile formFile)
    {
        var ext = Path.GetExtension(formFile.FileName);
        string savedName;
        string fullName;
        do
        {
            savedName = Guid.NewGuid() + ext;
            fullName = storagePath + savedName;
        } while (File.Exists(fullName));
        formFile.CopyTo(new FileStream(fullName, FileMode.CreateNew));

        return savedName;
    }
}