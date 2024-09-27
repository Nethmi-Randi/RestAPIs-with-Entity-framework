using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestApi.Models;
using Microsoft.EntityFrameworkCore;


namespace RestApi.Controllers
{
    [Route("api/demo")]
    [ApiController]
    public class DemoAPIController : ControllerBase
    {
        private readonly ApiDemoDbContext _apiDemoDbContext;
        public DemoAPIController(ApiDemoDbContext apiDemoDbContext)
        {
            _apiDemoDbContext= apiDemoDbContext;
        }

        //GET all Students 
        [HttpGet]
        [Route("get-students")]
        public async Task<ActionResult> GetStudentsAsync()
        {
            var Students = await _apiDemoDbContext.Students.ToListAsync();
            return Ok(Students);
        }

        // GET a single student by studentId
        [HttpGet]
        [Route("get-student/{id}")]
        public async Task<ActionResult> GetStudentByIdAsync(int id)
        {
            var student = await _apiDemoDbContext.Students.FirstOrDefaultAsync(s => s.studentId == id);

            if (student == null)
            {
                return NotFound(new { Message = "Student not found." }); 
            }
            return Ok(student); 
        }

        //GET all Courses
        [HttpGet]
        [Route("get-courses")]
        public async Task<ActionResult> GetCoursesAsync()
        {
            var Courses = await _apiDemoDbContext.Courses.ToListAsync();
            return Ok(Courses);
        }

        // GET a single course by courseId
        [HttpGet]
        [Route("get-course/{id}")]
        public async Task<ActionResult> GetCourseByIdAsync(int id)
        {
            var course = await _apiDemoDbContext.Courses.FirstOrDefaultAsync(s => s.courseId == id);

            if (course == null)
            {
                return NotFound(new { Message = "Course not found." });
            }

            return Ok(course);
        }


        //Get the student enrolled for given course 
        [HttpGet("course/{courseId}")]
        public IActionResult GetStudentsByCourse(int courseId)
        {
            var students = _apiDemoDbContext.Students
                                   .Where(s => s.courseId == courseId)
                                   .Select(s => new
                                   {
                                       s.studentId,
                                       s.studentName,
                                   })
                                   .ToList();

            if (students == null || !students.Any())
            {
                return NotFound(new { Message = "No students enrolled for the given course." });
            }

            return Ok(students);
        }


        // POST a Student
        [HttpPost]
        [Route("add-student")]
        public async Task<ActionResult> AddStudentAsync([FromBody] StudentDto studentDto)
        {
            if (studentDto == null)
            {
                return BadRequest(new { Message = "Invalid student data." });
            }

            var courseExists = await _apiDemoDbContext.Courses.AnyAsync(c => c.courseId == studentDto.courseId);
            if (!courseExists)
            {
                return BadRequest(new { Message = "Invalid course ID. The course does not exist." });
            }

            var studentExists = await _apiDemoDbContext.Students.AnyAsync(s => s.studentId == studentDto.studentId);
            if (studentExists)
            {
                return BadRequest(new { Message = "A student with the same studentId already exists." });
            }

            var student = new Students
            {
                studentId = studentDto.studentId,
                studentName = studentDto.studentName,
                age = studentDto.age,
                town = studentDto.town,
                courseId = studentDto.courseId
            };

            await _apiDemoDbContext.Students.AddAsync(student);
            await _apiDemoDbContext.SaveChangesAsync();

            return Ok(new { Message = "Student added successfully.", Student = student });
        }


        // POST a Course
        [HttpPost]
        [Route("add-course")]
        public async Task<ActionResult> AddCourseAsync([FromBody] CourseDto courseDto)
        {
            if (courseDto == null)
            {
                return BadRequest(new { Message = "Invalid course data." });
            }

            var courseExists = await _apiDemoDbContext.Courses.AnyAsync(s => s.courseId == courseDto.courseId);
            if (courseExists)
            {
                return BadRequest(new { Message = "A course with the same courseId already exists." });
            }

            var course = new Courses
            {
                courseId = courseDto.courseId,
                courseTitle = courseDto.courseTitle,
                maxStudents = courseDto.maxStudents
                    
            };

            await _apiDemoDbContext.Courses.AddAsync(course);
            await _apiDemoDbContext.SaveChangesAsync();

            return Ok(new { Message = "Course added successfully.", Course = course });
        }



        // PUT Students 
        [HttpPut]
        [Route("update-student/{id}")]
        public async Task<ActionResult> UpdateStudentAsync(int id, [FromBody] StudentDto studentDto)
        {
            var existingStudent = await _apiDemoDbContext.Students.FindAsync(id);
            if (existingStudent == null)
            {
                return NotFound(new { Message = "Student not found." });
            }

            existingStudent.studentName = studentDto.studentName;
            existingStudent.age = studentDto.age;
            existingStudent.town = studentDto.town;
            existingStudent.courseId = studentDto.courseId;

            await _apiDemoDbContext.SaveChangesAsync();

            return Ok(new { Message = "Student updated successfully.", Student = existingStudent });
        }


        // PUT Course 
        [HttpPut]
        [Route("update-course/{id}")]
        public async Task<ActionResult> UpdateCourseAsync(int id, [FromBody] CourseDto courseDto)
        {
            var existingCourse = await _apiDemoDbContext.Courses.FindAsync(id);
            if (existingCourse == null)
            {
                return NotFound(new { Message = "Course not found." });
            }

            existingCourse.courseTitle = courseDto.courseTitle;
            existingCourse.maxStudents = courseDto.maxStudents;

            await _apiDemoDbContext.SaveChangesAsync();

            return Ok(new { Message = "Course updated successfully.", Course = existingCourse });
        }


        // DELETE  a Student 
        [HttpDelete]
        [Route("delete-student/{id}")]
        public async Task<ActionResult> DeleteStudentAsync(int id)
        {
            var student = await _apiDemoDbContext.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new { Message = "Student not found." });
            }

            _apiDemoDbContext.Students.Remove(student);
            await _apiDemoDbContext.SaveChangesAsync();

            return Ok(new { Message = "Student removed successfully." });
        }


        //Delete a Course
        [HttpDelete]
        [Route("delete-course/{id}")]
        public async Task<ActionResult> DeleteCourseAsync(int id)
        {
            var course = await _apiDemoDbContext.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound(new { Message = "Course not found." });
            }

            var studentsEnrolled = await _apiDemoDbContext.Students
                .AnyAsync(s => s.courseId == id);

            if (studentsEnrolled)
            {
                return BadRequest(new { Message = "Cannot delete course. Students are enrolled in this course." });
            }

            _apiDemoDbContext.Courses.Remove(course);
            await _apiDemoDbContext.SaveChangesAsync();

            return Ok(new { Message = "Course removed successfully." });
        }







    }
}
