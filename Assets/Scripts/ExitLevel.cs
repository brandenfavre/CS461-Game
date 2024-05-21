using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : MonoBehaviour
{
    private bool playerInRange = false;
    public string nextLevel;
    private bool withPlayer;
    public NPCController npc;
    public PlayerController player;
    public DungeonGenerator dungeon;
    private Canvas prompt;


    void Start()
    {
        npc = FindObjectOfType<NPCController>();
        player = FindObjectOfType<PlayerController>();
        prompt = GetComponentInChildren<Canvas>();
        prompt.enabled = false;
        // Have to do this for the way I'm handling scenes for dungeons
        DungeonGenerator dungeon = FindObjectOfType<DungeonGenerator>();
        if(dungeon != null)
        {
            nextLevel = dungeon.nextScene;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && npc.isWithPlayer())
        {
            playerInRange = true;
            prompt.enabled = true;
        }
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            prompt.enabled = false;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Load the next scene
            SceneManager.LoadScene(nextLevel);
        }
    }
}
