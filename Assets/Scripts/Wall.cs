using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    Outline ol;
    SpriteRenderer sr;
    BoxCollider2D m_Collider;

    [SerializeField]
    private Defender defender;
    private bool selected = false;
    private Color baseColor;

    // Start is called before the first frame update
    void Start()
    {
        ol = gameObject.GetComponent<Outline>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        m_Collider = gameObject.GetComponent<BoxCollider2D>();
        defender = GameObject.Find("DefenderStart").GetComponent<Defender>();

        baseColor = sr.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Highlight(Vector3 mousePos)
    {
        if (!selected)
        {
            if (m_Collider.bounds.Contains((Vector2)mousePos))
            {
                ol.enabled = true;
                sr.color = Color.yellow;
            }
            else
            {
                ol.enabled = false;
                sr.color = baseColor;
            }
        } 
    }

    public void Select(Vector3 mousePos)
    {
        if (m_Collider.bounds.Contains((Vector2)mousePos) && !selected && defender.GetWalls() > 0)
        {
            selected = true;
            sr.color = Color.red;
            defender.PlaceWall();
            AkSoundEngine.PostEvent("Place_Wall", gameObject);
        } 
        else if (m_Collider.bounds.Contains((Vector2)mousePos) && selected)
        {
            Deselect();
            sr.color = baseColor;
            defender.DeselectWall();
        }
    }

    public bool IsSelected()
    {
        return selected;
    }

    public void Deselect()
    {
        selected = false;
        sr.color = baseColor;
    }
}
