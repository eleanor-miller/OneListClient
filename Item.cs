using System;

namespace OneListClient
{
    public class Item
    {
        public int id { get; set; }
        public string text { get; set; }
        public bool complete { get; set; }
        // Converted string to DateTime
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }


        // Custom 'get' for a property
        public string CompletedStatus
        {
            get
            {
                // if (complete)
                // {
                //     return "Completed";
                // }
                // else
                // {
                //     return "Not Completed";
                // }

                // Shorthand for the above =>  return   boolean expression  ?    value when true    :   value when false
                return complete ? "Completed" : "Not Completed";
            }
        }
    }
}