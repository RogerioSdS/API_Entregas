namespace Api_Entregas.ViewModels
{
    public class StudentNameIdDTO
    {
        public List<StudentDto> Students { get; set; } = new();
    }

    public class StudentDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // propriedade auxiliar para facilitar no Razor
        public string FullName => $"{FirstName} {LastName}";
    }
}
