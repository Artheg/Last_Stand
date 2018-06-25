using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour {

	static public MainGameManager Instance { get; private set; }

    public float TimeBeforeNextRound = 10f;

    public Material PlayerTeamMaterial;
    public Material EnemyTeamMaterial;

    public Text TimerTF;

	public Fountain Fountain;
	public Couch Couch;

	public Action OnCountDownFinish;
    public Action OnWaveBeaten;

	public MinionManager MinionManager { get; private set; }

    private float timeLeft;
	
	void Awake()
	{
		InitInstance();
	}

    void Start () 
	{
        timeLeft = TimeBeforeNextRound;
		InitMinionManager();
	}

    private void InitInstance()
    {
		if (Instance != null)
			throw new Exception("There can be only one MainGameManager!");
		Instance = this;
    }

    private void InitMinionManager()
	{
        MinionManager = gameObject.AddComponent<MinionManager>();
		MinionManager.Init(Couch, Fountain);
        MinionManager.OnWaveBeaten += OnEnemyWaveBeaten;
        MinionManager.OnMinionSpawned += OnMinionSpawned;

        MinionManager.AddTeamMaterial(TeamID.player, PlayerTeamMaterial);
        MinionManager.AddTeamMaterial(TeamID.enemy, EnemyTeamMaterial);
	}

    private void OnMinionSpawned(Minion minion)
    {
        var minionAI = minion.GetMinionAI();
        minionAI.OnTargetRequest += MinionManager.SearchObjectForAttack;
    }

    public void OnCouchDestroyed()
    {
        Debug.Log("GAME OVER");
    }

    public void OnAllWavesBeaten()
    {
        Debug.Log("YOU WIN");
    }

	public void OnEnemyWaveBeaten()
	{
        OnWaveBeaten.Invoke();
	}

    public void BeginNewRound()
    {
        timeLeft = TimeBeforeNextRound;
        StartCoroutine(OnNewRound());
    }

    private IEnumerator OnNewRound()
	{
        yield return PerformCountDown();
		OnCountDownFinish.Invoke();
	}

	private IEnumerator PerformCountDown()
	{
		while (timeLeft > 0)
		{
			yield return new WaitForSeconds(1f);
            timeLeft--;		
			TimerTF.text = timeLeft.ToString();
		}
	}
}
