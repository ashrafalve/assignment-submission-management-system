using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using AssignmentManagement.Api.Domain.Entities;
using AssignmentManagement.Api.Domain.Enums;

namespace AssignmentManagement.Api.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure database is created and migrations applied
        await context.Database.MigrateAsync();

        // 1. Seed Classes if none exist
        if (!await context.Classes.AnyAsync())
        {
            var classA = new SchoolClass
            {
                Id = new Guid("10000000-0000-0000-0000-000000000001"),
                Name = "Grade 10 - Section A",
                AcademicYear = "2026-2027",
                Description = "Tenth Grade Main Science & Tech Stream",
                IsActive = true
            };
            var classB = new SchoolClass
            {
                Id = new Guid("10000000-0000-0000-0000-000000000002"),
                Name = "Grade 10 - Section B",
                AcademicYear = "2026-2027",
                Description = "Tenth Grade Humanities Stream",
                IsActive = true
            };

            await context.Classes.AddRangeAsync(classA, classB);
            await context.SaveChangesAsync();
        }

        var class10A = await context.Classes.FirstAsync(c => c.Name.Contains("Section A"));
        var class10B = await context.Classes.FirstAsync(c => c.Name.Contains("Section B"));

        // 2. Seed Subjects if none exist
        if (!await context.Subjects.AnyAsync())
        {
            var math = new Subject
            {
                Id = new Guid("20000000-0000-0000-0000-000000000001"),
                Name = "Mathematics",
                Code = "MATH-101",
                Description = "Algebra, Geometry, and Trigonometry",
                IsActive = true
            };
            var cs = new Subject
            {
                Id = new Guid("20000000-0000-0000-0000-000000000002"),
                Name = "Computer Science",
                Code = "CS-101",
                Description = "Algorithms, Data Structures & Python Programming",
                IsActive = true
            };
            var science = new Subject
            {
                Id = new Guid("20000000-0000-0000-0000-000000000003"),
                Name = "Physics & Chemistry",
                Code = "SCI-101",
                Description = "Basic Mechanics and Chemical Reactions",
                IsActive = true
            };
            var english = new Subject
            {
                Id = new Guid("20000000-0000-0000-0000-000000000004"),
                Name = "English Literature",
                Code = "ENG-101",
                Description = "Classic Literature & Academic Writing",
                IsActive = true
            };

            await context.Subjects.AddRangeAsync(math, cs, science, english);
            await context.SaveChangesAsync();
        }

        var mathSub = await context.Subjects.FirstAsync(s => s.Code == "MATH-101");
        var csSub   = await context.Subjects.FirstAsync(s => s.Code == "CS-101");
        var sciSub  = await context.Subjects.FirstAsync(s => s.Code == "SCI-101");

        // 3. Seed Users (Admin, Teachers, Students) if none exist beyond default admin
        if (await context.Users.CountAsync() <= 1)
        {
            var teacherPassword = BCrypt.Net.BCrypt.HashPassword("Teacher@1234", workFactor: 12);
            var studentPassword = BCrypt.Net.BCrypt.HashPassword("Student@1234", workFactor: 12);

            var johnTeacher = new User
            {
                Id = new Guid("30000000-0000-0000-0000-000000000001"),
                FirstName = "John",
                LastName = "Teacher",
                Email = "john.teacher@assignmentmanagement.com",
                PasswordHash = teacherPassword,
                Role = UserRole.Teacher,
                IsActive = true
            };

            var sarahTeacher = new User
            {
                Id = new Guid("30000000-0000-0000-0000-000000000002"),
                FirstName = "Sarah",
                LastName = "Teacher",
                Email = "sarah.teacher@assignmentmanagement.com",
                PasswordHash = teacherPassword,
                Role = UserRole.Teacher,
                IsActive = true
            };

            var alexStudent = new User
            {
                Id = new Guid("30000000-0000-0000-0000-000000000010"),
                FirstName = "Alex",
                LastName = "Student",
                Email = "alex.student@assignmentmanagement.com",
                PasswordHash = studentPassword,
                Role = UserRole.Student,
                ClassId = class10A.Id,
                IsActive = true
            };

            var emmaStudent = new User
            {
                Id = new Guid("30000000-0000-0000-0000-000000000011"),
                FirstName = "Emma",
                LastName = "Student",
                Email = "emma.student@assignmentmanagement.com",
                PasswordHash = studentPassword,
                Role = UserRole.Student,
                ClassId = class10A.Id,
                IsActive = true
            };

            var liamStudent = new User
            {
                Id = new Guid("30000000-0000-0000-0000-000000000012"),
                FirstName = "Liam",
                LastName = "Student",
                Email = "liam.student@assignmentmanagement.com",
                PasswordHash = studentPassword,
                Role = UserRole.Student,
                ClassId = class10B.Id,
                IsActive = true
            };

            await context.Users.AddRangeAsync(johnTeacher, sarahTeacher, alexStudent, emmaStudent, liamStudent);
            await context.SaveChangesAsync();
        }

        var teacherJohn  = await context.Users.FirstAsync(u => u.Email == "john.teacher@assignmentmanagement.com");
        var teacherSarah = await context.Users.FirstAsync(u => u.Email == "sarah.teacher@assignmentmanagement.com");
        var studentAlex  = await context.Users.FirstAsync(u => u.Email == "alex.student@assignmentmanagement.com");
        var studentEmma  = await context.Users.FirstAsync(u => u.Email == "emma.student@assignmentmanagement.com");

        // 4. Seed TeacherSubjects if none exist
        if (!await context.TeacherSubjects.AnyAsync())
        {
            var ts1 = new TeacherSubject
            {
                Id = new Guid("40000000-0000-0000-0000-000000000001"),
                TeacherId = teacherJohn.Id,
                SubjectId = mathSub.Id,
                ClassId = class10A.Id,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            var ts2 = new TeacherSubject
            {
                Id = new Guid("40000000-0000-0000-0000-000000000002"),
                TeacherId = teacherJohn.Id,
                SubjectId = csSub.Id,
                ClassId = class10A.Id,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            var ts3 = new TeacherSubject
            {
                Id = new Guid("40000000-0000-0000-0000-000000000003"),
                TeacherId = teacherSarah.Id,
                SubjectId = sciSub.Id,
                ClassId = class10A.Id,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            await context.TeacherSubjects.AddRangeAsync(ts1, ts2, ts3);
            await context.SaveChangesAsync();
        }

        // 5. Seed Sample Assignments if none exist
        if (!await context.Assignments.AnyAsync())
        {
            var assignment1 = new Assignment
            {
                Id = new Guid("50000000-0000-0000-0000-000000000001"),
                Title = "Algebraic Equations Problem Set 1",
                Description = "Complete all problems on pages 45 to 50. Show all steps for quadratic equation solving.",
                DueDate = DateTime.UtcNow.AddDays(7),
                TotalMarks = 100,
                Status = AssignmentStatus.Published,
                TeacherId = teacherJohn.Id,
                SubjectId = mathSub.Id,
                ClassId = class10A.Id
            };

            var assignment2 = new Assignment
            {
                Id = new Guid("50000000-0000-0000-0000-000000000002"),
                Title = "Introduction to Python Functions",
                Description = "Write a Python script that calculates factorial recursively and handles negative number edge cases.",
                DueDate = DateTime.UtcNow.AddDays(5),
                TotalMarks = 50,
                Status = AssignmentStatus.Published,
                TeacherId = teacherJohn.Id,
                SubjectId = csSub.Id,
                ClassId = class10A.Id
            };

            var assignment3 = new Assignment
            {
                Id = new Guid("50000000-0000-0000-0000-000000000003"),
                Title = "Newton's Laws of Motion Lab Report",
                Description = "Submit a formal PDF report detailing acceleration observations from Wednesday's physics experiment.",
                DueDate = DateTime.UtcNow.AddDays(10),
                TotalMarks = 100,
                Status = AssignmentStatus.Published,
                TeacherId = teacherSarah.Id,
                SubjectId = sciSub.Id,
                ClassId = class10A.Id
            };

            var assignment4 = new Assignment
            {
                Id = new Guid("50000000-0000-0000-0000-000000000004"),
                Title = "Advanced Derivatives Draft Homework",
                Description = "Draft assignment on limits and derivatives (not visible to students yet).",
                DueDate = DateTime.UtcNow.AddDays(15),
                TotalMarks = 100,
                Status = AssignmentStatus.Draft,
                TeacherId = teacherJohn.Id,
                SubjectId = mathSub.Id,
                ClassId = class10A.Id
            };

            await context.Assignments.AddRangeAsync(assignment1, assignment2, assignment3, assignment4);
            await context.SaveChangesAsync();
        }

        var mathAssignment = await context.Assignments.FirstAsync(a => a.Title.StartsWith("Algebraic"));
        var csAssignment   = await context.Assignments.FirstAsync(a => a.Title.StartsWith("Introduction"));

        // 6. Seed Sample Submissions if none exist
        if (!await context.Submissions.AnyAsync())
        {
            var submission1 = new Submission
            {
                Id = new Guid("60000000-0000-0000-0000-000000000001"),
                AssignmentId = mathAssignment.Id,
                StudentId = studentAlex.Id,
                Content = "Here are my solutions to problems 1 through 10. Q1: x = 4, Q2: x = -2 or 5...",
                FilePath = "/uploads/submissions/alex_math_ps1.pdf",
                Status = SubmissionStatus.Graded,
                SubmittedAt = DateTime.UtcNow.AddDays(-1),
                MarksObtained = 94.5m,
                Feedback = "Excellent effort! Great work showing step-by-step factoring on Q7.",
                GradedAt = DateTime.UtcNow
            };

            var submission2 = new Submission
            {
                Id = new Guid("60000000-0000-0000-0000-000000000002"),
                AssignmentId = mathAssignment.Id,
                StudentId = studentEmma.Id,
                Content = "Attached is my completed algebra worksheet.",
                FilePath = "/uploads/submissions/emma_math_ps1.pdf",
                Status = SubmissionStatus.Submitted,
                SubmittedAt = DateTime.UtcNow.AddHours(-3)
            };

            var submission3 = new Submission
            {
                Id = new Guid("60000000-0000-0000-0000-000000000003"),
                AssignmentId = csAssignment.Id,
                StudentId = studentAlex.Id,
                Content = "def factorial(n):\n    if n < 0:\n        raise ValueError('Negative numbers not allowed')\n    return 1 if n <= 1 else n * factorial(n - 1)",
                Status = SubmissionStatus.Submitted,
                SubmittedAt = DateTime.UtcNow.AddHours(-1)
            };

            await context.Submissions.AddRangeAsync(submission1, submission2, submission3);
            await context.SaveChangesAsync();
        }
    }
}
