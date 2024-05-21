using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathToMainMenu : MonoBehaviour
{
    [SerializeField] private string mainMenu;
    [SerializeField] private float wait = 2f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("goToMainMenu", wait);
    }

    private void goToMainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }
}
