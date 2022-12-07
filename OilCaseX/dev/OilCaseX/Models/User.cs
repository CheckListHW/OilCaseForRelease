using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OilCaseApi.Models
{
    internal struct StandardRoles
    {
        public const string Admin = "Admin";
        public const string Captain = "Captain";
        public const string Regular = "Regular";
    }

    public class Team : IValidatableObject
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public int? LithologicalModelId { get; set; }
        public LithologicalModel? LithologicalModel { get; set; }
        public List<PurchasedObjectOfArrangement>? PurchasesObjectsOfArrangement { get; set; }
        public List<PurchasedSeismic>? PurchasesSeismic { get; set; }
        public List<PurchasedBoreholeExploration>? PurchasedBoreholeExplorations { get; set; }
        public List<PurchasedBoreholeProduction>? PurchasedBoreholeProductions { get; set; }
        public List<User>? Users { get; set; }
        public double Money { get; set; } = 0;
        public double Days { get; set; } = 0;
        public int GameStep { get; set; } = 1;

        //private IValidatableObject _validatableObjectImplementation;
        public IEnumerable<ValidationResult> Validate(ValidationContext _context)
        {
            using ApplicationContext db = new();
            List<ValidationResult> validationResult = new();
            if (db.Team.FirstOrDefault(x => x.Name == Name & x.Id != Id) != null)
                validationResult.Add(new ValidationResult
                    ($"Team {Name} already exists.", new[] { "Name" }));

            return validationResult;
        }
    }

    public class User : IdentityUser
    {
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
    }

    public class TeamAction
    {
        [Key] public int Id { get; set; }
        public Team Team { get; set; }
        public string TypeAction { get; set; }
        public string Descripton { get; set; }
    }

    public class TeamActionLogs
    {
        [Key] public int Id { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public string TypeAction { get; set; }
        public string Description { get; set; }
        public string Time { get; set; }
    }

    public interface IJwtGenerator
    {
        string CreateToken(User user);
    }
}
