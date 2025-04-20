namespace GameFlow.Services.Storage;

public interface IstorageService
{
    string SaveFile(IFormFile formFile);
    string GetRealPath(string name);
}