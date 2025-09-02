A API to make a CustomRoles and CustomHabilitys for LabApi

# Commands
```
.useskill
```
Command to use the skill
```
.info
```
See the information about the skill you have
```
CustomRoles <Spawn>, <List> or <abilityList> 
```

# Config

```yaml
# Message shown when a player tries to use a command without the required permissions.
dont_have_access: You do not have permission for this command!
# Message displayed when a role is successfully assigned to a player.
role_added: <b><color=yellow>You are <color=blue>%name%</color><color=#3a3a3a>!</color></b>
# Message displayed when a role is removed from a player.
role_removed: <b><color=yellow>You are <color=red>no longer</color> <color=blue>%name%</color><color=#3a3a3a>!</color></b>
# Message shown when the player's CustomRole does not have a skill assigned.
you_dont_have_skill_in_your_custom_role: Your CustomRole does not have an assigned skill
# Message displayed when a player uses their skill.
use_hability: skill used
# Message shown when a player tries to use a skill that is on cooldown.
ability_cooldown_message: The skill is loading, wait MINUTES:SECONDS minutes.
# Message displayed when the skill cooldown has finished and is ready again.
ability_cooldown_succesfull: The skill is ready to be used again!
# Message shown when a player tries to check their skill info without having a CustomRole.
you_need_a_custom_role_message: You need to have a CustomRole to be able to see your skill information
# Enables or disables debug mode (for logging and testing purposes).
debug: false
```

# USAGE
Role Example:
```CS
using AllCustomRoles.HabilidadesCustom;
using CustomRolesCrimsonBreach.API.CustomRole;
using PlayerRoles;
using UnityEngine;

namespace AllCustomRoles.CustomRoles.SCP106
{
    public class _106Stalkie : CustomRole
    {
        public override string Name => "106 Stalkie";
        public override string CustomInfo => "Stalkie";
        public override uint Id => 300;
        public override RoleTypeId BaseRole => RoleTypeId.Scp106;
        public override float SpawnPorcentage => 50;
        public override Vector3 Scale { get; set; } = Vector3.one;
        public override CustomAbility CustomAbility { get; set; } = new _106StalkieHab();
        public override int health { get; set; } = 2500;
    }
}
````
Skill Example:
```CS
using CustomRolesCrimsonBreach.API.CustomRole;
using LabApi.Features.Wrappers;
using PlayerRoles.PlayableScps.Scp106;
using System.Linq;
using UnityEngine;

namespace AllCustomRoles.HabilidadesCustom
{
    internal class _106StalkieHab : CustomAbility
    {
        public override uint ID => 300;

        public override string Name => "Stalk";

        public override float Cooldown => 180;

        public override string Description => "You can stalk anyone as long as you are underground";

        public override bool NeedCooldown => true;
        public override void OnUse(Player player)
        {
            if (player.RoleBase is Scp106Role scp106)
            {

                if (scp106.IsStalking)
                {

                    Player jugadorstalk = Player.List.Where(d => d != player && Vector3.Distance(player.Position, d.Position) < 120).FirstOrDefault();

                    if (jugadorstalk != null)
                    {
                        player.SendHint($"You have stalked to <color=red>{jugadorstalk.Nickname}</color>.", 2);

                        player.Position = jugadorstalk.Position;

                        return;
                    }

                    player.SendHint("<color=red>⚠️</color> There are no players in your area.");
                    return;
                }
                else
                {
                    player.SendHint("Requires you to be underground to use the ability!");
                }
            }

            base.OnUse(player);
        }
    }
}
```
Register Role and Skill:
```CS
    public override void Enable()
    {
        CustomAbility.RegisterSkills();
        CustomRoleHandler.RegisterRoles();
    }

    public override void Disable()
    {
        CustomRoleHandler.UnRegisterRoles();
        CustomAbility.UnRegisterSkills();
    }
```



