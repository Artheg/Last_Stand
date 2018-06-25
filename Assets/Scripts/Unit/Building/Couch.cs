public class Couch : Unit
{
    protected override void Die()
    {
        MainGameManager.Instance.OnCouchDestroyed();
    }
}
