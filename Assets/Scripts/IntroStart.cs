using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroStart : MonoBehaviour
{
    public void StartGame()
    {
    GameManager.Instance.LoadLevel(1);
    }
}
