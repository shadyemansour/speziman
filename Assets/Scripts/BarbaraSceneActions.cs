using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbaraSceneActions : MonoBehaviour, IActions
{
    private HelmutBarbaraSceneController helmut;
    private BarbaraController bar;
    private CameraFollow cam;
    [SerializeField] private Dictionary<string, int> collectables;
    [SerializeField] private GameObject hearts;
    [SerializeField] private CanvasGroup speziUI;
    private int coroutinesCounter;


    public System.Action[] Callbacks => _callbacks;
    public System.Action[] _callbacks = new System.Action[] { };
    public string[] Messages => _messages;


    private int numberOfSpeziBottles = 3;
    private GameObject[] speziBottles;


    private string[] _messages = new string[]
    {
        "After he set out on his journey to collect everything he needed to start the elixir of deliciousness",
        "he returned home to Augsburg to perfect his recipe.",
        "Babsi was the first one to try...",
        "and she loved it",
        "She loved it so much, that she gave it the name Spezi, which is the bavarian word for Buddy.",
        "And that was the moment Spezi was born. The drink that should soon become everyones favorite companion.",
        "Helmut was ready to spread word  far and wide and set out on his journey to find all the Babsis of the world, that are looking for a Spezi for life.",
    };


    void Awake()
    {
        cam = FindObjectOfType<CameraFollow>();
        bar = FindObjectOfType<BarbaraController>();
        helmut = FindObjectOfType<HelmutBarbaraSceneController>();
        speziBottles = new GameObject[numberOfSpeziBottles];
        collectables = new Dictionary<string, int>
        {
            { "Orange", 10 },
            { "Cola", 6 }
        };
        coroutinesCounter = 0;
        foreach (var pair in collectables)
        {
            coroutinesCounter += pair.Value;
        }



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

                helmut.WalkTo(new Vector3(-0.815F, -2.4f, 0), StopCameraFollow);
                cam.currentStop = 0;
                cam.isFollowingPlayer = true;
                break;
            case 2:
                SendCollectables(helmut.gameObject, new Vector3(0.583f, -2.283F, 0));
                break;
            case 3:
                bar.WaveAndMove(BarbaraFinishMoving);
                break;
            case 4:
                hearts.SetActive(true);
                _callbacks[0]?.Invoke();
                break;
            case 5:
                // hearts.SetActive(false);
                UpdateBottles();
                _callbacks[0]?.Invoke();
                break;
            case 6:
                StartCoroutine(FadeIn(1f, speziUI, _callbacks[0]));
                break;
            case 7:
                bar.Wave();
                helmut.WalkTo(new Vector3(15f, -2.4f, 0), null);
                _callbacks[1]?.Invoke();
                break;

        }


    }

    private void UpdateBottles()
    {
        Sprite speziSprite = Resources.Load<Sprite>("Sprites/Spezi");
        for (int i = 0; i < numberOfSpeziBottles; i++)
        {
            speziBottles[i].GetComponent<SpriteRenderer>().sprite = speziSprite;
        }
    }

    public void BarbaraFinishDrinking()
    {
        speziBottles[0].SetActive(true);
        _callbacks[0]?.Invoke();
    }
    public void BarbaraFinishMoving()
    {
        speziBottles[0].SetActive(false);
        bar.StartDrinking(BarbaraFinishDrinking);
        _callbacks[0]?.Invoke();
    }

    public void SetCallbacks(System.Action[] callbacks)
    {
        _callbacks = callbacks;
    }

    public void StopCameraFollow()
    {
        cam.isFollowingPlayer = false;
        _callbacks[0]?.Invoke();
    }

    public void TriggerSpeziOut()
    {
        cam.isFollowingPlayer = false;
        for (int i = 0; i < numberOfSpeziBottles; i++)
        {
            GameObject instance = Instantiate(Resources.Load<GameObject>("Prefabs/unknownAnim"), new Vector3(3.549f, -2.283f, 0), Quaternion.identity);
            speziBottles[i] = instance;
            StartCoroutine(MoveCollectable(instance, new Vector3(6.9f - (0.5f * i), -2.4f, 0), 1.0f, InvokeCallback, false));
        }
    }

    private void InvokeCallback()
    {
        coroutinesCounter--;
        Debug.Log(coroutinesCounter);
        if (coroutinesCounter == 0) _callbacks[0]?.Invoke();
    }


    public void SendCollectables(GameObject player, Vector3 barbaraPosition)
    {
        StartCoroutine(SendCollectablesCoroutine(player, barbaraPosition, 0.2f));
    }

    private IEnumerator SendCollectablesCoroutine(GameObject player, Vector3 targetPosition, float delayBetweenItems)
    {

        foreach (var pair in collectables)
        {
            for (int i = 0; i < pair.Value; i++)
            {
                string prefabPath = $"Prefabs/{pair.Key.ToLower()}Anim";

                GameObject instance = Instantiate(Resources.Load<GameObject>(prefabPath), player.transform.position, Quaternion.identity);
                float yoffset = pair.Key.ToLower().Contains("orange") ? 0.231f : 0;
                Vector3 newPosition = new Vector3(targetPosition.x, targetPosition.y - yoffset, targetPosition.z);
                StartCoroutine(MoveCollectableInParabola(instance, newPosition, 1.0f));
                yield return new WaitForSeconds(delayBetweenItems);
            }

        }
    }

    IEnumerator MoveCollectableInParabola(GameObject collectable, Vector3 targetPos, float duration)
    {
        float time = 0;
        Vector3 startPos = collectable.transform.position;
        float height = Mathf.Abs(targetPos.y - startPos.y) / 2 + 2; // Height of the parabola
        while (time < duration)
        {
            float t = time / duration; // Normalize time

            collectable.transform.position = Vector3.Lerp(startPos, targetPos, t) + new Vector3(0, height * Mathf.Sin(Mathf.PI * t), 0);
            time += Time.deltaTime;
            yield return null;
        }

        collectable.transform.position = targetPos;

        //Trigger movement to the right
        float yoffset = collectable.name.ToLower().Contains("orange") ? 0.231f : 0;
        StartCoroutine(MoveCollectable(collectable, new Vector3(3.549f, -2.283f - yoffset, 0), 1.0f, ContinueCase2));
        yield return null;

    }
    IEnumerator MoveCollectable(GameObject collectable, Vector3 targetPos, float duration, System.Action onCompleted, bool destroy = true)
    {
        float time = 0;
        Vector3 startPos = collectable.transform.position;

        while (time < duration)
        {
            float t = time / duration; // Normalize time
            collectable.transform.position = Vector3.Lerp(startPos, targetPos, t);
            time += Time.deltaTime;
            yield return null;
        }
        collectable.transform.position = targetPos; // Ensure it ends exactly at the target position
        if (destroy) Destroy(collectable);
        onCompleted?.Invoke();
    }

    void ContinueCase2()
    {
        coroutinesCounter--;
        if (coroutinesCounter == 0)
        {
            cam.currentStop = 1;
            cam.isFollowingPlayer = true;
            coroutinesCounter = numberOfSpeziBottles;
            helmut.WalkTo(new Vector3(5.28f, -2.4f, 0), TriggerSpeziOut);
        }
    }

    IEnumerator FadeIn(float fadeInTime, CanvasGroup canvasGroup, System.Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeInTime);
            yield return null;
        }
        canvasGroup.alpha = 1;
        onComplete?.Invoke();
    }





}
