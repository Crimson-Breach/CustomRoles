using CommandSystem;
using CustomRolesCrimsonBreach.Events;
using System;
using System.Linq;
using System.Text;

namespace CustomRolesCrimsonBreach.commands.Childs;


[CommandHandler(typeof(Parent))]
public class List : ICommand
{
    public string Command => "list";
    public string[] Aliases => new[] { "cilist", "cil" };
    public string Description => "Lists all registered custom items.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var items = CustomRoleHandler.Registered;

        if (!items.Any())
        {
            response = "There are no CustomRoles registered.";
            return false;
        }

        StringBuilder sb = new();
        sb.AppendLine("Registered CustomRoles:");

        foreach (var item in items)
        {
            sb.AppendLine($"- {item.Name} (ID: {item.Id}, Role: {item.BaseRole})");
        }

        response = sb.ToString();
        return true;
    }
}
