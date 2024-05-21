using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private string[] dialogue;
    private int index;
    [SerializeField] private float wordSpeed;
    [SerializeField] private bool playerInRange;
    [SerializeField] private GameObject contButton;
    [SerializeField] private PlayerController player;
    [SerializeField] private FireballController fireball;
    [SerializeField] private BoxCollider2D dialogueTrigger;
    [SerializeField] private GameObject prompt;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private NPCController NPC;
    private bool buttonActive = false;
    private bool firstTime = true;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            if (dialoguePanel.activeInHierarchy && buttonActive)
            {
                NextLine();
            }
            else if (firstTime)
            {
                firstTime = false;
                dialoguePanel.SetActive(true);
                player.enabled = false; // Disable movement
                fireball.enabled = false;
                player.animator.runtimeAnimatorController = player.idleControllers[player.getCurrentControllerIndex()]; // Make sure players in an idle state
                prompt.SetActive(false);
                StartCoroutine(text());
            }
        }

        if (dialogueText.text == dialogue[index])
        {
            if(index == dialogue.Length - 1)
                buttonText.text = "OK"; // Player accepts NPC request
            contButton.SetActive(true);
            buttonActive = true;
        }
    }

    public void endDialogue()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
        player.enabled = true; // Re-enable player movement
        fireball.enabled = true;
        dialogueTrigger.enabled = false; // Disable trigger zone
        NPC.enabled = true; // Enable the NPC Controller
    }

    IEnumerator text()
    {
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    public void NextLine()
    {
        buttonActive = false;
        contButton.SetActive(false);

        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(text());
        }
        else
        {
            endDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            prompt.SetActive(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            prompt.SetActive(false);
            playerInRange = false;
        }
    }
}
