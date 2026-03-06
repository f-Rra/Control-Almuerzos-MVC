using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SCA_MVC.Models;
using System.Security.Claims;

namespace SCA_MVC.Helpers
{
    public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public AppUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor) { }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("FullName", user.NombreCompleto));
            identity.AddClaim(new Claim("NombreUsuario", user.NombreUsuario));
            return identity;
        }
    }
}
