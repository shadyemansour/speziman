using System.Diagnostics.Tracing;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Text messageText;
    [SerializeField] private AudioSource talkingAudioSource;
    [SerializeField] private Button_UI button;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI buttonNext;
    [SerializeField] private Animator animator;
    private IActions levelData;
    private bool buttonEnabled = true;

    private int count = 0;
    private TextWriter.TextWriterSingle textWriterSingle;

    private void Awake()
    {
        if (messageText == null || talkingAudioSource == null || button == null)
        {
            Debug.LogError("Some UI components are not assigned!");
            return;
        }

        button.ClickFunc = HandleButtonClick;
        levelData  = FindComponentImplementingIActions();
        levelData.SetCallbacks(new System.Action[] { EnableButton, NextButton });
        buttonNext.transform.parent.gameObject.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.LoadNextLevel(true));

    }

    
    private IActions FindComponentImplementingIActions()
    {
        foreach (var component in FindObjectsOfType<MonoBehaviour>())
        {
            if (component is IActions)
            {
                return (IActions)component;
            }
        }
        Debug.LogError("No component implementing IActions was found!");
        return null;
    }

    private void HandleButtonClick()
    {
        if(buttonEnabled)
        {
            buttonEnabled = false;
            if (textWriterSingle != null && textWriterSingle.IsActive())
            {
                textWriterSingle.WriteAllAndDestroy();
            }
            else
            {
                if (count == 0) background.color = Color.white;
                animator.enabled=false;
                DisplayNextMessage();
            }
        }
    }

    private void DisplayNextMessage()
    {
        if (count >= levelData.Messages.Length)
        {
            return;
        }

        

        string message = levelData.Messages[count++];
        levelData.Actions(count);
        StartTalkingSound();
        textWriterSingle = TextWriter.AddWriter_Static(messageText, message, 0.02f, true, true, StopTalkingSound);
    }

    private void StartTalkingSound()
    {
        talkingAudioSource.Play();
    }

    private void StopTalkingSound()
    {
        talkingAudioSource.Stop();
    }

    public void EnableButton()
    {
        buttonEnabled = true;
        animator.enabled = true;
    }

    public void NextButton()
    {
        buttonNext.text = "Next";

    }
}
