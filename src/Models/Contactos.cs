//propiedades de la tabla ayuda a mapearlas
namespace DemoApi.Models;

public class Contacto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public string Birthday { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; }   = string.Empty;
}
