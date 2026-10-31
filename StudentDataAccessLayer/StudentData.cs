using Microsoft.Data.SqlClient;
using System.Data;

namespace StudentDataAccessLayer
{
    public class StudentDTO
    {
        public StudentDTO(int id, string Name, int Age, int Grade)
        {
            this.Id = id;
            this.Name = Name;
            this.Age = Age;
            this.Grade = Grade;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Grade { get; set; }  
    }

    public class StudentData
    {
        static string _connectionstring = "Server=localhost;Database=StudentsDB;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;";

        public static List<StudentDTO> GetAllStudents()
        {
            var Students = new List<StudentDTO>();

            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                using (SqlCommand command = new SqlCommand("SP_GetAllStudents", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) 
                        {
                            Students.Add(new StudentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal( "id")),
                                reader.GetString(reader.GetOrdinal( "Name")),
                                reader.GetInt32(reader.GetOrdinal( "Age")),
                                reader.GetInt32(reader.GetOrdinal( "Grade"))

                            ));
                                
                        }
                    }
                }
                return Students;
            }
        }

        public static List<StudentDTO> GetPassedStudents()
        {
            var Students = new List<StudentDTO>();

            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                using (SqlCommand command = new SqlCommand("SP_GetPassedStudents", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Students.Add(new StudentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("id")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetInt32(reader.GetOrdinal("Age")),
                                reader.GetInt32(reader.GetOrdinal("Grade"))

                            ));

                        }
                    }
                }
                return Students;
            }
        }

        public static double GetAverageGrade()
        {
            double averagegrade = 0;

            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                using (SqlCommand command = new SqlCommand("SP_GetAverageGrade", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != DBNull.Value)
                    {
                        averagegrade = Convert.ToDouble(result);
                    }
                    else
                        averagegrade = 0;
                }
                return averagegrade;
            }
        }

        public static StudentDTO GetStudentById(int StudentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                using (SqlCommand command = new SqlCommand("SP_GetStudentById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentId", StudentId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new StudentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("id")),
                                reader.GetString(reader.GetOrdinal("Name")),
                                reader.GetInt32(reader.GetOrdinal("Age")),
                                reader.GetInt32(reader.GetOrdinal("Grade"))

                            );

                        }
                        return null;
                    }
                }
            }
        }

        public static int AddStudent(StudentDTO studentDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                using (SqlCommand command = new SqlCommand("SP_AddStudent", connection))
                {
                    command.CommandType= CommandType.StoredProcedure;
                    connection.Open();

                    command.Parameters.AddWithValue("@Name",studentDTO.Name);
                    command.Parameters.AddWithValue("@Age", studentDTO.Age);
                    command.Parameters.AddWithValue("@Grade", studentDTO.Grade);
                    var outputIdparam = new SqlParameter("@NewStudentID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    command.Parameters.Add(outputIdparam);

                    command.ExecuteNonQuery();

                    return (int) outputIdparam.Value;
                }
            }
        }

        public static bool UpdateStudent(StudentDTO studentDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                using (SqlCommand command = new SqlCommand("SP_UpdateStudent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    command.Parameters.AddWithValue("@StudentId", studentDTO.Id);
                    command.Parameters.AddWithValue("@Name", studentDTO.Name);
                    command.Parameters.AddWithValue("@Age", studentDTO.Age);
                    command.Parameters.AddWithValue("@Grade", studentDTO.Grade);

                    command.ExecuteNonQuery();

                    return true;
                }
            }
        }

    

        public static bool DeleteStudent(int StudentID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                using (SqlCommand command = new SqlCommand("SP_DeleteStudent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    command.Parameters.AddWithValue("@StudentId", StudentID);
                    
                    int rowsAffected = (int) command.ExecuteScalar();

                    return rowsAffected == 1;
                }
            }
        }

    }
}
