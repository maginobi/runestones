namespace Runestones
{
    public abstract class RuneEffect
    {
        public RuneQuality _Quality;
        public abstract void DoMagicAttack(Attack baseAttack);
    }
}
