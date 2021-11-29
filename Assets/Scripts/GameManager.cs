using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int attackerScore = 0;
    [SerializeField]
    private int attackerWinScore = 5;
    [SerializeField]
    private int defenderScore = 0; // Technically this is attacker lives in this build
    [SerializeField]
    private int defenderWinScore = 5;

    public GameObject attackerSpawn;
    public GameObject attacker;

    public GameObject wall;
    private GameObject[] walls;

    public bool gameOver = false;
    [SerializeField]
    private bool gameStarted = false;

    private Text scoreboard;
    private Text attackerResources;
    private Text winScreen;
    private GameObject playerSelect;

    public AK.Wwise.Event placeWalls;

    public enum GameState { Attacker, Defender, NotStarted };
    public GameState gameState = GameState.NotStarted;

    // Start is called before the first frame update
    void Start()
    {
        scoreboard = GameObject.Find("Scoreboard").GetComponent<Text>();
        scoreboard.gameObject.SetActive(false);
        attackerResources = GameObject.Find("Attacker").GetComponent<Text>();
        attackerResources.gameObject.SetActive(false);
        winScreen = GameObject.Find("WinScreen").GetComponent<Text>();
        winScreen.gameObject.SetActive(false);

        // Add Listeners to buttons
        AddListernersToButtons();

        Transform[] objs = wall.GetComponentsInChildren<Transform>();
        walls = new GameObject[objs.Length];

        for (int i = 0; i < objs.Length; i++)
        {
            walls[i] = (objs[i].gameObject);
        }

        // First Round Logic - Need to place down 2 wall in the defender area
        //BuildWalls();
        wall.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // In Game Logic
        if (!gameOver && gameStarted)
        {
            // Logic for ending the game
            if (attackerScore >= attackerWinScore)
            {
                gameOver = true;
                winScreen.gameObject.SetActive(true);
                winScreen.text = "Attacker Wins! \nClick to Play Again!\nESC to Return to Title";
                AkSoundEngine.SetState("Music_States", "Game_Completed");
            }
            else if (defenderScore >= defenderWinScore)
            {
                gameOver = true;
                winScreen.gameObject.SetActive(true);
                winScreen.text = "You Lose. \nClick to Play Again! \nESC to Return to Title"; // Change to defender wins in future interations
                AkSoundEngine.SetState("Music_States", "Game_Completed");
            }
        }
        // Post Game Logic
        else if (gameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                attackerScore = 0;
                defenderScore = 0;
                UpdateScoreboard();
                winScreen.gameObject.SetActive(false);
                ResetRound();
                UpdateAttackerResources(attacker.GetComponent<Attacker>().GetBounces());
                gameOver = false;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Change to return to main menu
                SceneManager.LoadScene("Title Screen");
            }
        }
    }

    // Reset positions, randomizes new defender walls
    void ResetRound()
    {
        // Reset
        attacker.transform.position = attackerSpawn.transform.position;
        attacker.GetComponent<Rigidbody2D>().velocity = new Vector3(0f, 0f, 0f);
        attacker.GetComponent<Attacker>().fired = false;

        // New Round Logic
        //BuildWalls();
    }

    void UpdateScoreboard()
    {
        scoreboard.text = "Atk " + attackerScore + " | Def " + defenderScore; 
    }

    public void UpdateAttackerResources(int bounces)
    {
        attackerResources.text = "Bounces: " + bounces;
    }

    public void AttackerWin()
    {
        attackerScore++;
        UpdateScoreboard();
        UpdateAttackerResources(attacker.GetComponent<Attacker>().GetBounces());
        ResetRound();
    }

    public void DefenderWin()
    {
        defenderScore++;
        UpdateScoreboard();
        UpdateAttackerResources(attacker.GetComponent<Attacker>().GetBounces());
        ResetRound();
    }

    // Generates a list of random walls as index values
    private ArrayList RandomWalls()
    {
        int numWalls = 2 + attackerScore;
        ArrayList randNums = new ArrayList();

        for (int i = 0; i < numWalls; i++)
        {
            int rand = UnityEngine.Random.Range(1, walls.Length);
            if (!randNums.Contains(rand))
            {
                randNums.Add(rand);
            }
        }

        return randNums;
    }

    public void BuildWalls()
    {
        // Deactivates all active walls
        foreach (GameObject go in walls)
        {
            go.SetActive(false);
        }
        wall.SetActive(true);

        // Activates selected walls
        ArrayList wallNumbers = RandomWalls();
        foreach (int x in wallNumbers)
        {
            walls[x].SetActive(true);

            Debug.Log(x);
        }

        placeWalls.Post(gameObject);
    }

    public int AttackerBounces()
    {
        return 3 + defenderScore;
    }

    // Adds Listeners to buttons for this scene
    private void AddListernersToButtons()
    {
        playerSelect = GameObject.Find("Player Select");
        Button attackerBtn = playerSelect.GetComponentsInChildren<Button>()[0];
        Button defenderBtn = playerSelect.GetComponentsInChildren<Button>()[1];

        attackerBtn.onClick.AddListener(AttackerStart);
        defenderBtn.onClick.AddListener(DefenderStart);
    }

    // Starts the game with the player as an attacker
    private void AttackerStart()
    {
        gameState = GameState.Attacker;
        playerSelect.SetActive(false);
        scoreboard.gameObject.SetActive(true);
        attackerResources.gameObject.SetActive(true);
        AkSoundEngine.PostEvent("Menu_Buttons", gameObject);
    }

    // Starts the game with the player as a defender
    private void DefenderStart()
    {
        gameState = GameState.Defender;
        playerSelect.SetActive(false);
        scoreboard.gameObject.SetActive(true);
        attackerResources.gameObject.SetActive(true);
        AkSoundEngine.PostEvent("Menu_Buttons", gameObject);
    }
}
