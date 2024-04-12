using AI;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private PlayerController player;
    [SerializeField] private EnemyCounterUI counter;
    
    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if(player.playerHealth <= 0)
            SceneManager.LoadSceneAsync("Lose Scene");

        if(counter.enemyCounter <= 0)
            SceneManager.LoadSceneAsync("Win Scene");
    }
}
