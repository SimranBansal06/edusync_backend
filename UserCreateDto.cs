// DTOs/UserCreateDto.cs
namespace webapi.DTOs
{
    public class UserCreateDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string PasswordHash { get; set; } = null!;
    }
}

// DTOs/AssessmentCreateDto.cs
namespace webapi.DTOs
{
    public class AssessmentCreateDto
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string Questions { get; set; } = null!;
        public int MaxScore { get; set; }
    }
}

// DTOs/CourseCreateDto.cs
namespace webapi.DTOs
{
    public class CourseCreateDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid InstructorId { get; set; }
        public string MediaUrl { get; set; } = null!;
    }
}

// DTOs/ResultCreateDto.cs
namespace webapi.DTOs
{
    public class ResultCreateDto
    {
        public Guid AssessmentId { get; set; }
        public Guid UserId { get; set; }
        public int Score { get; set; }
        public DateTime AttemptDate { get; set; }
    }
}