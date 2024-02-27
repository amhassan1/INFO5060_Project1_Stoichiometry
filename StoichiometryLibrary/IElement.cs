namespace StoichiometryLibrary
{
    /// <summary>
    /// Element interface
    /// </summary>
    public interface IElement
    {
        public string Symbol { get; }
        public string Name { get; }
        public ushort AtomicNumber { get; }
        public double AtomicMass { get; }
        public ushort Period { get; }
        public ushort Group { get; }
    }
}
