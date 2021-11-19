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

    public Camera camera;
    public bool fired = false;
    public int bounces = 3; // May increase in the future
    private int curBounces;


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

        curBounces = bounces;
    }

    // Update is called once per frame
    void Update()
    {
        if (!fired && !gameManager.gameOver)
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
            if (Input.GetMouseButtonDown(0))
            {
                // Shooting Logic
                //Debug.Log("FIRED");
                gameManager.BuildWalls();
                
                _lineRenderer.enabled = false;
                Vector3 cursorInWorldPos = GetCurrentMouseDirection();
                cursorInWorldPos.Normalize();
                //Debug.Log(cursorInWorldPos);
                rb.velocity = new Vector2(cursorInWorldPos.x * speed, cursorInWorldPos.y * speed);

                fired = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" && curBounces != 0)
        {
            rb.velocity = Vector3.Reflect(rb.velocity, collision.contacts[0].normal);
            curBounces--;
            gameManager.UpdateAttackerResources(curBounces);
        }
        else if (curBounces == 0)
        {
            gameManager.DefenderWin();
            bounces = gameManager.AttackerBounces();
            curBounces = bounces; // Resets Bounces
            gameManager.UpdateAttackerResources(curBounces);
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
}
