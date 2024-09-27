using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestApi.Models
 
{
    public class Students
    {
        [Key]
        public int studentId { get; set; }
        public string studentName { get; set; }
        public int age { get; set; }
        public string town { get; set; }
        public int courseId { get; set; }

        [JsonIgnore]
        public Courses Course { get; set; }
    }
}
