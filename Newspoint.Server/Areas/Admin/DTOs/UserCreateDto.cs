using Newspoint.Domain.Entities;

namespace Newspoint.Server.Areas.Admin.DTOs;

public class UserCreateDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
}