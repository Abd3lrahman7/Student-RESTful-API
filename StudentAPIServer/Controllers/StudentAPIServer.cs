using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StudentAPIBusinessLayer;
using StudentDataAccessLayer;
using System.Threading.Tasks;

namespace StudentAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/Students")]

    [ApiController]
    public class StudentAPIServer : ControllerBase
    {
        [HttpGet("All",Name ="GetAllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<StudentDTO>> GetAllStudents()
        {
            var students = Student.GetAllStudents();

            if (students.Count == 0)
                return NotFound("Students not found");

            return Ok(students);
        }

        [HttpGet("Passed",Name = "GetPassedStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<StudentDTO>> GetPassedStudents()
        {
            var passedstudents = Student.GetPassedStudents();

            if (passedstudents.Count == 0)
                return NotFound("Passed students not found");

            return Ok(passedstudents);
        }

        [HttpGet("AverageGrade", Name = "GetAverageGrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<double> GetAverageGrade()
        {
            double averageGrade = Student.GetAverageGrade();

            if (averageGrade == 0)
            {
                return NotFound("No grades found");
            }

            return Ok(averageGrade);
        }

        [HttpGet("{id}", Name = "GetStudentByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<StudentDTO> GetStudentByID(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted id : {id}");
            }

            StudentAPIBusinessLayer.Student student = StudentAPIBusinessLayer.Student.Find(id);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            StudentDTO SDTO = student.SDTO;

            return Ok(SDTO);
        }

        [HttpPost("Add", Name = "AddStudent")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<StudentDTO> AddStudent(StudentDTO newstudentDTO)
        {
            if (newstudentDTO == null || string.IsNullOrEmpty(newstudentDTO.Name) || newstudentDTO.Age < 0 || newstudentDTO.Grade < 0)
            {
                return BadRequest("Invalid student data");
            }

            StudentAPIBusinessLayer.Student student = new Student(new StudentDTO(newstudentDTO.Id, newstudentDTO.Name, newstudentDTO.Age, newstudentDTO.Grade));

            student.Save();
            newstudentDTO.Id = student.ID;


            return CreatedAtRoute("GetStudentByID", new { id = newstudentDTO.Id }, newstudentDTO);
        }

        [HttpDelete("{id}", Name ="DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Invalid student ID {id}");
            }
            StudentAPIBusinessLayer.Student student = StudentAPIBusinessLayer.Student.Find(id);

            if (student == null)
            {
                return NotFound("Student not found");
            }
            
            if(Student.DeleteStudent(student.ID))
                return Ok($"Student with ID: {id} has been deleted");

            return StatusCode(500, "An error occurred");
        }

        [HttpPut("{id}",Name ="UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateStudent(int id, StudentDTO updatedStudent)
        {
            if (id < 1)
            {
                return BadRequest("Invalid student ID");
            }
            if (updatedStudent == null || string.IsNullOrEmpty(updatedStudent.Name) || updatedStudent.Age < 1 || updatedStudent.Grade < 0)
            {
                return BadRequest("Invalid student data");
            }

            StudentAPIBusinessLayer.Student student = Student.Find(id);

            if (student == null)
            {
                return NotFound("Student not found");
            }

            student.Name = updatedStudent.Name;
            student.Age = updatedStudent.Age;
            student.Grade = updatedStudent.Grade;

            if (student.Save())
            {
                return Ok(student);

            }
            return StatusCode(500, new { message = "An error occured " });

        }

    }

    public class ImageUploadController : ControllerBase
    {
        [HttpPost("Upload")]
        public async Task<IActionResult> UploadImage(IFormFile imagefile)
        {
            if (imagefile == null || imagefile.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var UploadDirectory = @"E:\MyUploads";

            var filename = Guid.NewGuid().ToString() + Path.GetExtension(imagefile.FileName);
            var filepath = Path.Combine(UploadDirectory, filename);

            if (!Directory.Exists(UploadDirectory))
            {
                Directory.CreateDirectory(UploadDirectory);
            }

            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                await imagefile.CopyToAsync(stream);
            }

            return Ok(new { filepath });
        }

        [HttpGet("GetImage/{filename}")]
        public IActionResult GetImage(string filename)
        {
            var UploadDirectory = @"E:\MyUploads";
            var filepath = Path.Combine(UploadDirectory, filename);

            if (!System.IO.File.Exists(filepath))
            {
                return NotFound("Image not found");
            }

            var image = System.IO.File.OpenRead(filepath);
            var mimetype = GetMimeType(filepath);

            return File(image, mimetype);
        }

        private string GetMimeType(string filepath)
        {
            var extention = Path.GetExtension(filepath).ToLowerInvariant();

            return extention switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                 _ => "application/octet-stream"

            };
        }
    }
}

