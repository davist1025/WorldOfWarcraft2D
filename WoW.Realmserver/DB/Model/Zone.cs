using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.DB.Model
{
    /// <summary>
    /// Used to verify against maps being loaded.
    /// 
    /// If a map is loaded with an unknown or empty zone-id, it cannot be accessed in conventional ways.
    /// </summary>
    [Table("zones")]
    public class Zone
    {
        public int Id { get; set; }
        public string Guid { get; set; }
    }
}
