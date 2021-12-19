using System.ComponentModel.DataAnnotations;

namespace WellBot.Domain.Chats.Entities
{
    /// <summary>
    /// Possible slap option.
    /// </summary>
    public class SlapOption
    {
        /// <summary>
        /// Option id.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Animation file id.
        /// </summary>
        [MaxLength(150)]
        public string FileId { get; set; }
    }
}
