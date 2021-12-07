using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Multiplayer : MonoBehaviour
{
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(EnterGame);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void EnterGame()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    public void OnClick()
    {
        AkSoundEngine.PostEvent("Menu_Buttons", gameObject);
    }
}
