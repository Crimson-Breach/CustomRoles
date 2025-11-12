using CommandSystem;
using CustomRolesCrimsonBreach.API.CustomRole;
using LabApi.Features.Wrappers;
using System;

namespace CustomRolesCrimsonBreach.commands;


[CommandHandler(typeof(ClientCommandHandler))]
public class UseHability : ICommand
{
    public string Command => "useskill";

    public string[] Aliases => ["uh"];

    public string Description => "Comando para usar la habilidad";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get(sender);

        CustomRole role = CustomRole.GetRole(player);
        if (role != null)
        {
            if (role.CustomHability == null)
            {
                response = Main.Instance.Config.YouDontHaveSkillInYourCustomRole;
                return false;
            }

            if (role.CustomHability.NeedCooldown)
            {
                role.CustomHability?.OnUseWithCooldown(player);
            }
            else
            {
                role.CustomHability?.OnUse(player);
            }

            response = Main.Instance.Config.UseHability;
            return true;
        }

        response = Main.Instance.Config.YouNeedACustomRoleMessage;
        return false;

    }
}
