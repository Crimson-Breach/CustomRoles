global using Logger = LabApi.Features.Console.Logger;

using CustomRolesCrimsonBreach.API.CustomRole;
using CustomRolesCrimsonBreach.Events;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using System;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace CustomRolesCrimsonBreach;

public class Main : Plugin<Config>
{
    public override string Name => "CrimsonCustomRole";
    public override string Description => "Implementing the creation of Custom Roles for LabAPI";
    public override string Author => "Davilone32";
    public override Version Version => new(1, 0, 2);
    public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;
    public static Main Instance { get; private set; }
    public CustomPlayerHandler playerHandler { get; set; }
    public override void Enable()
    {
        Instance = this;
        playerHandler = new CustomPlayerHandler();

        LabApi.Events.Handlers.PlayerEvents.Spawned += playerHandler.OnSpawned;
        LabApi.Events.Handlers.PlayerEvents.Death += playerHandler.OnKillDeath;
        LabApi.Events.Handlers.PlayerEvents.ChangedRole += playerHandler.OnChangedRole;

        if (Config.AbilityUsage == "ServerConfig" || Config.AbilityUsage == "both")
        {
            ServerSpecificSettingsSync.DefinedSettings = new ServerSpecificSettingBase[2]
            {
                new SSGroupHeader("CustomRole"),
                new SSKeybindSetting(201, "Use hability", KeyCode.Y, preventInteractionOnGui: true, allowSpectatorTrigger: true, "use your hability.")
            };
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSettingsUpdated;
            ServerSpecificSettingsSync.SendToAll();
        }

        CustomAbility.RegisterSkills();
        CustomRoleHandler.RegisterRoles();
    }

    public override void Disable()
    {
        LabApi.Events.Handlers.PlayerEvents.Spawned -= playerHandler.OnSpawned;
        LabApi.Events.Handlers.PlayerEvents.ChangedRole -= playerHandler.OnChangedRole;
        LabApi.Events.Handlers.PlayerEvents.Death -= playerHandler.OnKillDeath;

        CustomRoleHandler.UnRegisterRoles();
        CustomAbility.UnRegisterSkills();
        playerHandler = null;
        Instance = null;
    }

    private static void OnSettingsUpdated(ReferenceHub sender, ServerSpecificSettingBase setting)
    {
        Player player = Player.Get(sender);

        if (player.IsHost)
            return;

        if (setting.CollectionId == 201)
        {

            Logger.Debug("Test", Main.Instance.Config.debug);
            CustomRole role = CustomRole.GetRole(player);
            if (role != null)
            {
                if (role.CustomAbility == null)
                {
                    return;
                }

                if (role.CustomAbility.NeedCooldown)
                {
                    role.CustomAbility?.OnUseWithCooldown(player);
                }
                else
                {
                    role.CustomAbility?.OnUse(player);
                }
            }
        }


    }

}
