using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using OilCaseApi.Models;

namespace OilCaseApi.ViewModels;

public class CreateUserViewModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public int? TeamId { get; set; }
    public Team? Team { get; set; }
}
public class EditUserViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public int? TeamId { get; set; }
    public Team? Team { get; set; }
    public string[]? RoleNames { get; set; }
    public IdentityRole[]? Roles { get; set; }
}