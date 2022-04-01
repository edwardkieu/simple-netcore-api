using Microsoft.AspNetCore.Identity;

namespace WebApi.Data.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }

        public virtual TestTable TestTable { get; set; }
    }
}
