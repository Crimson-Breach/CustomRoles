using CustomRolesCrimsonBreach.API.CustomRole.SpawnAPI;
using CustomRolesCrimsonBreach.Events;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public virtual bool TryRegister()
    {
        if (CustomRoleHandler.Registered.Any(c => c.Id == Id)) return false;
        CustomRoleHandler.Registered.Add(this);
        LabApi.Features.Console.Logger.Info($"Register: {Name} ({GetHashCode()})");
        return true;
    }

    public virtual bool TryUnregister()
    {
        return CustomRoleHandler.Registered.Remove(this);
    }

    public virtual void EventsCustom()
    {
        LabApi.Events.Handlers.PlayerEvents.ChangedRole += PlayerChangeRole;
    }

    public virtual void UnEventsCustom()
    {
        LabApi.Events.Handlers.PlayerEvents.ChangedRole -= PlayerChangeRole;
    }

    private void PlayerChangeRole(PlayerChangedRoleEventArgs ev)
    {
        if (KeepRoleWithScapeOrSomethingIDK) return;

        if (ev.NewRole.RoleTypeId != BaseRole)
        {
            RemoveRole(ev.Player);
        }
    }

    public virtual void AddRole(Player player)
    {
        if (HasRole(player, this)) return;

        MEC.Timing.CallDelayed(1.5f, () =>
        {
            LabApi.Features.Console.Logger.Debug($"{Name}: Assingning role to {player.Nickname}, Role instance: {GetHashCode()}", Main.Instance.Config.debug);

            if (!_players.TryGetValue(player.UserId, out var roles))
            {
                roles = new HashSet<CustomRole>();
                _players[player.UserId] = roles;
            }

            roles.Add(this);


            player.SendHint(Main.Instance.Config.RoleAdded.Replace("%name%", Name), 10);


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


            LabApi.Features.Console.Logger.Debug($"{Name}: Item added {player.Nickname}", Main.Instance.Config.debug);
            MEC.Timing.CallDelayed(0.5f, () =>
            {
                player.Scale = Scale;
                player.MaxHealth = health;
                player.Health = health;
                player.CustomInfo = $"{CustomInfo}";
            });

            foreach (string itemName in Inventory)
            {
                LabApi.Features.Console.Logger.Info($"{Name}: Adding {itemName} to inventory.");
                TryAddItem(player, itemName);
            }

            foreach (KeyValuePair<ItemType, ushort> ammo in AmmoItems)
            {
                player.AddAmmo(ammo.Key, ammo.Value);
            }
        });


        RoleAdded(player);
        OnAssigned(player);
    }

    public virtual void RoleAdded(Player player) 
    {
    }

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
        LabApi.Features.Console.Logger.Debug($"{Name}: TryAddItem intentando con: {itemName}", Main.Instance.Config.debug);

        if (Enum.TryParse(itemName, out ItemType type))
        {
            LabApi.Features.Console.Logger.Debug($"{Name}: Its a valid ItemType: {type}", Main.Instance.Config.debug);
            player.AddItem(type);
            return true;
        }

        LabApi.Features.Console.Logger.Debug($"{Name}: TryAddItem error. {itemName} its not valid.", Main.Instance.Config.debug);
        return false;
    }
    public virtual void RemoveRole(Player player)
    {
        if (!_players.TryGetValue(player.UserId, out var roles)) return;

        if (!roles.Remove(this)) return;

        if (roles.Count == 0)
            _players.Remove(player.UserId);


        player.SendHint( Main.Instance.Config.RoleRemoved.Replace("%name%", Name) , 10 );
        player.CustomInfo = null;

        RemovedRole(player);
        OnRemoved(player);
    }

    public virtual void RemovedRole(Player player)
    {
    }

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
