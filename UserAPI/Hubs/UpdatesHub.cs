using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace UserAPI.Hubs
{
    [Authorize]
    public class UpdatesHub:Hub
    {        
    }
}
