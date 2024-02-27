using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StoichiometryLibrary
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Element : IMolecularElement
    {
        public ushort Multiplier { get; set; }

        [JsonProperty]
        public required string Symbol { get; set; }
        [JsonProperty]
        public required string Name { get; set; }

        [JsonProperty("number")]
        public ushort AtomicNumber { get; set; }

        [JsonProperty("atomic_mass")]
        public double AtomicMass { get; set; }
        [JsonProperty]
        public ushort Period { get; set; }
        [JsonProperty]
        public ushort Group { get; set; }
    }
}
