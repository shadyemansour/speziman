using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroActions : MonoBehaviour, IActions
{
    private Fall specialOrange;
    private Helmut helmut;
    public System.Action[] Callbacks => _callbacks;
    public System.Action[] _callbacks = new System.Action[] { };

    public string[] Messages => _messages;

    private string[] _messages = new string[]
    {
        "Once upon a time, roughly around 1956, Helmut was sitting underneath an orange tree.",
        "It was a sunny afternoon and he was enjoying his first vacation in Italy when suddenly...",
        "an orange fell on his head.",
        "After a short rest he came up with a brilliant idea!",
        "A wonderful brewed mixture to change peoples life for the better...",
        "He was in a hurry, because his mission was of undeniable importance.",
    };


    void Awake()
    {
        specialOrange = FindObjectOfType<Fall>();
        helmut = FindObjectOfType<Helmut>();
    }

    public void NextLevel()
    {
        SoundManager.Instance.FadeOutSfxSound();
        GameManager.Instance.LoadNextLevel(true);
    }


    public void Actions(int count)
    {
        switch (count)
        {
            case 1:
                _callbacks[0]?.Invoke();
                break;
            case 2:
                helmut.Drink();
                _callbacks[0]?.Invoke();
                break;
            case 3:
                helmut.PauseDrinking(_callbacks[0]);
                specialOrange.StartFall();
                break;
            case 4:
                helmut.Idea();
                _callbacks[0]?.Invoke();
                break;
            case 5:
                _callbacks[0]?.Invoke();
                break;

            case 6:
                helmut.WearGlasses();
                _callbacks[1]?.Invoke();
                break;

        }
    }


    public void SetCallbacks(System.Action[] callbacks)
    {
        _callbacks = callbacks;
    }
}
