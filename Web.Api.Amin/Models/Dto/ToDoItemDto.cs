namespace Web.Api.Amin.Models.Dto
{
    public class ToDoItemDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime inserttime { get; set; }
        public List<Links> Links { get; set; }
    }
}
