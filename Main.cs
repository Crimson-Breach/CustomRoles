using CustomRolesCrimsonBreach.API.CustomRole;
using CustomRolesCrimsonBreach.Events;
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
}
