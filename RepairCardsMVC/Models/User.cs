using Microsoft.AspNetCore.Identity;

namespace RepairCardsMVC.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public int Department { get; set; }
    }
}
