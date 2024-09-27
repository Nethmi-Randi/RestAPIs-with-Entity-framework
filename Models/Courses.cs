using System.ComponentModel.DataAnnotations;

namespace RestApi.Models
{
    public class Courses
    {
        [Key]
        public int courseId { get; set; }
        public string courseTitle { get; set; }
        public int maxStudents { get; set; }


        public ICollection<Students> Students { get; set; }
    }
}
