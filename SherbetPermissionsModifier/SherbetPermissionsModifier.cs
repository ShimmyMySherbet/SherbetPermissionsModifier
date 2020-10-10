using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using SherbetPermissionsModifier.Models;

namespace SherbetPermissionsModifier
{
    public class SherbetPermissionsModifier : RocketPlugin<ModifierConfig>
    {
        public static SherbetPermissionsModifier Instance;
        public static ModifierConfig Config;
        public static IRocketPermissionsProvider UnderlyingProvider;

        private static bool Installed = false;

        public override void LoadPlugin()
        {
            base.LoadPlugin();
            Instance = this;
            Config = Configuration.Instance;
            if (!Installed)
            {
                if (Config.DelayLoad && !Level.isLoaded)
                {
                    Level.onLevelLoaded += OnlevelLoaded;
                }
                else
                {
                    Installed = true;
                    R.Permissions = new PermissionsModifier(R.Permissions);
                }
            }
        }

        private void OnlevelLoaded(int Level)
        {
            if (Config.DelayLoad && !Installed)
            {
                Installed = true;
                R.Permissions = new PermissionsModifier(R.Permissions);
            }
        }

        public void RemovePermissionModifiers(ulong Target, string Permission)
        {
            Config.Players.RemoveAll(x => x.Player == Target && string.Equals(Permission, x.Permission, StringComparison.InvariantCultureIgnoreCase));
            Configuration.Save();
        }

        public void AddPermissionModifier(ulong target, string Permission, bool Allow)
        {
            Config.Players.Add(new PlayerModifier() { Player = target, Permission = Permission, Allow = Allow });
            Configuration.Save();
        }

        public override void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            Level.onLevelLoaded -= OnlevelLoaded;
            base.UnloadPlugin(state);
        }

        public bool CheckPlayerExemptions(ulong player, out bool Restrictors, out bool Permitters)
        {
            foreach (PlayerExemption exemption in Config.Exemptions.Where(x => x.Player == player))
            {
                Restrictors = exemption.AllowRestrictors;
                Permitters = exemption.AllowPermitters;
                return true;
            }

            Restrictors = true;
            Permitters = true;
            return false;
        }

        public bool CheckPlayerPermissionsOverrides(ulong player, string Permission, out bool Allowed)
        {
            if (UnderlyingProvider != null)
            {
                UnturnedPlayer pl = UnturnedPlayer.FromCSteamID(new Steamworks.CSteamID(player));
                if (pl == null)
                {
                    Allowed = false;
                    return false;
                }
                Dictionary<string, KeyValuePair<int, bool>> Modifiers = new Dictionary<string, KeyValuePair<int, bool>>(StringComparer.InvariantCultureIgnoreCase);
                foreach (var permission in UnderlyingProvider.GetPermissions(pl).Where(x => x.Name.StartsWith("SherbetPermissionsModifier.Mod.", StringComparison.InvariantCultureIgnoreCase)))
                {
                    string perm = permission.Name.Remove(0, "SherbetPermissionsModifier.Mod.".Length);
                    List<string> prts = perm.Split('.').ToList();
                    if (prts.Count >= 3)
                    {
                        Console.WriteLine("has cn");
                        string Mode = prts[0];
                        string weightstr = prts[1];
                        prts.RemoveAt(0);
                        prts.RemoveAt(0);
                        string permval = string.Join(".", prts);
                        if (!string.Equals(permval, Permission, StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        bool all = false;
                        bool valid = false;
                        if (string.Equals(Mode, "Deny", StringComparison.InvariantCultureIgnoreCase))
                        {
                            valid = true;
                            all = false;
                        }
                        else if (string.Equals(Mode, "Grant", StringComparison.InvariantCultureIgnoreCase))
                        {
                            valid = true;
                            all = true;
                        }
                        if (int.TryParse(weightstr, out int weight) && valid)
                        {
                            if (Modifiers.ContainsKey(permval))
                            {
                                if (Modifiers[permval].Key < weight)
                                {
                                    Modifiers[permval] = new KeyValuePair<int, bool>(weight, all);
                                }
                            }
                            else
                            {
                                Modifiers.Add(permval, new KeyValuePair<int, bool>(weight, all));
                            }
                        }
                    }
                }

                if (Modifiers.ContainsKey(Permission))
                {
                    Allowed = Modifiers[Permission].Value;
                    return true;
                }
                else
                {
                    Allowed = false;
                    return false;
                }
            }
            else
            {
                Allowed = false;
                return false;
            }
        }

        public bool CheckOverrides(ulong player, string Permission, out bool Allowed)
        {
            foreach (PlayerModifier modifier in Config.Players.Where(x => x.Player == player && (string.Equals(Permission, x.Permission, StringComparison.InvariantCultureIgnoreCase) || x.Permission == "*")))
            {
                Allowed = modifier.Allow;
                return true;
            }
            if (Config.UsePermissionConfigModifiers)
            {
                if (CheckPlayerPermissionsOverrides(player, Permission, out bool All))
                {

                    Allowed = All;
                    return true;
                }
                else
                {
                    Allowed = false;
                    return false;
                }
            }
            Allowed = false;
            return false;
        }

        public bool CheckOverrides(ulong player, List<string> Permissions, out bool Allowed)
        {
            Allowed = true;
            bool HasValue = false;
            foreach (string perm in Permissions)
            {
                if (CheckOverrides(player, perm, out bool NodeAllowed))
                {
                    HasValue = true;
                    if (!NodeAllowed)
                    {
                        Allowed = false;
                        return true;
                    }
                }
            }
            return HasValue;
        }
    }
}