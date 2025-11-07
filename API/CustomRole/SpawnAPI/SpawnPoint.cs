using MapGeneration;
using UnityEngine;

namespace CustomRolesCrimsonBreach.API.CustomRole.SpawnAPI;

public class SpawnPoint
{
    public RoomName Room { get; set; }
    public Vector3 Offset { get; set; } = Vector3.zero;
    public Quaternion Rotation { get; set; } = Quaternion.identity;
}
