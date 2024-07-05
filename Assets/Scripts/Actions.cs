using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActions 
{

    string[] Messages { get; }
    System.Action[] Callbacks { get; }
    void Actions(int count);
    void SetCallbacks(System.Action[] callbacks);
    void NextLevel();





   
}
