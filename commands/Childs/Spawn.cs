using CommandSystem;
using CustomRolesCrimsonBreach.Events;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomRolesCrimsonBreach.commands.Childs
{
    [CommandHandler(typeof(Parent))]
    public class Spawn : ICommand
    {
        public string Command => "spawn";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Comando para spawn customrole";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player executor = Player.Get(sender);

            if (!executor.HasPermissions("customitems.give"))
            {
                response = Main.Instance.Config.DontHaveAccess;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "Usage: spawn <CustomRole ID/Name> [player name/UserID/*]";
                return false;
            }

            string roleName = arguments.At(0);

            var role = CustomRoleHandler.Registered.FirstOrDefault(i =>
                i.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase) ||
                i.Id.ToString() == roleName);

            if (role == null)
            {
                response = $"Custom role '{roleName}' not found.";
                return false;
            }

            List<Player> targets = new List<Player>();

            if (arguments.Count == 1)
            {
                if (executor != null)
                    targets.Add(executor);

                else
                {
                    response = "You must specify a player when running from console.";
                    return false;
                }
            }
            else
            {
                string targetArg = arguments.At(1);

                if (targetArg == "*" || targetArg.ToLower() == "all")
                {
                    targets = Player.List.ToList();
                }
                else
                {
                    Player targetById = Player.List.FirstOrDefault(p => p.UserId == targetArg);
                    if (targetById != null)
                    {
                        targets.Add(targetById);
                    }
                    else
                    {
                        // Buscar por nombre parcial
                        targets = Player.List
                            .Where(p => p.Nickname.IndexOf(targetArg, StringComparison.OrdinalIgnoreCase) >= 0)
                            .ToList();
                    }
                }
            }

            if (targets.Count == 0)
            {
                response = "No valid target players found.";
                return false;
            }

            foreach (Player target in targets)
            {
                role.AddRole(target);
            }

            response = $"Gave {role.Name} to {targets.Count} player(s).";
            return true;
        }
    }
}