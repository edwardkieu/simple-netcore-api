using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Data.Common;

namespace WebApi.Data.Entities
{
    public class TestTable : AuditableEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }
    }
}