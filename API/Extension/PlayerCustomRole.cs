using LabApi.Features.Wrappers;

namespace CustomRolesCrimsonBreach.API.Extension;

public class PlayerCustomRole
{
    private readonly Player _player;

    public PlayerCustomRole(Player player)
    {
        _player = player;
    }

    public bool IsCustomRole
    {
        get => CustomRole.CustomRole.GetRole(_player) != null;
    }

    public void AddRole(int IDCustomRole)
    {
        var customrole = CustomRole.CustomRole.GetRole(IDCustomRole);
        customrole?.AddRole(_player);
    }
    public void RemoveRole()
    {
        var currentRole = CustomRole.CustomRole.GetRole(_player);
        currentRole?.RemoveRole(_player);
    }

    public void ChangeRole(int newRoleId)
    {
        RemoveRole();
        AddRole(newRoleId);
    }

    public CustomRole.CustomRole? GetCurrentRole()
    {
        return CustomRole.CustomRole.GetRole(_player);
    }

    public string? RoleName
    {
        get => GetCurrentRole()?.Name;
    }
}
