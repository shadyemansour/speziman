using System.Diagnostics.Tracing;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private AudioSource talkingAudioSource;
    [SerializeField] private Button_UI button;
    [SerializeField] private Fall specialOrange;
    [SerializeField] private Image background;
    [SerializeField] private Helmut helmut;
    [SerializeField] private GameObject buttonNext;
    [SerializeField] private GameObject bigButton;

    private int count = 0;
    private bool isDone = false;
    private TextWriter.TextWriterSingle textWriterSingle;

    private string[] messageArray = new string[]
    {
        "Once upon a time, roughly around 1956, Helmut was sitting underneath an orange tree.",
        "It was a sunny afternoon and he was enjoying his first holiday in Italy when suddenly...",
        "an orange fell on his head.",
        "After a short rest he came up with a brilliant idea.",
        "A wonderful brewed mixture to change peoples life for the better.",
        "He was in a hurry, because his mission was of undeniable importance.",
    };

    private void Awake()
    {
        if (messageText == null || talkingAudioSource == null || button == null)
        {
            Debug.LogError("Some UI components are not assigned!");
            return;
        }

        button.ClickFunc = HandleButtonClick;
    }

    private void HandleButtonClick()
    {
        if (textWriterSingle != null && textWriterSingle.IsActive())
        {
            textWriterSingle.WriteAllAndDestroy();
        }
        else
        {
            if(count == 0) background.color = Color.white;
            DisplayNextMessage();
        }
    }

    private void DisplayNextMessage()
    {
        if (count >= messageArray.Length)
        {
            isDone = true;
            return;
        }
        switch (count)
        {
            case 1:
                helmut.Drink();
                break;
            case 2:
                helmut.PauseDrinking();
                specialOrange.StartFall();
                break;
            case 3:
                helmut.Idea();
                break;
            case 5:
                helmut.WearGlasses();
                bigButton.SetActive(false);
                buttonNext.SetActive(true);
                break;

        }
        string message = messageArray[count++];
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
}
