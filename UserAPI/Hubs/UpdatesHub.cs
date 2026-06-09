using Application.Models.Internal.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using UserAPI.Models;

namespace UserAPI.Hubs
{
    [Authorize(Policy = Policies.USER_POLICY)]
    public class UpdatesHub:Hub
    {        
        
    }
}
