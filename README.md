A API to make a CustomRoles and CustomHabilitys

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




