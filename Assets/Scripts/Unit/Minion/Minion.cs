public class Minion : Unit
{
    public MinionAI GetMinionAI()
    {
        var minionAI = GetComponent<MinionAI>();
        if (minionAI == null)
            minionAI = gameObject.AddComponent<MinionAI>();
        return minionAI;
    }
}

