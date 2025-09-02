using CommandSystem;
using CustomRolesCrimsonBreach.Events;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomRolesCrimsonBreach.commands.Childs;

[CommandHandler(typeof(Parent))]
public class Spawn : ICommand
{
    public string Command => "spawn";

    public string[] Aliases => [];

    public string Description => "comando para spawn customrole";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player pl = Player.Get(sender);

        if (!pl.HasPermissions("customitems.give"))
        {
            //response = "Permission Denied: Requires 'customitems.give'";
            response = Main.Instance.Config.DontHaveAccess;
            return false;
        }

        if (arguments.Count == 0)
        {
            response = "Usage: give <item name or ID> [player name/UserID/*]";
            return false;
        }

        string roleName = arguments.At(0);

        if (!CustomRoleHandler.Registered.Any())
        {
            response = "No CustomRoles registered.";
            return false;
        }

        var Role = CustomRoleHandler.Registered.FirstOrDefault(i =>
            i.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase) ||
            i.Id.ToString() == roleName);

        if (Role is null)
        {
            response = $"Custom role '{roleName}' not found.";
            return false;
        }

        if (arguments.Count == 1)
        {
            if (sender is Player pcs)
            {

                if (pcs is null)
                {
                    response = "Error: Target player not found.";
                    return false;
                }

                Role.AddRole(pcs);
                response = $"Gave {Role.Name} to {pcs.Nickname}.";
                return true;
            }

            response = "Please specify a player when running from console.";
            return false;
        }

        string targetArg = string.Join(" ", arguments.Skip(1));

        IEnumerable<Player> targets = targetArg is "*" or "all"
            ? Player.List.Where(IsEligible)
            : Player.List.Where(p =>
                p.Nickname.IndexOf(targetArg, StringComparison.OrdinalIgnoreCase) >= 0 ||
                p.UserId.IndexOf(targetArg, StringComparison.OrdinalIgnoreCase) >= 0);

        int givenCount = 0;
        foreach (Player player in targets)
        {
            if (IsEligible(player))
            {
                Role.AddRole(player);
                givenCount++;
            }
        }

        if (givenCount == 0)
        {
            Player player = Player.Get(sender);
            if (IsEligible(player))
            {
                Role.AddRole(player);
                givenCount++;
            }

            response = $"Gave {Role.Name} to {givenCount} player(s).";
            return false;
        }

        response = $"Gave {Role.Name} to {givenCount} player(s).";
        return true;
    }

    private bool IsEligible(Player player) =>
        player.IsAlive;
}
