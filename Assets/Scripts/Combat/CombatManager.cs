using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CombatOrder
{
    player1,
    player2,
    enemy1,
    enemy2,
    enemy3,
}

public class CombatManager : MonoBehaviour
{
    #region Variables

    public static CombatManager Instance;

    [Header("Start")]
    [SerializeField] private GameObject m_overworldEnemy;
    [SerializeField] private GameObject[] m_combatEnemyPrefabs;

    private Scene m_overworldScene;
    private Scene m_combatScene;

    [Header("During")]
    public bool InCombat;

    [SerializeField] private CombatOrder m_order;

    private CombatPlayer m_combatPlayer;

    // Ending
    private OverworldPlayer m_overworldPlayer;

    #endregion

    #region Setup

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        m_overworldScene = SceneManager.GetSceneByBuildIndex(0);
        m_combatScene = SceneManager.GetSceneByBuildIndex(1);

        // InCombat = false;

        // SceneManager.SetActiveScene(m_overworldScene);

        m_overworldPlayer = FindObjectOfType<OverworldPlayer>();

        StartCombat();
    }

    #endregion

    #region Combat

    public void StartCombat()
    {
        // When combat starts the combat scene gets loaded and set as active 
        print("Combat started!");
        //SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        //m_combatScene = SceneManager.GetSceneByBuildIndex(1);
        InCombat = true;
        //SceneManager.SetActiveScene(m_combatScene);
        m_combatPlayer = FindObjectOfType<CombatPlayer>();
        HandleOrder();
    }

    public void EndCombat()
    {
        // When combat ends the overworld scene gets set as active again to continue playing
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync(1);
        InCombat = false;
        SceneManager.SetActiveScene(m_overworldScene);
        SceneManager.UnloadSceneAsync(1);
        m_overworldPlayer.EndCombat();
    }

    #region Order

    private void HandlePlayer(int player)
    {
        m_combatPlayer.UsePlayer(player);
    }

    private void HandleEnemy(int enemy)
    {

    }

    public void NextOrder()
    {
        m_order = (CombatOrder)(int)m_order + 1;
        if ((int)m_order >= 5)
        {
            m_order = CombatOrder.player1;
        }
        HandleOrder();
    }

    private void HandleOrder()
    {
        switch (m_order)
        {
            case CombatOrder.player1:
                HandlePlayer(1);
                break;
            case CombatOrder.player2:
                HandlePlayer(2);
                break;
            case CombatOrder.enemy1:
                HandleEnemy(1);
                break;
            case CombatOrder.enemy2:
                HandleEnemy(2);
                break;
            case CombatOrder.enemy3:
                HandleEnemy(3);
                break;
        }
    }

    #endregion

    #endregion
}
