using StudentDataAccessLayer;

namespace StudentAPIBusinessLayer
{
    public class Student
    {
        public enum enMode { AddNew,Update}
        public enMode Mode = enMode.AddNew;

        public int ID {  get; set; }
        public string Name { get; set; }
        public int Age {  get; set; }
        public int Grade {  get; set; }

        public Student(StudentDTO SDTO, enMode mode = enMode.AddNew)
        {
            this.ID = SDTO.Id;
            this.Name = SDTO.Name;
            this.Age = SDTO.Age;
            this.Grade = SDTO.Grade;

            this.Mode = mode;
        }

        public StudentDTO SDTO { get { return (new StudentDTO(this.ID, this.Name, this.Age, this.Grade)); } }

        private bool _AddNewStudent()
        {
            this.ID = StudentData.AddStudent(SDTO);

            return ID != 0;
        }

        private bool _UpdateStudent()
        {
            return StudentData.UpdateStudent(SDTO);
        }


        public static List<StudentDTO> GetAllStudents()
        {
            return StudentData.GetAllStudents();
        }

        public static List<StudentDTO> GetPassedStudents()
        {
            return StudentData.GetPassedStudents();
        }

        public static double GetAverageGrade()
        {
            return StudentData.GetAverageGrade();
        }

        public static bool DeleteStudent(int id)
        {
            return StudentData.DeleteStudent(id);
        }

        public static Student Find(int id)
        {
            StudentDTO SDTO = StudentData.GetStudentById(id);

            if (SDTO != null)
            {
                return new Student(SDTO, enMode.Update);
            }
            return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewStudent())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateStudent();

            }
            return false;
        }
    }
}
