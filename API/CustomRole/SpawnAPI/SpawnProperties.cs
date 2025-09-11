using CustomRolesCrimsonBreach.API.CustomRole.SpawnAPI;
using System.Collections.Generic;

namespace CustomRolesCrimsonBreach.API.CustomRole
{
    public class SpawnProperties
    {
        public List<SpawnPoint> StaticSpawnPoints { get; set; }
        public List<SpawnPoint> DynamicSpawnPoints { get; set; }
        public List<SpawnPoint> RoleSpawnPoints { get; set; }
        public uint Limit { get; set; }

        public int Count() => (StaticSpawnPoints?.Count ?? 0) + (DynamicSpawnPoints?.Count ?? 0) + (RoleSpawnPoints?.Count ?? 0);
    }
}
