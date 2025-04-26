using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Feature
    {
        public int Id { get; set; }
        public string Name { get; set; } // "GPS", "Sunroof", "Child Seat"
        public string Icon { get; set; }

        public ICollection<VehicleFeatures> VehicleFeatures { get; set; }
    }
}
