using Newspoint.Domain.Entities;

namespace Newspoint.Server.Areas.Admin.DTOs;

public class UserUpdateDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
}
