using System;
using System.Text.Json.Serialization;

namespace OneListClient
{
    public class Item
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("complete")]
        public bool Complete { get; set; }

        // Converted string to DateTime
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }


        // Custom 'get' for a property
        public string CompletedStatus
        {
            get
            {
                // Shorthand for  =>  return   boolean expression  ?    value when true    :   value when false
                return Complete ? "Completed" : "Not Completed";
            }
        }
    }
}