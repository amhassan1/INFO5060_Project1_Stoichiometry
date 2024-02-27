using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoichiometryLibrary
{
    /// <summary>
    /// Element in molecule based on IElement with multiplier for number of occuernces
    /// </summary>
    public interface IMolecularElement : IElement
    {
        public ushort Multiplier  { get; }
    }
}
