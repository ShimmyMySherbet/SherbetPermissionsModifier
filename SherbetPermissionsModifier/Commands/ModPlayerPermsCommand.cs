using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;

namespace SherbetPermissionsModifier.Commands
{
    public class ModPlayerPermsCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "ModPlayerPerms";

        public string Help => "Modifies a players permissions";

        public string Syntax => "ModPlayerPerms <Grant/Deny/Reset/List> <player> <Permission>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "SherbetPermissionsModifier.ModPlayerPerms" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length >= 2)
            {
                string pl = command[1];
                string action = command[0];

                ulong TargetPlayer;

                if (!ulong.TryParse(pl, out TargetPlayer))
                {
                    UnturnedPlayer localPlayer = UnturnedPlayer.FromName(pl);
                    if (localPlayer != null)
                    {
                        TargetPlayer = localPlayer.CSteamID.m_SteamID;
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "Failed to find player");
                        return;
                    }
                }

                if (string.Equals("Grant", action, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (caller.HasPermission("SherbetPermissionsModifier.Grant"))
                    {
                        if (command.Length >= 3)
                        {
                            bool allow = true;
                            if (SherbetPermissionsModifier.Instance.CheckPlayerExemptions(TargetPlayer, out bool r, out bool p))
                            {
                                Console.WriteLine($"player has exemps, R: {r}, P: {p}");
                                allow = p;
                            }
                            if (allow)
                            {
                                SherbetPermissionsModifier.Instance.RemovePermissionModifiers(TargetPlayer, command[2]);
                                SherbetPermissionsModifier.Instance.AddPermissionModifier(TargetPlayer, command[2], true);
                                UnturnedChat.Say(caller, $"Granted player {TargetPlayer} permission {command[2]}.");
                            }
                            else
                            {
                                UnturnedChat.Say(caller, "You cannot grant permissions to this player.");
                                return;
                            }
                        }
                        else
                        {
                            UnturnedChat.Say(caller, "Missing parameter: Permission.");
                            return;
                        }
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "You do not have permission to grant permissions.");
                        return;
                    }
                }
                else if (string.Equals("Deny", action, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (caller.HasPermission("SherbetPermissionsModifier.Deny"))
                    {
                        if (command.Length >= 3)
                        {
                            bool allow = true;
                            if (SherbetPermissionsModifier.Instance.CheckPlayerExemptions(TargetPlayer, out bool r, out bool p))
                            {
                                Console.WriteLine($"player has exemps, R: {r}, P: {p}");
                                allow = r;
                            }
                            if (allow)
                            {
                                SherbetPermissionsModifier.Instance.RemovePermissionModifiers(TargetPlayer, command[2]);
                                SherbetPermissionsModifier.Instance.AddPermissionModifier(TargetPlayer, command[2], false);
                                UnturnedChat.Say(caller, $"Denyed player {TargetPlayer} permission {command[2]}.");
                            }
                            else
                            {
                                UnturnedChat.Say(caller, "You cannot deny permissions to this player.");
                                return;
                            }
                        }
                        else
                        {
                            UnturnedChat.Say(caller, "Missing parameter: Permission.");
                            return;
                        }
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "You do not have permission to deny permissions.");
                        return;
                    }
                }
                else if (string.Equals("Reset", action, StringComparison.InvariantCultureIgnoreCase))
                {

                    if (caller.HasPermission("SherbetPermissionsModifier.Reset"))
                    {
                        if (command.Length >= 3)
                        {
                            bool allow = true;
                            if (SherbetPermissionsModifier.Instance.CheckPlayerExemptions(TargetPlayer, out bool r, out bool p))
                            {
                                allow = p;
                            }
                            if (allow)
                            {
                                SherbetPermissionsModifier.Instance.RemovePermissionModifiers(TargetPlayer, command[2]);
                                UnturnedChat.Say(caller, $"Reset player permission modifiers for permission {command[2]}.");
                            }
                            else
                            {
                                UnturnedChat.Say(caller, "You cannot reset permissions for this player.");
                                return;
                            }
                        }
                        else
                        {
                            UnturnedChat.Say(caller, "Missing parameter: Permission.");
                            return;
                        }
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "You do not have permission to reset permissions.");
                        return;
                    }

                }
                else if (string.Equals("List", action, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!caller.HasPermission("SherbetPermissionsModifier.List"))
                    {
                        UnturnedChat.Say(caller, "You do not have permission to list modifiers");
                        return;
                    }
                    foreach(var perm in SherbetPermissionsModifier.Config.Players.Where(x => x.Player == TargetPlayer))
                    {
                        UnturnedChat.Say(caller, $"Permission: {perm.Permission} ; Allowed: {perm.Allow}");
                    }
                }
            } else if (command.Length > 0 && string.Equals(command[0], "reload", StringComparison.InvariantCultureIgnoreCase))
            {
                if (caller.HasPermission("SherbetPermissionsModifier.Reload"))
                {
                    SherbetPermissionsModifier.Instance.Configuration.Load();
                    SherbetPermissionsModifier.Config = SherbetPermissionsModifier.Instance.Configuration.Instance;
                    UnturnedChat.Say(caller, "Modifiers reloaded.");
                } else
                {
                    UnturnedChat.Say(caller, "You do not have permission to reload modifiers");
                    return;
                }
            } else
            {
                UnturnedChat.Say(caller, Syntax);
            }
        }
    }
}