namespace CustomRolesCrimsonBreach;

public sealed class Config
{
    public string DontHaveAccess { get; set; } = "You do not have permission for this command!";
    public string RoleAdded { get; set; } = "<b><color=yellow>You are <color=blue>%name%</color><color=#3a3a3a>!</color></b>";
    public string RoleRemoved { get; set; } = "<b><color=yellow>You are <color=red>no longer</color> <color=blue>%name%</color><color=#3a3a3a>!</color></b>";




    public string YouDontHaveSkillInYourCustomRole { get; set; } = "Your CustomRole does not have an assigned skill";
    public string UseHability { get; set; } = "skill used";
    public string AbilityCooldownMessage { get; set; } = "The skill is loading, wait MINUTES:SECONDS minutes.";
    public string AbilityCooldownSuccesfull { get; set; } = "The skill is ready to be used again!";
    public string YouNeedACustomRoleMessage { get; set; } = "You need to have a CustomRole to be able to see your skill information";



    public bool debug { get; set; } = false;

}
