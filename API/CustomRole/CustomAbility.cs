using LabApi.Features.Wrappers;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomRolesCrimsonBreach.API.CustomRole;

public abstract class CustomAbility
{
    public static HashSet<CustomAbility> Registered { get; } = new();
    public virtual HashSet<uint> TrackedSerials { get; } = new();
    private static readonly Dictionary<uint, CustomAbility?> idLookupTable = new();
    private static Dictionary<string, CustomAbility?> stringLookupTable = new();
    private readonly Dictionary<string, float> _lastUseTime = new();
    public static event EventHandler<PlayerUseHability>? PlayerUsedAbility;
    public abstract uint ID { get; }
    public abstract string Name { get; }
    public abstract float Cooldown { get; }
    public abstract string Description { get; }

    public abstract bool NeedCooldown { get; }

    public Dictionary<Player, float> RealCooldown = new Dictionary<Player, float>();
    public virtual bool TryRegister()
    {
        if (Registered.Any(c => c.ID == ID)) return false;

        Registered.Add(this);
        idLookupTable.Add(ID, this);
        stringLookupTable.Add(Name, this);
        return true;
    }

    public virtual bool TryUnregister()
    {
        return Registered.Remove(this);
    }

    public static IEnumerable<CustomAbility> RegisterSkills()
    {
        List<CustomAbility> items = new();
        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            if (!type.IsSubclassOf(typeof(CustomAbility)) || type.IsAbstract)
                continue;

            if (Registered.Any(r => r.GetType() == type))
                continue;

            CustomAbility instance = (CustomAbility)Activator.CreateInstance(type)!;

            if (instance.TryRegister())
            {
                items.Add(instance);
            }
        }

        return items;
    }

    public static IEnumerable<CustomAbility> UnRegisterSkills()
    {
        var items = Registered.ToList();
        foreach (var item in items)
            item.TryUnregister();
        return items;
    }

    public static CustomAbility Get(string name) => stringLookupTable.TryGetValue(name, out var ability) ? ability : null;
    public static CustomAbility Get(uint ID) => idLookupTable.TryGetValue(ID, out var ability) ? ability : null;
    public static CustomAbility? Get(CustomAbility ability) => Registered.FirstOrDefault(a => a.Equals(ability));


    public virtual void OnUseWithCooldown(Player player)
    {
        if (!RealCooldown.ContainsKey(player) || RealCooldown[player] <= 0)
        {
            RealCooldown[player] = Cooldown;

            OnUse(player);

            Timing.RunCoroutine(CooldownCoroutine(player));
        }
        else
        {
            float timeRemaining = Mathf.Ceil(RealCooldown[player]);
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);

            player.SendHint(Main.Instance.Config.AbilityCooldownMessage.Replace("MINUTES", minutes.ToString()).Replace("SECONDS", seconds.ToString()), 5f);
        }
    }

    private IEnumerator<float> CooldownCoroutine(Player player)
    {
        while (RealCooldown[player] > 0)
        {
            RealCooldown[player] -= 1f;
            yield return Timing.WaitForSeconds(1f);
        }

        player.SendHint(Main.Instance.Config.AbilityCooldownSuccesfull, 5f);
    }

    public virtual void OnUse(Player player)
    {
        var args = new PlayerUseHability(player, this);
        PlayerUsedAbility?.Invoke(this, args);

        if (!args.IsAllowed)
            return;
    }
}