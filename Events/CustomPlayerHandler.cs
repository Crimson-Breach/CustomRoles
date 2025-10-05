using CustomRolesCrimsonBreach.API.CustomRole;
using CustomRolesCrimsonBreach.API.Extension;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CustomRolesCrimsonBreach.Events;

public class CustomPlayerHandler
{
    private readonly Dictionary<string, CustomRole> _assignedRoles = new();
    private readonly Dictionary<Type, int> _roleCounts = new();

    public void OnKillDeath(PlayerDeathEventArgs ev)
    {
        if (!ev.Player.IsCustomRole()) return;

        if (!_assignedRoles.TryGetValue(ev.Player.UserId, out var role))
            return;

        role.RemoveRole(ev.Player);
        _assignedRoles.Remove(ev.Player.UserId);
    }

    public void OnSpawned(PlayerSpawnedEventArgs ev)
    {
        if (ev.Player == null) return;

        string playerIdentifier = $"{ev.Player.Nickname} ({ev.Player.UserId})";
        Logger.Debug($"Event triggered for player {playerIdentifier}. Current RoleTypeId: {ev.Role.RoleTypeId}.", false);

        if (_assignedRoles.ContainsKey(ev.Player.UserId))
        {
            CustomRole existingRole = _assignedRoles[ev.Player.UserId];
            Logger.Warn($"Player {playerIdentifier} already has {existingRole.Name}. Reapplying inventory.");

            existingRole.AddRole(ev.Player);
            MEC.Timing.CallDelayed(0.2f, () =>
            {
                if (ev.Player == null || ev.Player.GameObject == null)
                    return;

                existingRole.ReapplyInventory(ev.Player);
            });
            return;
        }

        var candidates = CustomRoleHandler.Registered
            .Where(role =>
                role.BaseRole == ev.Role.RoleTypeId &&
                (role.spawnNumber == 0 || !_roleCounts.TryGetValue(role.GetType(), out var count) || count < role.spawnNumber)
            )
            .ToList();

        if (!candidates.Any())
        {
            Logger.Debug($"No candidate roles available (with spawn limit) for BaseRole {ev.Role.RoleTypeId} for player {playerIdentifier}.", false);
            return;
        }

        Logger.Debug($"Found {candidates.Count} valid candidate roles (respecting spawn limits) for BaseRole {ev.Role.RoleTypeId} for player {playerIdentifier}.", false);

        float totalWeight = candidates.Sum(r => r.SpawnPorcentage);
        float roll = UnityEngine.Random.Range(0f, 100f);

        Logger.Debug($"Roll: {roll}, TotalCustomWeight: {totalWeight}", false);

        if (roll > totalWeight)
        {
            Logger.Debug($"Roll {roll} > {totalWeight}. No custom role will be assigned to {playerIdentifier}.", false);
            return;
        }

        float cumulative = 0f;
        CustomRole? selectedRole = null;
        foreach (var role in candidates)
        {
            cumulative += role.SpawnPorcentage;
            if (roll <= cumulative)
            {
                selectedRole = role;
                break;
            }
        }

        if (selectedRole != null)
        {
            Logger.Debug($"Selected role {selectedRole.Name} (Instance: {selectedRole.GetHashCode()}) for player {playerIdentifier} with roll {roll}.", false);

            _assignedRoles[ev.Player.UserId] = selectedRole;

            if (_roleCounts.TryGetValue(selectedRole.GetType(), out int count))
                _roleCounts[selectedRole.GetType()] = count + 1;
            else
                _roleCounts[selectedRole.GetType()] = 1;

            MEC.Timing.CallDelayed(0.1f, () =>
            {
                Logger.Debug($"Calling AddRole for {selectedRole.Name} on player {playerIdentifier}.", false);
                selectedRole.AddRole(ev.Player);
            });
        }
        else
        {
            Logger.Debug($"No role selected for player {playerIdentifier} after weighted roll.", false);
        }
    }


    public void OnChangedRole(PlayerChangedRoleEventArgs ev)
    {
        if (ev.Player == null) return;

        if (!_assignedRoles.TryGetValue(ev.Player.UserId, out var role))
            return;

        if (ev.NewRole.RoleTypeId != role.BaseRole && !role.KeepRoleWithScapeOrSomethingIDK)
        {
            role.RemoveRole(ev.Player);
            _assignedRoles.Remove(ev.Player.UserId);

            if (_roleCounts.TryGetValue(role.GetType(), out int count))
            {
                _roleCounts[role.GetType()] = Math.Max(0, count - 1);
            }
        }
    }
}
