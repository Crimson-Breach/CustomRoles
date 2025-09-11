using CustomRolesCrimsonBreach.API.CustomRole.SpawnAPI;
using LabApi.Features.Wrappers;
using System.Linq;
using UnityEngine;

namespace CustomRolesCrimsonBreach.API.CustomRole
{
    public static class RoomUtils
    {
        public static Vector3 GetSpawnPosition(SpawnPoint spawnPoint)
        {
            var room = Map.Rooms.FirstOrDefault(r => r.Name == spawnPoint.Room);
            if (room == null)
            {
                LabApi.Features.Console.Logger.Warn($"[SpawnPoint] No se encontró la sala {spawnPoint.Room}");
                return Vector3.zero;
            }

            return room.Position + spawnPoint.Offset;
        }

    }

}
