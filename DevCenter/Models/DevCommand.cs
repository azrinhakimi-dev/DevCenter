using System.ComponentModel.DataAnnotations;

namespace DevCenter.Models
{
    public class DevCommand
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Script { get; set; }
        public string Tags { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public List<string> TagList =>
            (Tags ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
    }
}