namespace GameFlow.Models;

public class RestResponse
{
    public string Service { get; set; } = null!;
    public RestStatus Status { get; set; } = new();
    public long CacheTime { get; set; } = 0L;
    public string DataType { get; set; } = "empty";
    public Dictionary<string, string> Meta { get; set; } = new();
    public Manipulations Manipulations { get; set; } = new();
    public Object? Data { get; set; }
}

public class RestStatus
{
    public bool IsOk { get; set; } = true;
    public int Code { get; set; } = 200;
    public string Phrase { get; set; } = "Ok";
}

public class Manipulations
{
    public string? Create { get; set; }
    public string? Read { get; set; }
    public string Update { get; set; }
    public string? Delete { get; set; }
}