using CommandSystem;
using CustomRolesCrimsonBreach.API.CustomRole;
using System;
using System.Linq;
using System.Text;

namespace CustomRolesCrimsonBreach.commands.Child2;

[CommandHandler(typeof(Parent))]
public class ListAbility : ICommand
{
    public string Command => "abilityList";
    public string[] Aliases => new[] { "abl" };
    public string Description => "Lists all registered custom items.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var items = CustomAbility.Registered;

        if (!items.Any())
        {
            response = "There are no custom items registered.";
            return false;
        }

        StringBuilder sb = new();
        sb.AppendLine("Registered Custom Ability:");

        foreach (var item in items)
        {
            sb.AppendLine($"- {item.Name} (ID: {item.ID})");
        }

        response = sb.ToString();
        return true;
    }
}
