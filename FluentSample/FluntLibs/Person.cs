namespace FluntLibs;

public class Person
{
    public string Surname { get; set; } = string.Empty;
    public string Forename { get; set; } = string.Empty;
    public int Id { get; set; }
    public bool HasDiscount { get; set; }
    public double CustomerDiscount { get; set; }
    public string Postcode { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PasswordConfirmation { get; set; } = string.Empty;
    public bool IsPreferredCustomer { get; set; }
    public bool IsPreferred { get; set; }
    public string CreditCardNumber { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public List<string> AddressLines { get; set; } = new List<string>();
    public List<Pet> Pets { get; set; } = new List<Pet>();
    public List<Order> Orders { get; set; } = new List<Order>();
    public IContact Contact { get; set; } = null!; // Added for inheritance validation
    public List<IContact> Contacts { get; set; } = new List<IContact>(); // Added for collection inheritance
}