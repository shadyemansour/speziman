using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;
using System.Text;
using System.Linq;
using System.Collections;
using UnityEditor;

public class MouseController : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
     public float idleTimeToHideCursor = 2f; 

    private Vector3 lastMousePosition;
    private float idleTimer;
    private static MouseController instance;


   

    void Awake()
    {
         if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        cursorTexture = Resources.Load<Texture2D>("Sprites/cursor");
        if (cursorTexture != null)
        {

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;

            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

            lastMousePosition = Input.mousePosition;
            idleTimer = 0f;
        }

       
    }

    void FixedUpdate()
    {
        // Check if the mouse has moved
        if (Input.mousePosition != lastMousePosition)
        {
            Cursor.visible = true;
            idleTimer = 0f;
        }
        else
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTimeToHideCursor)
            {
                Cursor.visible = false;
            }
        }

        lastMousePosition = Input.mousePosition;
    }
}