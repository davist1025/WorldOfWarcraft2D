using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Database.Models.Realm.Items.Components
{
    [Table("item_container_component_descriptor")]
    public class ContainerComponentDescriptor
    {
        /// <summary>
        /// The unique item this descriptor will act on.
        /// </summary>
        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("slot_count")]
        public int SlotCount { get; set; }
    }
}
