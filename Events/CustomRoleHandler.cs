using CustomRolesCrimsonBreach.API.CustomRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CustomRolesCrimsonBreach.Events;

public class CustomRoleHandler
{
    public static HashSet<CustomRole> Registered { get; } = new();
    private static readonly Dictionary<uint, CustomRole?> idLookupTable = new();

    public static CustomRole? Get(uint id)
    {
        if (!idLookupTable.ContainsKey(id))
            idLookupTable[id] = Registered.FirstOrDefault(i => i.Id == id);
        return idLookupTable[id];
    }

    public static IEnumerable<CustomRole> RegisterRoles(bool skipReflection = false, object? overrideClass = null)
    {
        List<CustomRole> items = new();
        Assembly assembly = Assembly.GetCallingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsSubclassOf(typeof(CustomRole)))
                continue;
        }

        return items;
    }

    public static IEnumerable<CustomRole> RegisterRoles()
    {
        List<CustomRole> items = new();

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray();
            }

            foreach (Type type in types)
            {
                if (!type.IsSubclassOf(typeof(CustomRole)) || type.IsAbstract)
                    continue;

                if (Registered.Any(r => r.GetType() == type))
                    continue;

                if (Activator.CreateInstance(type) is CustomRole instance && instance.TryRegister())
                {
                    instance.EventsCustom();
                    items.Add(instance);
                }
            }
        }

        return items;
    }

    public static IEnumerable<CustomRole> UnRegisterRoles()
    {
        var items = Registered.ToList();
        foreach (var item in items)
            item.TryUnregister();
        return items;
    }

    public static IEnumerable<CustomRole> UnRegisterRoles(IEnumerable<Type> types, bool isIgnored = false)
    {
        var roles = Registered.Where(ci =>
            (!isIgnored && types.Contains(ci.GetType())) ||
            (isIgnored && !types.Contains(ci.GetType()))).ToList();

        foreach (var role in roles)
            role.TryUnregister();

        return roles;
    }

    private static IEnumerable<CustomRole> UnregisterItems(IEnumerable<Type> enumerable, bool isIgnored = false) =>
        UnregisterItems(enumerable.Select(i => i.GetType()), isIgnored);

}
