using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Unturned.Player;

namespace SherbetPermissionsModifier.Models
{
    public class PermissionsModifier : IRocketPermissionsProvider
    {
        public IRocketPermissionsProvider Underlying;

        public PermissionsModifier(IRocketPermissionsProvider Underlying)
        {
            this.Underlying = Underlying;
            SherbetPermissionsModifier.UnderlyingProvider = Underlying;
        }

        public RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group)
        {
            return Underlying.AddGroup(group);
        }

        public RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player)
        {
            return Underlying.AddPlayerToGroup(groupId, player);
        }

        public RocketPermissionsProviderResult DeleteGroup(string groupId)
        {
            return Underlying.DeleteGroup(groupId);
        }

        public RocketPermissionsGroup GetGroup(string groupId)
        {
            return Underlying.GetGroup(groupId);
        }

        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups)
        {
            return Underlying.GetGroups(player, includeParentGroups);
        }

        public List<Permission> GetPermissions(IRocketPlayer player)
        {
            return Underlying.GetPermissions(player);
        }

        public List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions)
        {
            return Underlying.GetPermissions(player, requestedPermissions);
        }

        public bool HasPermission(IRocketPlayer player, List<string> requestedPermissions)
        {
            if (SherbetPermissionsModifier.Instance != null && SherbetPermissionsModifier.Instance.State == PluginState.Loaded)
            {
                if (player is UnturnedPlayer UPlayer)
                {
                    if (SherbetPermissionsModifier.Instance.CheckOverrides(UPlayer.CSteamID.m_SteamID, requestedPermissions, out bool ret))
                    {
                        return ret;
                    }
                }
            }
            return Underlying.HasPermission(player, requestedPermissions);
        }

        public void Reload()
        {
            Underlying.Reload();
        }

        public RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player)
        {
            return Underlying.RemovePlayerFromGroup(groupId, player);
        }

        public RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group)
        {
            return Underlying.SaveGroup(group);
        }
    }
}
