using System.ComponentModel;
using UnityEngine;

namespace CustomRolesCrimsonBreach;

public sealed class Config
{
    [Description("Message shown when a player tries to use a command without the required permissions.")]
    public string DontHaveAccess { get; set; } = "You do not have permission for this command!";

    [Description("Message displayed when a role is successfully assigned to a player.")]
    public string RoleAdded { get; set; } = "<b><color=yellow>You are <color=blue>%name%</color><color=#3a3a3a>!</color></b>";

    [Description("Message displayed when a role is removed from a player.")]
    public string RoleRemoved { get; set; } = "<b><color=yellow>You are <color=red>no longer</color> <color=blue>%name%</color><color=#3a3a3a>!</color></b>";
    [Description("Message shown when the player's CustomRole does not have a skill assigned.")]
    public string YouDontHaveSkillInYourCustomRole { get; set; } = "Your CustomRole does not have an assigned skill";

    [Description("Message displayed when a player uses their skill.")]
    public string UseHability { get; set; } = "skill used";
    [Description("Use button")]
    public bool UseButton { get; set; } = true;
    [Description("KeyCode To use")]
    public KeyCode KeyButton { get; set; } = KeyCode.R;

    [Description("Message shown when a player tries to use a skill that is on cooldown.")]
    public string AbilityCooldownMessage { get; set; } = "The skill is loading, wait MINUTES:SECONDS minutes.";

    [Description("Message displayed when the skill cooldown has finished and is ready again.")]
    public string AbilityCooldownSuccesfull { get; set; } = "The skill is ready to be used again!";

    [Description("Message shown when a player tries to check their skill info without having a CustomRole.")]
    public string YouNeedACustomRoleMessage { get; set; } = "You need to have a CustomRole to be able to see your skill information";

    [Description("Configuration option: Hint, broadcast, or Both.")]
    public string ShowMessage { get; set; } = "both";

    [Description("Enables or disables debug mode (for logging and testing purposes).")]
    public bool debug { get; set; } = false;

    [Description("Enables or disables friendlyfire mode.")]
    public bool FriendlyFire { get; set; } = false;
    public float DefaultHumanSpawnChance { get; set; } = 40f;
}
