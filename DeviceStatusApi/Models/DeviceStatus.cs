using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceStatusApi.Models
{
    /// <summary>
    /// Representerar en enhet i systemet
    /// </summary>
    public class DeviceStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Namnet på enheten
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Adressen till Modbus-coilen (används för att styra enheten)
        /// </summary>
        public int CoilAddress { get; set; }

        /// <summary>
        /// Status för om enheten är på (true) eller av (false) on/off
        /// </summary>
        public bool IsOn { get; set; }
    }
}
