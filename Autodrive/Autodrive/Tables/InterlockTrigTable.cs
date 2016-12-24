using Autodrive.Options;

namespace Autodrive.Tables
{
    public class InterlockTrigTable : NavigationTable<InterlockTrigOptions>
    {
        public InterlockTrigTable()
        {
            table = new InterlockTrigOptions[7][];
            table[0] = new[] {InterlockTrigOptions.OVERRIDE_INTRLKS};
            table[1] = new[] {InterlockTrigOptions.GENERAL_INTERL_CLEAR};
            table[2] = new[] {InterlockTrigOptions.DOSE_INTERL_CLEAR};
            table[3] = new[] {InterlockTrigOptions.TRIGGERS};
            table[4] = new[] {InterlockTrigOptions.KLYSTRON_DELAY};
            table[5] = new[] {InterlockTrigOptions.GUN_DELAY};
            table[6] = new[] {InterlockTrigOptions.LIGHTS};
            Current = InterlockTrigOptions.OVERRIDE_INTRLKS;
        }
    }
}