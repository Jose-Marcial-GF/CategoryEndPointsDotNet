using System;
using Microsoft.AspNetCore.Identity;

namespace ApiEcommerce.Model;

public class ApplicationUser : IdentityUser
{

    public string? Name { get; set; }
}
