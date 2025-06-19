using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Enum cho trình độ khóa học
public enum CourseLevel { Beginner, Intermediate, Advanced }

// Abstract class User
public abstract class User
{
    public abstract void Login();
    public abstract void Logout();
}

// Interface ICanLearn
public interface ICanLearn
{
    void RegisterCourse(Course course);
    void TakeExam(Course course, double score);
}

// Lớp cha Person với access modifiers hợp lý
public class Person
{
    // Private fields
    private string _fullName;
    private string _email;
    
    // Public properties với validation
    public string FullName 
    { 
        get => _fullName;
        set 
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tên không được để trống");
            _fullName = value;
        }
    }
    
    public string Email 
    { 
        get => _email;
        set 
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                throw new ArgumentException("Email không hợp lệ");
            _email = value;
        }
    }
    
    // Constructor với validation
    public Person(string fullName, string email)
    {
        FullName = fullName; // Sử dụng property để validation
        Email = email;
    }
    
    // Protected method có thể override
    protected virtual string GetDisplayPrefix()
    {
        return "Thông tin cá nhân:";
    }
    
    public virtual void DisplayInfo()
    {
        Console.WriteLine($"{GetDisplayPrefix()} Họ tên: {FullName}, Email: {Email}");
    }
}

// Lớp Student với encapsulation tốt hơn
public class Student : Person, ICanLearn
{
    // Private fields
    private string _level;
    private readonly List<Enrollment> _enrollments;
    
    // Public property với validation
    public string Level 
    { 
        get => _level;
        set 
        {
            if (value != "Beginner" && value != "Intermediate" && value != "Advanced")
                throw new ArgumentException("Trình độ không hợp lệ");
            _level = value;
        }
    }
    
    // Read-only property để bảo vệ collection
    public IReadOnlyList<Enrollment> Enrollments => _enrollments.AsReadOnly();
    
    // Constructor với validation đầy đủ
    public Student(string fullName, string email, string level = "Beginner") : base(fullName, email)
    {
        _enrollments = new List<Enrollment>();
        Level = level; // Sử dụng property để validation
    }
    
    // Override protected method
    protected override string GetDisplayPrefix()
    {
        return "Thông tin học viên:";
    }
    
    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Trình độ: {Level}");
        Console.WriteLine("Khóa học đã đăng ký:");
        foreach (var e in _enrollments)
        {
            Console.WriteLine($"  - {e.Course.Name} ({e.Course.Level}) | Điểm: {(e.Score.HasValue ? e.Score.ToString() : "Chưa có")}");
        }
    }
    
    public void RegisterCourse(Course course)
    {
        if (course == null)
            throw new ArgumentNullException(nameof(course));
            
        if (!_enrollments.Any(e => e.Course == course))
            _enrollments.Add(new Enrollment(this, course));
        else
            Console.WriteLine("Học viên đã đăng ký khóa học này rồi!");
    }
    
    public void TakeExam(Course course, double score)
    {
        if (course == null)
            throw new ArgumentNullException(nameof(course));
        if (score < 0 || score > 10)
            throw new ArgumentException("Điểm phải từ 0 đến 10");
            
        var enroll = _enrollments.FirstOrDefault(e => e.Course == course);
        if (enroll != null)
            enroll.Score = score;
        else
            throw new InvalidOperationException("Học viên chưa đăng ký khóa học này");
    }
    
    // Private helper method
    private bool IsEnrolledInCourse(Course course)
    {
        return _enrollments.Any(e => e.Course == course);
    }
    
    // Implement abstract methods from User
    public void Login() { Console.WriteLine($"{FullName} đã đăng nhập."); }
    public void Logout() { Console.WriteLine($"{FullName} đã đăng xuất."); }
}

// Lớp Course với encapsulation
public class Course
{
    // Private fields
    private string _name;
    private CourseLevel _level;
    
    // Public properties với validation
    public string Name 
    { 
        get => _name;
        set 
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tên khóa học không được để trống");
            _name = value;
        }
    }
    
    public CourseLevel Level 
    { 
        get => _level;
        set => _level = value;
    }
    
    // Constructor với validation
    public Course(string name, CourseLevel level)
    {
        Name = name; // Sử dụng property để validation
        Level = level;
    }
    
    // Override ToString để hiển thị thông tin
    public override string ToString()
    {
        return $"{Name} ({Level})";
    }
    
    // Override Equals để so sánh courses
    public override bool Equals(object obj)
    {
        if (obj is Course other)
            return Name == other.Name && Level == other.Level;
        return false;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Level);
    }
}

// Lớp Enrollment với encapsulation
public class Enrollment
{
    // Private fields
    private double? _score;
    
    // Read-only properties
    public Student Student { get; private set; }
    public Course Course { get; private set; }
    
    // Property với validation
    public double? Score 
    { 
        get => _score;
        set 
        {
            if (value.HasValue && (value < 0 || value > 10))
                throw new ArgumentException("Điểm phải từ 0 đến 10");
            _score = value;
        }
    }
    
    // Constructor
    public Enrollment(Student student, Course course)
    {
        Student = student ?? throw new ArgumentNullException(nameof(student));
        Course = course ?? throw new ArgumentNullException(nameof(course));
        Score = null;
    }
    
    // Method để kiểm tra đã có điểm chưa
    public bool HasScore() => Score.HasValue;
    
    // Method để lấy điểm dạng string
    public string GetScoreDisplay() => Score?.ToString() ?? "Chưa có";
}

class Program
{
    static List<Student> students = new List<Student>();
    static List<Course> courses = new List<Course>()
    {
        new Course("C# Cơ bản", CourseLevel.Beginner),
        new Course("Java Nâng cao", CourseLevel.Advanced),
        new Course("Python Ứng dụng", CourseLevel.Intermediate)
    };
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n===== MENU =====");
            Console.WriteLine("1. Thêm học viên");
            Console.WriteLine("2. Đăng ký khóa học");
            Console.WriteLine("3. Nhập điểm");
            Console.WriteLine("4. Hiển thị danh sách");
            Console.WriteLine("5. Ghi dữ liệu ra file");
            Console.WriteLine("6. Đọc dữ liệu từ file");
            Console.WriteLine("0. Thoát");
            Console.Write("Chọn chức năng: ");
            string choice = Console.ReadLine();
            try
            {
                switch (choice)
                {
                    case "1": AddStudent(); break;
                    case "2": RegisterCourse(); break;
                    case "3": InputScore(); break;
                    case "4": ShowStudents(); break;
                    case "5": WriteToFile(); break;
                    case "6": ReadFromFile(); break;
                    case "0": return;
                    default: Console.WriteLine("Lựa chọn không hợp lệ!"); break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Lỗi: Nhập sai kiểu dữ liệu!");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Lỗi: Không tìm thấy file!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
        }
    }
    static void AddStudent()
    {
        string name;
        do {
            Console.Write("Họ tên: ");
            name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) Console.WriteLine("Tên không được để trống!");
        } while (string.IsNullOrWhiteSpace(name));
    
        string email;
        do {
            Console.Write("Email: ");
            email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@")) Console.WriteLine("Email không hợp lệ!");
        } while (string.IsNullOrWhiteSpace(email) || !email.Contains("@"));
    
        string level;
        do {
            Console.Write("Trình độ (Beginner/Intermediate/Advanced): ");
            level = Console.ReadLine();
            if (level != "Beginner" && level != "Intermediate" && level != "Advanced") Console.WriteLine("Trình độ không hợp lệ!");
        } while (level != "Beginner" && level != "Intermediate" && level != "Advanced");
    
        var student = new Student(name, email) { Level = level };
        students.Add(student);
        Console.WriteLine("Đã thêm học viên!");
    }
    static void RegisterCourse()
    {
        if (students.Count == 0) { Console.WriteLine("Chưa có học viên!"); return; }
        Console.WriteLine("Chọn học viên:");
        for (int i = 0; i < students.Count; i++)
            Console.WriteLine($"{i + 1}. {students[i].FullName}");
        int sIdx = int.Parse(Console.ReadLine()) - 1;
        if (sIdx < 0 || sIdx >= students.Count) throw new Exception("Không tìm thấy học viên!");
        Console.WriteLine("Chọn khóa học:");
        for (int i = 0; i < courses.Count; i++)
            Console.WriteLine($"{i + 1}. {courses[i].Name} ({courses[i].Level})");
        int cIdx = int.Parse(Console.ReadLine()) - 1;
        if (cIdx < 0 || cIdx >= courses.Count) throw new Exception("Không tìm thấy khóa học!");
        students[sIdx].RegisterCourse(courses[cIdx]);
        Console.WriteLine("Đăng ký thành công!");
    }
    static void InputScore()
    {
        if (students.Count == 0) { Console.WriteLine("Chưa có học viên!"); return; }
        Console.WriteLine("Chọn học viên:");
        for (int i = 0; i < students.Count; i++)
            Console.WriteLine($"{i + 1}. {students[i].FullName}");
        int sIdx = int.Parse(Console.ReadLine()) - 1;
        if (sIdx < 0 || sIdx >= students.Count) throw new Exception("Không tìm thấy học viên!");
        var student = students[sIdx];
        if (student.Enrollments.Count == 0) { Console.WriteLine("Học viên chưa đăng ký khóa học!"); return; }
        Console.WriteLine("Chọn khóa học để nhập điểm:");
        for (int i = 0; i < student.Enrollments.Count; i++)
            Console.WriteLine($"{i + 1}. {student.Enrollments[i].Course.Name}");
        int eIdx = int.Parse(Console.ReadLine()) - 1;
        if (eIdx < 0 || eIdx >= student.Enrollments.Count) throw new Exception("Không tìm thấy khóa học!");
        Console.Write("Nhập điểm: ");
        double score;
        do {
            Console.Write("Nhập điểm: ");
            string input = Console.ReadLine();
            if (!double.TryParse(input, out score) || score < 0 || score > 10)
                Console.WriteLine("Điểm phải là số từ 0 đến 10!");
        } while (score < 0 || score > 10);
        student.TakeExam(student.Enrollments[eIdx].Course, score);
        Console.WriteLine("Đã nhập điểm!");
    }
    static void ShowStudents()
    {
        if (students.Count == 0) { Console.WriteLine("Chưa có học viên!"); return; }
        foreach (var s in students)
        {
            s.DisplayInfo();
            Console.WriteLine("----------------------");
        }
    }
    static void WriteToFile()
    {
        string path = "students.csv";
        using (var sw = new StreamWriter(path))
        {
            foreach (var s in students)
            {
                foreach (var e in s.Enrollments)
                {
                    string scoreStr = e.Score.HasValue ? $"Điểm: {e.Score}" : "Điểm: Chưa có";
                    sw.WriteLine($"Họ tên: {s.FullName},Email: {s.Email},Trình độ: {s.Level},Khóa học: {e.Course.Name},Cấp độ: {e.Course.Level},{scoreStr}");
                }
            }
        }
        Console.WriteLine("Đã ghi dữ liệu ra file!");
    }
    static void ReadFromFile()
    {
        string path = "students.csv";
        if (!File.Exists(path)) throw new FileNotFoundException();
        students.Clear();
        using (var sr = new StreamReader(path))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                var parts = line.Split(',');
                var name = parts[0];
                var email = parts[1];
                var level = parts[2];
                var courseName = parts[3];
                var courseLevel = (CourseLevel)Enum.Parse(typeof(CourseLevel), parts[4]);
                var score = string.IsNullOrEmpty(parts[5]) ? (double?)null : double.Parse(parts[5]);
                var student = students.FirstOrDefault(s => s.FullName == name && s.Email == email);
                if (student == null)
                {
                    student = new Student(name, email) { Level = level };
                    students.Add(student);
                }
                var course = courses.FirstOrDefault(c => c.Name == courseName && c.Level == courseLevel);
                if (course == null)
                {
                    course = new Course(courseName, courseLevel);
                    courses.Add(course);
                }
                student.RegisterCourse(course);
                student.TakeExam(course, score ?? 0);
            }
        }
        Console.WriteLine("Đã đọc dữ liệu từ file!");
    }
}