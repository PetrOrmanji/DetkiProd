using System.Text.Json.Serialization;

namespace DetkiProd.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    Admin
}