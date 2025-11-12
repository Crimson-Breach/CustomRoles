using LabApi.Features.Wrappers;
using System;

namespace CustomRolesCrimsonBreach.API.CustomRole;

public class PlayerUseHability : EventArgs
{
    public Player Player { get; }
    public bool IsAllowed { get; set; } = true;
    public CustomHability customAbility { get; set; }

    public PlayerUseHability(Player player, CustomHability customAbility)
    {
        Player = player;
        this.customAbility = customAbility;
    }
}
