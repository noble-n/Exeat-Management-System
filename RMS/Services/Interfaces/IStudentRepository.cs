using RMS.Model;
using RMS.ViewModel.Students;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RMS.Services.Interfaces
{
    public interface IStudentRepository
    {
        /// <summary>
        /// Get all students
        /// </summary>
        /// <returns>List of students</returns>
        Task<List<StudentVM>> GetStudentsAsync(string search);
        /// <summary>
        /// Get student by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>student</returns>
        /// 

        //Task<List<Student>> GetStudentsForSigningAsync(string search);
        /// <summary>
        /// Get student by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>student</returns>
        Task<StudentVM> GetStudentByIdAsync(int id);

        /// <summary>
        /// Create students
        /// </summary>
        /// <param name="model"></param>
        /// <returns>an integer value representing the save status</returns>
        Task<int> CreateStudent(CreateStudentVM model, string loggedInUser);
        /// <summary>
        /// update student
        /// </summary>
        /// <param name="model"></param>
        /// <returns>an integer value representing the save status</returns>
        Task<int> UpdateStudent(StudentVM model);
        Task<int> EditProfile(EditProfileVM model);
        /// <summary>
        /// Delete students
        /// </summary>
        /// <param name="model"></param>
        /// <returns>an integer value representing the save status</returns>
        Task<int> DeleteStudent(int id, string LoggedInUser);
        Task<StudentVM> StudentDetails(int id);
    }
}
