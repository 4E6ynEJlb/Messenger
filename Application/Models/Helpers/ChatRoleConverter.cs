using Application.Models.Internal.Constants;
using Application.Models.Output;
using Domain.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Helpers
{
    internal static class ChatRoleConverter
    {
        internal static PublicChatMemberRole Convert(EnPublicChatMemberRole role) => role switch
        {
            EnPublicChatMemberRole.Creator => PublicChatMemberRole.Owner,
            EnPublicChatMemberRole.Administrator => PublicChatMemberRole.Administrator,
            EnPublicChatMemberRole.Member => PublicChatMemberRole.Member,
            EnPublicChatMemberRole.Reader => PublicChatMemberRole.Reader,
            _ => throw new Exception("Invalid chat member role")
        };

        internal static EnPublicChatMemberRole Convert(PublicChatMemberRole role) => role switch
        {
            PublicChatMemberRole.Owner => EnPublicChatMemberRole.Creator,
            PublicChatMemberRole.Administrator => EnPublicChatMemberRole.Administrator,
            PublicChatMemberRole.Member => EnPublicChatMemberRole.Member,
            PublicChatMemberRole.Reader => EnPublicChatMemberRole.Reader,
            _ => throw new Exception("Invalid chat member role")
        };
    }
}
