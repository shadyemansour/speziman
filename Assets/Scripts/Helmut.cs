using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmut : MonoBehaviour
{
    private SpriteRenderer imageRenderer;
    [SerializeField] private GameObject bulb;
    private bool done = false;
    private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    private Animator animator;

    void Awake()
    {
        imageRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        PreloadSprites();
    }

    private void PreloadSprites()
    {
        string[] spriteNames = { "Story 1", "Story 2", "Story 3", "Story 4" };
        foreach (var name in spriteNames)
        {
            spriteCache[name] = Resources.Load<Sprite>($"Sprites/{name}");
            if (spriteCache[name] == null)
            {
                Debug.LogError($"Failed to load sprite: Sprites/{name}");
            }
        }
    }

    private void ChangeSprite(string storyName)
    {
        if (spriteCache.TryGetValue(storyName, out Sprite newSprite))
        {
            imageRenderer.sprite = newSprite;
            Debug.Log($"{storyName} Sprite loaded");
        }
        else
        {
            Debug.LogError($"{storyName} sprite is missing!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.CompareTag("Orange") && !done)
        {
            ChangeSprite("Story 3");
            done = true;
        }
    }

    public void Drink()
    {
        animator.Play("helmut");
    }

        public void PauseDrinking()
    {
        animator.enabled = false;
        ChangeSprite("Story 2");
    }


    public void Idea()
    {
        ChangeSprite("Story 1");
        bulb.SetActive(true);
    }

    public void WearGlasses()
    {
        bulb.SetActive(false);
        ChangeSprite("Story 4");
    }
}
