using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;


namespace StudentAPIClient
{
    class Program
    {
        static readonly HttpClient httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            httpClient.BaseAddress = new Uri("https://localhost:7244/api/Students/");

            await GetAllStudents();
            await GetPassedStudents();
            await GetAverageGrade();
            await GetStudentByID(1);
            await AddStudent(new Student { Id = 0 , Name = "Mohammed", Age = 25, Grade = 95 });
            await GetAllStudents();
            await DeleteStudent(2);
            await GetAllStudents();
            await UpdateStudent(1, new Student { Name = "Abdelrahman", Age = 22, Grade = 94 });
        }

        static async Task GetAllStudents()
        {
            try
            {
                Console.WriteLine("\n_____________________________\n");
                Console.WriteLine("Fetching all students...\n");

                var students = await httpClient.GetFromJsonAsync<List<Student>>("All");

                foreach (var student in students)
                {
                    Console.WriteLine($"ID : {student.Id} , Name : {student.Name} , Age : {student.Age}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred : {ex.Message}");
            }
        }

        static async Task GetPassedStudents()
        {
            try
            {
                Console.WriteLine("\n_____________________________\n");
                Console.WriteLine("Fetching Passed students...\n");

                var students = await httpClient.GetFromJsonAsync<List<Student>>("Passed");

                foreach (var student in students)
                {
                    Console.WriteLine($"ID : {student.Id} , Name : {student.Name} , Age : {student.Age} , Grade : {student.Grade}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred : {ex.Message}");
            }
        }

        static async Task GetAverageGrade()
        {
            try
            {
                Console.WriteLine("\n_____________________________\n");
                Console.WriteLine("Fetching average grade...\n");

                var AvgGrade = await httpClient.GetFromJsonAsync<double>("AverageGrade");

                Console.WriteLine($"Average grade = {AvgGrade}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred : {ex.Message}");
            }
        }

        static async Task GetStudentByID(int id)
        {
            try
            {
                Console.WriteLine("\n_____________________________\n");
                Console.WriteLine("Fetching student by ID...\n");

                var student = await httpClient.GetFromJsonAsync<Student>($"{id.ToString()}");

                if (student == null)
                { 
                    Console.WriteLine("Student not found");
                    return;              
                }
                Console.WriteLine($"ID : {student.Id} , Name : {student.Name} , Age : {student.Age} , Grade : {student.Grade}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred : {ex.Message}");
            }
        }

        static async Task AddStudent(Student newstudent)
        {
            try
            {
                Console.WriteLine("\n_____________________________\n");
                Console.WriteLine("Adding new student...\n");

                HttpResponseMessage response = await httpClient.PostAsJsonAsync<Student>("Add",newstudent);

                if (response.IsSuccessStatusCode)
                {
                    var addedstudent = await response.Content.ReadFromJsonAsync<Student>();
                    Console.WriteLine($"Added student - ID : {addedstudent.Id} , Name : {addedstudent.Name} , Age : {addedstudent.Age} , Grade : {addedstudent.Grade}");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    Console.WriteLine("Bad request : Invalid student data");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred : {ex.Message}");
            }
        }

        static async Task DeleteStudent(int ID)
        {
            try
            {
                Console.WriteLine("\n_____________________________\n");
                Console.WriteLine("Deleting new student...\n");

                var response = await httpClient.DeleteAsync($"{ID}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    Console.WriteLine($"Bad request : Invalid ID: {ID}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred : {ex.Message}");
            }
        }

        static async Task UpdateStudent(int id, Student newdata)
        {
            try
            {
                Console.WriteLine("\n_____________________________\n");
                Console.WriteLine("Updating new student...\n");

                var response = await httpClient.PutAsJsonAsync<Student>($"{id}",newdata);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    Console.WriteLine("Bad request: Invalid ID or Student data");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Grade { get; set; }

    }
}