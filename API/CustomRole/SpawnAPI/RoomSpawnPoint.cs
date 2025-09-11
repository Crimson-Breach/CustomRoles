using MapGeneration;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomRolesCrimsonBreach.API.CustomRole.SpawnAPI
{
    public class RoomSpawnPoint
    {
        public RoomName Room { get; set; } 
        public Vector3 Offset { get; set; } = Vector3.zero;
    }
}
