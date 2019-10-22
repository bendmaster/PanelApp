using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoilReaderPanel.Policies
{
    public class IsAdminPolicy : IAuthorizationRequirement
    {
        public IsAdminPolicy(bool isAdmin)
        {
            IsAdmin = isAdmin;
        }
        
        public bool IsAdmin { get; }
    }

    public class IsAdminHandler : AuthorizationHandler<IsAdminPolicy>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminPolicy requirement)
        {
            if (context.User.HasClaim(c => c.Type == "isAdmin"))           
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
