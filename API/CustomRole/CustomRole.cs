using CustomRolesCrimsonBreach.API.CustomRole.SpawnAPI;
using CustomRolesCrimsonBreach.API.Extension;
using CustomRolesCrimsonBreach.Events;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp049Events;
using LabApi.Features.Wrappers;
using PlayerRoles;
using RemoteAdmin.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utf8Json.Formatters;

namespace CustomRolesCrimsonBreach.API.CustomRole;

public abstract class CustomRole
{
    protected static Dictionary<string, HashSet<CustomRole>> _players = new();
    public event EventHandler<SpawningCustomRole>? Spawning;
    public abstract string Name { get; }
    public abstract string CustomInfo { get; }
    public abstract uint Id { get; }
    public abstract RoleTypeId BaseRole { get; }
    public abstract float SpawnPorcentage { get; }
    public abstract Vector3 Scale { get; set; }
    public abstract SpawnProperties SpawnProperties { get; set; }
    public virtual bool DisplayMessageRole { get; set; } = true;
    public virtual bool KeepRoleWithScapeOrSomethingIDK => false;
    public virtual List<string> Inventory { get; set; } = new();
    public virtual Dictionary<ItemType, ushort> AmmoItems { get; set; } = new();
    public virtual CustomAbility CustomAbility { get; set; }
    public virtual int health { get; set; } = 100;
    public virtual int spawnNumber { get; set; } = 0;
    public virtual bool GiveOnlyTheAbility { get; set;} = false;
    public virtual Team RolTeam { get; set; }

    public virtual bool TryRegister()
    {
        if (CustomRoleHandler.Registered.Any(c => c.Id == Id)) return false;
        CustomRoleHandler.Registered.Add(this);
        Logger.Info($"Register: {Name} ({GetHashCode()})");
        return true;
    }

    public virtual bool TryUnregister()
    {
        return CustomRoleHandler.Registered.Remove(this);
    }

    public virtual void EventsCustom()
    {
        LabApi.Events.Handlers.Scp049Events.Attacking += OnPlagueHurting;
        LabApi.Events.Handlers.PlayerEvents.Hurting += OnPlayerHurt;
        LabApi.Events.Handlers.PlayerEvents.ChangedRole += PlayerChangeRole;
        LabApi.Events.Handlers.PlayerEvents.Spawned += AddRoleEvent;
    }

    public virtual void UnEventsCustom()
    {
        LabApi.Events.Handlers.Scp049Events.Attacking -= OnPlagueHurting;
        LabApi.Events.Handlers.PlayerEvents.Hurting -= OnPlayerHurt;
        LabApi.Events.Handlers.PlayerEvents.ChangedRole -= PlayerChangeRole;
        LabApi.Events.Handlers.PlayerEvents.Spawned -= AddRoleEvent;
    }

    private void OnPlayerHurt(PlayerHurtingEventArgs ev) 
    {
        if (ev.Player == null) return;
        if (ev.Attacker == null) return;
        if (RolTeam == null) return;

        if (ev.Player.Team == RolTeam)
        {
            ev.IsAllowed = false;
            return;
        }

        if (ev.Attacker.Team == RolTeam)
        {
            ev.IsAllowed = false;
            return;
        }
    }

    private void OnPlagueHurting(Scp049AttackingEventArgs ev) 
    {
        if (ev.Player ==  null) return;
        if (RolTeam == null) return;
            
        if (ev.Player.Team == RolTeam)
        {
            ev.IsAllowed = false;
            return;
        }
    }

    private void PlayerChangeRole(PlayerChangedRoleEventArgs ev)
    {
        if (KeepRoleWithScapeOrSomethingIDK) return;

        if (ev.Player.IsSCP)
        {
            ev.Player.CustomInfo = null;
            RemoveRole(ev.Player);
        }

        if (ev.NewRole.RoleTypeId != BaseRole)
        {
            ev.Player.CustomInfo = null;
            RemoveRole(ev.Player);
        }
    }

    public virtual void AddRole(Player player)
    {
        if (HasRole(player, this)) return;

        Logger.Debug($"{Name}: Assingning role to {player.Nickname}, Role instance: {GetHashCode()}", Main.Instance.Config.debug);

        if (!_players.TryGetValue(player.UserId, out var roles))
        {
            roles = new HashSet<CustomRole>();
            _players[player.UserId] = roles;
        }

        roles.Add(this);

        if (DisplayMessageRole)
        {
            string message = Main.Instance.Config.RoleAdded.Replace("%name%", Name);
            string mode = Main.Instance.Config.ShowMessage.ToLower();

            switch (mode)
            {
                case "hint":
                    player.SendHint(message, 10);
                    break;

                case "broadcast":
                    player.SendBroadcast(message, 10);
                    break;

                case "both":
                default:
                    player.SendHint(message, 10);
                    player.SendBroadcast(message, 10);
                    break;
            }
        }

        if (!GiveOnlyTheAbility)
        {
            player.SetRole(BaseRole);

            var spawnPoint = GetRandomSpawnPoint();
            if (spawnPoint != null)
            {
                player.Position = RoomUtils.GetSpawnPosition(spawnPoint);
                player.Rotation = spawnPoint.Rotation;
            }
        }

            Logger.Info($"{Name}: Inventory tiene {Inventory.Count} ítems.");
        foreach (string itemName in Inventory)
        {
            Logger.Info($"{Name}: Adding {itemName} to inventory.");
            TryAddItem(player, itemName);
        }

        foreach (KeyValuePair<ItemType, ushort> ammo in AmmoItems)
        {
            player.AddAmmo(ammo.Key, ammo.Value);
        }

        Logger.Debug($"{Name}: Item added {player.Nickname}", Main.Instance.Config.debug);

    }

    public void ReapplyInventory(Player player)
    {
        foreach (string itemName in Inventory)
        {
            Logger.Info($"{Name}: Reapplying {itemName} to inventory.");
            TryAddItem(player, itemName);
        }

        foreach (var ammo in AmmoItems)
        {
            player.AddAmmo(ammo.Key, ammo.Value);
        }
    }

    private void AddRoleEvent(PlayerSpawnedEventArgs ev)
    {
        if (!HasRole(ev.Player, this)) return;

        MEC.Timing.CallDelayed(0.1f, () =>
        {
            if (ev.Player == null || ev.Player.GameObject == null) return;

            ev.Player.Scale = this.Scale;
            ev.Player.MaxHealth = this.health;
            ev.Player.Health = this.health;
            if (HasRole(ev.Player, this))
            {
                ev.Player.CustomInfo = $"{this.CustomInfo}";
            } 

            OnAssigned(ev.Player);
            RoleAdded(ev.Player);
        });
    }

    public virtual void RoleAdded(Player player) { }
    private SpawnPoint? GetRandomSpawnPoint()
    {
        if (SpawnProperties == null || SpawnProperties.StaticSpawnPoints.Count == 0)
            return null;

        return SpawnProperties.StaticSpawnPoints[
            UnityEngine.Random.Range(0, SpawnProperties.StaticSpawnPoints.Count)
        ];
    }

    protected bool TryAddItem(Player player, string itemName)
    {
        Logger.Debug($"{Name}: TryAddItem intentando con: {itemName}", Main.Instance.Config.debug);

        if (Enum.TryParse(itemName, out ItemType type))
        {
            Logger.Debug($"{Name}: Its a valid ItemType: {type}", Main.Instance.Config.debug);
            player.AddItem(type);
            return true;
        }

        Logger.Debug($"{Name}: TryAddItem error. {itemName} its not valid.", Main.Instance.Config.debug);
        return false;
    }
    public virtual void RemoveRole(Player player)
    {
        if (!_players.TryGetValue(player.UserId, out var roles)) return;

        if (!roles.Remove(this)) return;

        if (roles.Count == 0)
            _players.Remove(player.UserId);

        if (DisplayMessageRole) 
        {
            player.SendHint(Main.Instance.Config.RoleRemoved.Replace("%name%", Name), 10);
        }

        RemovedRole(player);
        OnRemoved(player);
    }

    public virtual void RemovedRole(Player player) { }

    public static bool HasRole(Player player, CustomRole role)
    {
        return _players.TryGetValue(player.UserId, out var roles) && roles.Contains(role);
    }
    public static CustomRole GetRole(int id) => CustomRoleHandler.Registered.FirstOrDefault(r => r.Id == id);
    public static CustomRole GetRole(Type type) => CustomRoleHandler.Registered.FirstOrDefault(r => r.GetType() == type);
    public static CustomRole GetRole(Player player)
    {
        return CustomRoleHandler.Registered.FirstOrDefault(role => CustomRole.HasRole(player, role));
    }
    public virtual void HandleRoleChange(Player player, RoleTypeId newRole)
    {
        if (!KeepRoleWithScapeOrSomethingIDK && newRole != BaseRole)
        {
            RemoveRole(player);
        }
    }

    protected virtual void OnAssigned(Player player) { }
    protected virtual void OnRemoved(Player player) { }

    public static implicit operator HashSet<object>(CustomRole v)
    {
        throw new NotImplementedException();
    }
}
