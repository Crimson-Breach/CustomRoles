using UnityEngine;

namespace CustomRolesCrimsonBreach.API.Hat
{
    public class SchematicConfig
    {
        public string SchematicName { get; set; }
        public bool IsSchematicVisibleForOwner { get; set; } = true;
        public bool IsAttachToCamera { get; set; } = false;
        public Vector3 Offset { get; set; } = Vector3.zero;
        public Vector3 Rotation { get; set; } = Vector3.zero;
        public Vector3 Scale { get; set; } = Vector3.one;
    }
}
