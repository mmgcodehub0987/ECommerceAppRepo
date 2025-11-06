using System.Text.Json.Serialization;

namespace ECOMapplication.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RefundMethod
    {
        BankAccount = 1,
        DebitCard = 2,
        CreditCard = 3,
        UPI = 4 
    }
}
