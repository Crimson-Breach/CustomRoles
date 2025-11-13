global using Logger = LabApi.Features.Console.Logger;
using CustomRolesCrimsonBreach.Events;
using CustomRolesCrimsonBreach.Intergrations;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using System;

namespace CustomRolesCrimsonBreach;

public class Main : Plugin<Config>
{
    public override string Name => "CrimsonCustomRole";
    public override string Description => "Implementing the creation of CustomRoles for LabAPI";
    public override string Author => "Davilone32";
    public override Version Version => new(1, 2, 2, 3);
    public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;
    public static Main Instance { get; private set; }
    public CustomPlayerHandler playerHandler { get; set; }
    public SSCustomRole Settings { get; private set; }
    public override LoadPriority Priority => LoadPriority.Highest;
    
    public override void Enable()
    {
        Instance = this;
        Settings = new SSCustomRole();
        playerHandler = new CustomPlayerHandler();

        LabApi.Events.Handlers.PlayerEvents.Spawned += playerHandler.OnSpawned;
        LabApi.Events.Handlers.PlayerEvents.Death += playerHandler.OnKillDeath;
        LabApi.Events.Handlers.PlayerEvents.ChangedRole += playerHandler.OnChangedRole;
        LabApi.Events.Handlers.ServerEvents.WaitingForPlayers += OnWaitingForPlayers;

        Settings.Activate();
    }

    public override void Disable()
    {
        Settings.Deactivate();
        LabApi.Events.Handlers.PlayerEvents.Spawned -= playerHandler.OnSpawned;
        LabApi.Events.Handlers.PlayerEvents.ChangedRole -= playerHandler.OnChangedRole;
        LabApi.Events.Handlers.PlayerEvents.Death -= playerHandler.OnKillDeath;
        LabApi.Events.Handlers.ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;

        playerHandler = null;
        Settings = null;
        Instance = null;
    }

    private static void OnWaitingForPlayers()
    {
        CustomItemsAPI.Init();
    }
}
