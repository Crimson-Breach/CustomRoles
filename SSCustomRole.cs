using CustomRolesCrimsonBreach.API.CustomRole;
using LabApi.Features.Wrappers;
using System.Linq;
using UserSettings.ServerSpecific;

namespace CustomRolesCrimsonBreach
{
    public class SSCustomRole
    {
        private SSKeybindSetting ButtonTuUseHability;

        public void Activate()
        {
            ButtonTuUseHability = new SSKeybindSetting(null, "Use Skill" ,Main.Instance.Config.KeyButton);

            var settings = new ServerSpecificSettingBase[2]
            {
                new SSGroupHeader("Custom Roles"),
                ButtonTuUseHability
            };

            if (ServerSpecificSettingsSync.DefinedSettings == null) 
                ServerSpecificSettingsSync.DefinedSettings = settings;
            else
                ServerSpecificSettingsSync.DefinedSettings = ServerSpecificSettingsSync.DefinedSettings.Concat(settings).ToArray();
            ServerSpecificSettingsSync.SendToAll();
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += ProcessUserInput;
        }

        public void Deactivate()
        {
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= ProcessUserInput;
        }

        private void ProcessUserInput(ReferenceHub sender, ServerSpecificSettingBase setting)
        {
            if (Main.Instance.Config.UseButton && setting.SettingId == ButtonTuUseHability.SettingId && (setting is SSKeybindSetting kb && kb.SyncIsPressed))
            {
                Player player = Player.Get(sender);

                CustomRole role = CustomRole.GetRole(player);
                if (role != null)
                {
                    if (role.CustomAbility == null)
                    {
                        return;
                    }

                    if (role.CustomAbility.NeedCooldown)
                    {
                        role.CustomAbility?.OnUseWithCooldown(player);
                    }
                    else
                    {
                        role.CustomAbility?.OnUse(player);
                    }

                    return;
                }
            }
        }
    }
}
