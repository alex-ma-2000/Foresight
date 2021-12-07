using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
    public Camera camera;
    public GameObject defenderWalls;

    public Wall[] walls;
    private GameManager gameManager;
    private Attacker attacker;
    [SerializeField]
    private int wallResources = 2;

    public AK.Wwise.Event defenderHit;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        attacker = GameObject.Find("ball").GetComponent<Attacker>();

        // Getting all the walls
        Wall[] objs = defenderWalls.GetComponentsInChildren<Wall>();
        walls = new Wall[objs.Length];

        for (int i = 0; i < objs.Length; i++)
        {
            walls[i] = (objs[i].gameObject.GetComponent<Wall>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Defender Start Logic
        if (!gameManager.gameOver && gameManager.gameState == GameManager.GameState.Defender)
        {
            // Highlighting Blocks Hovered
            Vector3 mousePosToWorldPos = GetCurrentMousePosition();
            foreach (Wall wall in walls)
            {
                wall.Highlight(mousePosToWorldPos);    
            }
            // Selecting Blocks
            if (Input.GetMouseButtonDown(0))
            {
                foreach (Wall wall in walls)
                {
                    wall.Select(mousePosToWorldPos);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tick Game Score
        gameManager.AttackerWin();
        attacker.ResetBouncesOccured();
        defenderHit.Post(gameObject);
    }

    private Vector3 GetCurrentMousePosition()
    {
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }

    public int GetWalls()
    {
        return wallResources;
    }

    public void PlaceWall()
    {
        wallResources--;
        gameManager.UpdateDefenderResources(wallResources);
    }

    public void DeselectWall()
    {
        wallResources++;
        gameManager.UpdateDefenderResources(wallResources);
    }

    public void DeselectAllWalls()
    {
        foreach (Wall wall in walls)
        {
            wall.Deselect();
        }
    }

    public void HideUnselectedWalls()
    {
        foreach (Wall wall in walls)
        {
            if (!wall.IsSelected())
            {
                wall.gameObject.SetActive(false);
            }
        }
    }

    public void SetWallResources(int walls)
    {
        wallResources = walls;
    }
}
