public class FileReadResult
{
    public string text;
    public string error;
    public byte[] data;

    public bool IsError { get; set; }
}