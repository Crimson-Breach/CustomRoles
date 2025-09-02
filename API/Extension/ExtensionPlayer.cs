using LabApi.Features.Wrappers;

namespace CustomRolesCrimsonBreach.API.Extension;

public static class ExtensionPlayer
{
    public static bool IsCustomRole(this Player player)
    {
        var role = CustomRole.CustomRole.GetRole(player);
        return role != null;
    }
}