using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Rocket.API;

namespace SherbetPermissionsModifier.Models
{
    public class ModifierConfig : IRocketPluginConfiguration
    {
        public bool DelayLoad;
        public bool UsePermissionConfigModifiers = true;
        [XmlArrayItem("PlayerModifier")]
        public List<PlayerModifier> Players;
        [XmlArrayItem("PlayerExemption")]
        public List<PlayerExemption> Exemptions;
        public void LoadDefaults()
        {
            DelayLoad = true;
            UsePermissionConfigModifiers = true;
            Players = new List<PlayerModifier>();
            Players.Add(new PlayerModifier() { Allow = true, Permission = "AllowedPermission", Player = 1234 });
            Players.Add(new PlayerModifier() { Allow = false, Permission = "DeniedPermission", Player = 1234 });
            Exemptions = new List<PlayerExemption>();
            Exemptions.Add(new PlayerExemption() { Player = 1446, AllowPermitters = true, AllowRestrictors = false });
        }
    }
}
