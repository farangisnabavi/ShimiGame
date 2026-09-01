using System;

namespace PeriodicTableSystem.Chemistry
{
    public static class ChemistryEventBus
    {
        public static event Action<Bond> OnBondCreated;
        public static event Action<Bond> OnBondBroken;

        public static void BondCreated(Bond bond)
        {
            OnBondCreated?.Invoke(bond);
        }

        public static void BondBroken(Bond bond)
        {
            OnBondBroken?.Invoke(bond);
        }
    }
}