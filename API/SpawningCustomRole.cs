using CustomRolesCrimsonBreach.API.CustomRole;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomRolesCrimsonBreach.API
{
    public class SpawningCustomRole : EventArgs
    {
        public Player Player { get; }
        public bool IsAllowed { get; set; } = true;
        public CustomRole.CustomRole customRole { get; set; }

        public SpawningCustomRole(Player player, CustomRole.CustomRole customRole)
        {
            Player = player;
            this.customRole = customRole;
        }
    }
}
