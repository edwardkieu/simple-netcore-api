using System.Collections.Generic;

namespace WebApi.ViewModels.Account
{
    public class AppUserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public bool IsVerified { get; set; }
    }
}
