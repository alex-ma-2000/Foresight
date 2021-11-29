using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    GameManager gameManager;

    public AK.Wwise.Event bgm;
    public AK.Wwise.Event victoryJingle;
    public AK.Wwise.State playingState;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        bgm.Post(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        uint currentState = 0;

        if (gameManager.gameOver)
        {
            bgm.Stop(gameObject);
            victoryJingle.Post(gameObject);
        }
        AkSoundEngine.GetState(playingState.GroupId, out currentState);
        if (currentState == playingState.Id)
        {
            bgm.Post(gameObject);
        }
    }
}
