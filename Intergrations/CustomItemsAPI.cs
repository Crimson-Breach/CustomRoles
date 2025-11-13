using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Misc;
using LabApi.Loader.Features.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomRolesCrimsonBreach.Intergrations
{
    public class CustomItemsAPI
    {
        public static MethodInfo GiveCustomItemMethod;
        public static bool Found;
        public static Assembly CustomItemsAPIAssembly;

        public static void Init()
        {
            foreach (Plugin plugin in LabApi.Loader.PluginLoader.EnabledPlugins)
            {
                if (plugin.Name is "CustomItemsAPI")
                {
                    plugin.TryGetLoadedAssembly(out CustomItemsAPIAssembly);
                    Found = true;
                    break;
                }
            }

            if (Found)
            {
                Logger.Debug("CustomItemsAPI was found!", Main.Instance.Config.debug);
                Type customItemType = CustomItemsAPIAssembly.GetType("CustomItemsAPI.CustomItems");
                if (customItemType is not null)
                {
                    List<Type> parameters = [typeof(string), typeof(Player)];
                    GiveCustomItemMethod = customItemType.GetMethod("AddCustomItem", BindingFlags.Static | BindingFlags.Public, null, parameters.ToArray(), null);
                }
            }
        }
#nullable enable
        public static bool TryGiveCustomItem(string name, Player player)
        {
            if (GiveCustomItemMethod is null || !Found)
                return false;

            object? result = GiveCustomItemMethod.Invoke(null, [name, player]);
            return result is not null;
        }
    }
}