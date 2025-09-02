﻿using CommandSystem;
using CustomRolesCrimsonBreach.API.CustomRole;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomRolesCrimsonBreach.commands;


[CommandHandler(typeof(ClientCommandHandler))]
public class UseHability : ICommand
{
    public string Command => "usarhabilidad";

    public string[] Aliases => ["uh"];

    public string Description => "Comando para usar la habilidad";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        Player player = Player.Get(sender);

        CustomRole role = CustomRole.GetRole(player);
        if (role != null)
        {
            if (role.CustomAbility == null)
            {
                response = Main.Instance.Config.YouDontHaveSkillInYourCustomRole;
                return false;
            }

            if (role.CustomAbility.NeedCooldown)
            {
                role.CustomAbility?.OnUseWithCooldown(player);
            }
            else
            {
                role.CustomAbility?.OnUse(player);
            }

            response = Main.Instance.Config.UseHability;
            return true;
        }

        response = Main.Instance.Config.YouNeedACustomRoleMessage;
        return false;

    }
}
