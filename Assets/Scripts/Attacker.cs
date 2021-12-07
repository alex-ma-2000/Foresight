using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    private float speed = 100f;
    private int lineLength = 50;

    private LineRenderer _lineRenderer;
    private Rigidbody2D rb;
    private GameManager gameManager;
    private Transform[] shootPos;

    public Camera camera;
    public bool fired = false;
    public bool lockedIn = false;
    public int bounces = 3; // May increase in the future
    private int curBounces;
    private int bouncesOccured;
    private Vector3 lockedFirePos;

    public AK.Wwise.Event ballBounce;
    public AK.Wwise.Event fireBall;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = 1f;
        _lineRenderer.endWidth = 1f;
        _lineRenderer.startColor = Color.white;
        _lineRenderer.endColor = Color.white;
        _lineRenderer.enabled = true;
        rb = gameObject.GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager.gameMode == GameManager.GameMode.Singleplayer)
        {
            AttackerPos[] atkPos = GameObject.Find("ShootPositions").GetComponentsInChildren<AttackerPos>();

            shootPos = new Transform[atkPos.Length];
            for (int i = 0; i < atkPos.Length; i++)
            {
                shootPos[i] = atkPos[i].gameObject.GetComponent<Transform>();
            }
        }

        curBounces = bounces;
    }

    // Update is called once per frame
    void Update()
    {
        // Attacker Start Logic
        if (!fired && !gameManager.gameOver && gameManager.gameState == GameManager.GameState.Attacker && !lockedIn)
        {
            // Draws a line
            _lineRenderer.SetPosition(0, this.transform.position);
            _lineRenderer.positionCount = 2;

            Vector3 dir = GetCurrentMouseDirection();
            Vector3 finalDir = dir + dir.normalized;
            Vector3 lineEnd = finalDir.normalized * lineLength + transform.position;
            lineEnd.z = 2f;

            _lineRenderer.SetPosition(1, lineEnd);
            _lineRenderer.enabled = true;

            // Fires the ball
            if (Input.GetMouseButtonDown(0) && gameManager.gameMode == GameManager.GameMode.Singleplayer)
            {
                // Shooting Logic
                //Debug.Log("FIRED");
                gameManager.BuildWalls();
                fireBall.Post(gameObject);

                _lineRenderer.enabled = false;
                Vector3 cursorInWorldPos = GetCurrentMouseDirection();
                cursorInWorldPos.Normalize();
                //Debug.Log(cursorInWorldPos);
                rb.velocity = new Vector2(cursorInWorldPos.x * speed, cursorInWorldPos.y * speed);

                fired = true;
            }
            else if (Input.GetMouseButtonDown(0) && gameManager.gameMode == GameManager.GameMode.Multiplayer)
            {
                _lineRenderer.enabled = false;
                Vector3 cursorInWorldPos = GetCurrentMouseDirection();
                cursorInWorldPos.Normalize();
                lockedFirePos = cursorInWorldPos;
                lockedIn = true;
            }
        } 
        // Defender Start Logic
        else if (gameManager.gameState == GameManager.GameState.Defender && gameManager.playingRound && !fired)
        {
            // Attacker AI
            int rand = Random.Range(1, shootPos.Length);
            Vector3 shootDir = shootPos[rand].position - this.transform.position;
            shootDir.Normalize();
            rb.velocity = new Vector2(shootDir.x * speed, shootDir.y * speed);
            fired = true;
        }
    }

    // Event Trigger for Bounces
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" && curBounces != 0)
        {
            rb.velocity = Vector3.Reflect(rb.velocity, collision.contacts[0].normal);
            // Ball Bounce Sound
            ballBounce.Post(gameObject);
            curBounces--;
            bouncesOccured++;
            Debug.Log(bouncesOccured);
            AkSoundEngine.SetRTPCValue("Bounces_Occured", bouncesOccured);
            gameManager.UpdateAttackerResources(curBounces);
        }
        else if (curBounces == 0)
        {
            gameManager.DefenderWin();
            bounces = gameManager.AttackerBounces();
            curBounces = bounces; // Resets Bounces
            ResetBouncesOccured();
            gameManager.UpdateAttackerResources(curBounces);
            AkSoundEngine.PostEvent("Defender_Hit", gameObject);
        }
    }

    private Vector3 GetCurrentMouseDirection()
    {
        Vector3 shootDirection;
        shootDirection = Input.mousePosition;
        shootDirection.z = 0.0f;
        shootDirection = camera.ScreenToWorldPoint(shootDirection);
        shootDirection = shootDirection - transform.position;

        return shootDirection;
    }

    public int GetBounces()
    {
        return bounces;
    }

    public void ResetBouncesOccured()
    {
        bouncesOccured = 0;
    }

    public void FireBall()
    {
        rb.velocity = new Vector2(lockedFirePos.x * speed, lockedFirePos.y * speed);
        fired = true;
    }
}
