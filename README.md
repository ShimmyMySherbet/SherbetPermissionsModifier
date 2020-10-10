# SherbetPermissionsModifier
A permissions modifier for Unturned. Allows you to Deny/Grant permissions to Players/Groups, even deny admins certain commands.

# Usage:

## /ModPlayerPerms <Grant/Deny/Reset/List> <player> <Permission>

#### To grant a permission to a player:

 /ModPlayerPerms Grant [Player] [Permission]
 
 This has the effect as if the player had the permission in Permissions.config.xml, but for a single player, and without a new rocket group.
 
 
 #### To deny a permission to a player:
 
 /ModPlayerPerms Deny [Player] [Permission]
 
 This denies the permission/command to the target player. This overrides permissions set in permissions.config.xml, and also overrides admin perms.
 
 #### To reset/undo a modification
 
 /ModPlayerPerms Reset [Player] [Permission]
 
 Removes any Grants/Denies of a permission to the target player.
 
 #### View a player's permission modifications
 
 /ModPlayerPerms View [Player]
 
 Lists all player permission modifications (set with /modplayerperms, not set in the config)
 
 ## Config permission modifications
 
 This is mainly useful if you want to grant a permission to everyone except a couple of rocket groups, or if you want to deny a permission/command to an admin without disabling the command entirely.
 
 Permission Format: **SherbetPermissionsModifier.Mod.[Grant/Deny].[Weight (number)].[Permission]**
 
 The node for a permission with the highest weight value will be used.
 
 e.g.,
 
Default Permissions:
```
Jump
```

Restricted's Permissions
```
SherbetPermissionsModifier.Mod.Deny.10.Jump
```

OnDutyModerator's Permissions:

```
SherbetPermissionsModifier.Mod.Grant.20.Jump
```
 
Here, everyone would be able to use /Jump, except for people with the Restricted group. However, if an OnDutyModerator also has the Restricted role, they will still be able to use /Jump, since the grant weight of OnDutyModerator is higher than the Deny weight of Restricted.

Or, another example (using a duty plugin, which allows a player to toggle admin perms on themself)

Administrator's Permissions
```
Duty
Spy
Investigate
SherbetPermissionsModifier.Mod.Deny.90.ModPlayerPerms
```

HeadAdministrator's 
```
SherbetPermissionsModifier.Mod.Grant.100.ModPlayerPerms
```

Here, Admins, even when on duty (they have server admin), will not be able to use /ModPlayerPerms.

The Administrator group here also grants off duty Admins /Spy and /Investigate.

The Head Admin would have both Administrator and HeadAdministrator rocket groups. However, since the grant weight of HeadAdministrator is higher than the deny permissions of Administrator, the Head Admin would still be able to use /ModPlayerPerms, and regular Admins would not.
 
 # Downloads
 Download via [Releases](https://github.com/ShimmyMySherbet/SherbetPermissionsModifier/releases)
 

