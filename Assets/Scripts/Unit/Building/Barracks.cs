using System;

public class Barracks : Unit
{
    private MinionSpawner minionSpawner;

    override protected void Init()
    {
        base.Init();
        minionSpawner = GetComponent<MinionSpawner>();
        if (minionSpawner == null)
            throw new Exception("You must add and setup MinionSpawner in order to Barracks to work");
    }

    public override bool IsDead
    {
        get
        {
            return false;
        }
    }

    protected override void Die()
    {

    }

    void Update()
    {
        minionSpawner.RequestUnitSpawn();
    }
}
