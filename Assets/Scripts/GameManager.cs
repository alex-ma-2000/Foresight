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
    private int defenderScore = 0;
    [SerializeField]
    private int defenderWinScore = 5;

    public GameObject attackerSpawn;
    public GameObject attacker;
    public GameObject defender;

    public GameObject wall;
    private GameObject[] walls;

    public bool gameOver = false;
    public bool playingRound = false;
    [SerializeField]
    private bool gameStarted = false;

    [HideInInspector]
    public Text scoreboard;
    [HideInInspector]
    public Text attackerResources;
    [HideInInspector]
    public Text defenderResources;
    private Text winScreen;
    [SerializeField]
    private Button nextTurn;
    private GameObject playerSelect;

    public AK.Wwise.Event placeWalls;

    public enum GameState { Attacker, Defender, NotStarted };
    public GameState gameState = GameState.NotStarted;

    public enum GameMode { Singleplayer, Multiplayer };
    public GameMode gameMode;

    // Start is called before the first frame update
    void Start()
    {
        scoreboard = GameObject.Find("Scoreboard").GetComponent<Text>();
        attackerResources = GameObject.Find("Attacker").GetComponent<Text>();
        defenderResources = GameObject.Find("DefenderResources").GetComponent<Text>();
        winScreen = GameObject.Find("WinScreen").GetComponent<Text>();
        winScreen.gameObject.SetActive(false);
        if (gameMode == GameMode.Singleplayer)
        {
            playerSelect = GameObject.Find("Player Select");
        }
        if (gameMode == GameMode.Multiplayer)
        {
            gameStarted = true;
        }

        nextTurn = GameObject.Find("Next Turn").GetComponent<Button>();
        nextTurn.onClick.AddListener(NextTurn);

        Wall[] objs = wall.GetComponentsInChildren<Wall>();
        walls = new GameObject[objs.Length];

        for (int i = 0; i < objs.Length; i++)
        {
            walls[i] = (objs[i].gameObject);
        }

        // First Round Logic - Need to place down 2 wall in the defender area
        //BuildWalls();
        //wall.SetActive(false);
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
                AkSoundEngine.PostEvent("Game_Over", gameObject);
            }
            else if (defenderScore >= defenderWinScore)
            {
                gameOver = true;
                winScreen.gameObject.SetActive(true);
                winScreen.text = "Defender Wins! \nClick to Play Again! \nESC to Return to Title";
                AkSoundEngine.SetState("Music_States", "Game_Completed");
                AkSoundEngine.PostEvent("Game_Over", gameObject);
            }

            // Logic for determining turns
            if (gameMode == GameMode.Multiplayer)
            {
                if ((gameState == GameState.Attacker && attacker.GetComponent<Attacker>().lockedIn) ||
                    (gameState == GameState.Defender && defender.GetComponent<Defender>().GetWalls() == 0))
                {
                    nextTurn.interactable = true;
                }
                else
                {
                    nextTurn.interactable = false;
                }
            } 
            else if (gameMode == GameMode.Singleplayer && gameState == GameState.Defender && defender.GetComponent<Defender>().GetWalls() == 0)
            {
                nextTurn.interactable = true;
            }
            else
            {
                nextTurn.interactable = false;
            }
        }
        // Post Game Logic
        else if (gameOver)
        {
            if (Input.GetMouseButtonDown(0) && gameMode == GameMode.Singleplayer)
            {
                attackerScore = 0;
                defenderScore = 0;
                UpdateScoreboard();
                winScreen.gameObject.SetActive(false);
                ResetRound();
                gameOver = false;
                gameState = GameState.NotStarted;
                gameStarted = false;
                playerSelect.SetActive(true);
                UpdateAttackerResources(3);
                UpdateDefenderResources(2);
                AkSoundEngine.StopAll(gameObject);
                AkSoundEngine.PostEvent("Play", gameObject);
            } 
            else if (Input.GetMouseButtonDown(0) && gameMode == GameMode.Multiplayer) 
            {
                attackerScore = 0;
                defenderScore = 0;
                UpdateScoreboard();
                winScreen.gameObject.SetActive(false);
                ResetRound();
                gameOver = false;
                gameState = GameState.Attacker;
                UpdateAttackerResources(3);
                UpdateDefenderResources(2);
                attacker.GetComponent<Attacker>().fired = false;
                attacker.GetComponent<Attacker>().lockedIn = false;
                defender.GetComponent<Defender>().DeselectAllWalls();
                AkSoundEngine.StopAll(gameObject);
                AkSoundEngine.PostEvent("Play", gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Change to return to main menu
                SceneManager.LoadScene("Title Screen");
                AkSoundEngine.StopAll(gameObject);
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
        attacker.GetComponent<Attacker>().lockedIn = false;
        defender.GetComponent<Defender>().DeselectAllWalls();

        playingRound = false;
        foreach (GameObject wall in walls)
        {
            wall.SetActive(true);
            wall.GetComponent<Wall>().Deselect();
        }
        defender.GetComponent<Defender>().SetWallResources(DefenderWalls());

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

    public void UpdateDefenderResources(int walls)
    {
        defenderResources.text = "Walls: " + walls;
    }

    public void AttackerWin()
    {
        attackerScore++;
        UpdateScoreboard();
        UpdateAttackerResources(AttackerBounces());
        UpdateDefenderResources(DefenderWalls());
        ResetRound();
    }

    public void DefenderWin()
    {
        defenderScore++;
        UpdateScoreboard();
        UpdateAttackerResources(AttackerBounces());
        UpdateDefenderResources(DefenderWalls());
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

            //Debug.Log(x);
        }

        placeWalls.Post(gameObject);
    }

    public int AttackerBounces()
    {
        return 3 + defenderScore;
    }

    public int DefenderWalls()
    {
        return 2 + attackerScore;
    }

    public void setGameStarted(bool started)
    {
        gameStarted = started;
    }

    private void NextTurn()
    {
        if (gameMode == GameMode.Singleplayer && gameState == GameState.Defender)
        {
            playingRound = true;
            defender.GetComponent<Defender>().HideUnselectedWalls();
        }
        else if (gameMode == GameMode.Multiplayer && gameState == GameState.Attacker && attacker.GetComponent<Attacker>().lockedIn)
        {
            gameState = GameState.Defender;
        }
        else if (gameMode == GameMode.Multiplayer && gameState == GameState.Defender)
        {
            gameState = GameState.Attacker;
            playingRound = true;
            defender.GetComponent<Defender>().HideUnselectedWalls();
            attacker.GetComponent<Attacker>().FireBall();
        }
    }
}
