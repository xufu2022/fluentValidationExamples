namespace FluntLibs;

public class ContactRequest
{
    public IContact Contact { get; set; } = null!;
    public List<IContact> Contacts { get; set; } = new List<IContact>();
    public string MessageToSend { get; set; } = string.Empty;
}