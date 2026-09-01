using PeriodicTableSystem.World;

namespace PeriodicTableSystem.Chemistry
{
    public class Bond
    {
        public PeriodicElementInstance AtomA;
        public PeriodicElementInstance AtomB;
        public BondType BondType;
        public int BondOrder;
        public BondVisual Visual;

        public Bond(PeriodicElementInstance atomA, PeriodicElementInstance atomB, BondType bondType, int bondOrder = 1)
        {
            AtomA = atomA;
            AtomB = atomB;
            BondType = bondType;
            BondOrder = bondOrder;
            Visual = null;
        }
    }
}
    
