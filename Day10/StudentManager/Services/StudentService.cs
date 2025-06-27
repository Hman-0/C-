using Microsoft.EntityFrameworkCore;
using StudentManager.Data;
using StudentManager.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentManager.Services
{
    public class StudentService
    {
        private readonly StudentContext _context;
        
        public StudentService(StudentContext context)
        {
            _context = context;
        }
        
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students.OrderBy(s => s.FullName).ToListAsync();
        }
        
        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }
        
        public async Task<List<Student>> SearchStudentsAsync(string searchTerm)
        {
            return await _context.Students
                .Where(s => s.FullName.Contains(searchTerm) || 
                           s.StudentCode.Contains(searchTerm) ||
                           s.Class.Contains(searchTerm))
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }
        
        public async Task<List<Student>> GetStudentsByClassAsync(string className)
        {
            return await _context.Students
                .Where(s => s.Class == className)
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }
        
        public async Task<bool> AddStudentAsync(Student student)
        {
            try
            {
                // Validate student data
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(student);
                
                if (!Validator.TryValidateObject(student, validationContext, validationResults, true))
                {
                    Console.WriteLine("Validation errors:");
                    foreach (var error in validationResults)
                    {
                        Console.WriteLine($"- {error.ErrorMessage}");
                    }
                    return false;
                }
                
                // Check for duplicate student code
                if (await _context.Students.AnyAsync(s => s.StudentCode == student.StudentCode))
                {
                    Console.WriteLine("Error: Student code already exists!");
                    return false;
                }
                
                // Check for duplicate email
                if (await _context.Students.AnyAsync(s => s.Email == student.Email))
                {
                    Console.WriteLine("Error: Email already exists!");
                    return false;
                }
                
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding student: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> UpdateStudentAsync(Student student)
        {
            try
            {
                var existingStudent = await _context.Students.FindAsync(student.Id);
                if (existingStudent == null)
                {
                    Console.WriteLine("Student not found!");
                    return false;
                }
                
                // Validate student data
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(student);
                
                if (!Validator.TryValidateObject(student, validationContext, validationResults, true))
                {
                    Console.WriteLine("Validation errors:");
                    foreach (var error in validationResults)
                    {
                        Console.WriteLine($"- {error.ErrorMessage}");
                    }
                    return false;
                }
                
                // Check for duplicate student code (excluding current student)
                if (await _context.Students.AnyAsync(s => s.StudentCode == student.StudentCode && s.Id != student.Id))
                {
                    Console.WriteLine("Error: Student code already exists!");
                    return false;
                }
                
                // Check for duplicate email (excluding current student)
                if (await _context.Students.AnyAsync(s => s.Email == student.Email && s.Id != student.Id))
                {
                    Console.WriteLine("Error: Email already exists!");
                    return false;
                }
                
                // Update properties
                existingStudent.FullName = student.FullName;
                existingStudent.StudentCode = student.StudentCode;
                existingStudent.Email = student.Email;
                existingStudent.PhoneNumber = student.PhoneNumber;
                existingStudent.Class = student.Class;
                existingStudent.GPA = student.GPA;
                existingStudent.DateOfBirth = student.DateOfBirth;
                existingStudent.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating student: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> DeleteStudentAsync(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                {
                    Console.WriteLine("Student not found!");
                    return false;
                }
                
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting student: {ex.Message}");
                return false;
            }
        }
        
        public async Task<Dictionary<string, int>> GetStudentStatsByClassAsync()
        {
            return await _context.Students
                .GroupBy(s => s.Class)
                .Select(g => new { Class = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Class, x => x.Count);
        }
        
        public async Task<double> GetAverageGPAAsync()
        {
            if (!await _context.Students.AnyAsync())
                return 0;
                
            return await _context.Students.AverageAsync(s => s.GPA);
        }
        
        public async Task<List<string>> GetAllClassesAsync()
        {
            return await _context.Students
                .Select(s => s.Class)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }
    }
}