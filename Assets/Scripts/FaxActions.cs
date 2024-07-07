using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaxActions : MonoBehaviour, IActions
{
    private Printer fax;
    public System.Action[] Callbacks => _callbacks;
    public System.Action[] _callbacks = new System.Action[] { };

    public string[] Messages => _messages;

    private string[] _messages = new string[]
    {
        "But there were, as all business owners in germany know,",
        "still some obstacles to overcome...",

    };


    void Awake()
    {
        fax = FindObjectOfType<Printer>();
    }

    public void NextLevel()
    {
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
                fax.isActiveAndEnabled = true;
                _callbacks[0]?.Invoke();
                _callbacks[1]?.Invoke();
                break;
        }
    }


    public void SetCallbacks(System.Action[] callbacks)
    {
        _callbacks = callbacks;
    }
}
