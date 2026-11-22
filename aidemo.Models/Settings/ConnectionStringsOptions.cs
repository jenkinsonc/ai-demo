using System.ComponentModel.DataAnnotations;

namespace aidemo.Models.Settings
{
    public class ConnectionStringsOptions
    {
        public const string SectionName = "ConnectionStrings";

        [Required]
        public string Database { get; set; }
    }
}
