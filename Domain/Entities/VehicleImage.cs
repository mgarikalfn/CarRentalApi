using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class VehicleImage
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}
