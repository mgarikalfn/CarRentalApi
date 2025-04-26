using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class VehicleFeatures
    {
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public int FeatureId { get; set; }
        public Feature Feature { get; set; }
    }
}
