using CommandSystem;
using CustomRolesCrimsonBreach.API.CustomRole;
using LabApi.Features.Wrappers;
using System;

namespace CustomRolesCrimsonBreach.commands;

[CommandHandler(typeof(ClientCommandHandler))]
public class Info : ICommand
{
    public string Command => "info";

    public string[] Aliases => [ "inf" ];

    public string Description => "See the information about the skill you have";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get(sender);

        CustomRole role = CustomRole.GetRole(player);
        if (role != null)
        {
            if (role.CustomAbility == null)
            {
                response = "Your CustomRole does not have an assigned skill";
                return false;
            }

            response = role.CustomAbility.Description;
            return true;
        }

        response = "You need to have a CustomRole to be able to see your skill information";
        return false;
    }
}
