using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{

    [SerializeField]
    private Vector2 parallaxEffectSpeed;
    [SerializeField]
    private GameObject spriteObject;
    [SerializeField]
    private Transform levelPlayerSpawnPoint;

    private Transform cameraTransform;
    private float startPositionY;
    private float startPositionX;
    private float spriteSizeX;



    // Start is called before the first frame update
    // public void Start()
    // /* public void InitializeBackground() */
    // {
    //     cameraTransform = Camera.main.transform;
    //     startPositionX = levelPlayerSpawnPoint.position.x;
    //     startPositionY = transform.position.y;
    //     transform.position = new Vector3 (startPositionX, transform.position.y, transform.position.z);
    //     spriteSizeX = spriteObject.GetComponent<SpriteRenderer>().bounds.size.x;
    // }

    public void InitializeBackground()
    {
        cameraTransform = Camera.main.transform;
        startPositionX = levelPlayerSpawnPoint.position.x;
        startPositionY = transform.position.y;
        transform.position = new Vector3(startPositionX, transform.position.y, transform.position.z);
        spriteSizeX = spriteObject.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (levelPlayerSpawnPoint)
        {
            float relativeDistX = cameraTransform.position.x * parallaxEffectSpeed.x;
            float relativeDistY = cameraTransform.position.y * parallaxEffectSpeed.y;
            transform.position = new Vector3(startPositionX + relativeDistX, startPositionY + relativeDistY, transform.position.z);

            float relativeCameraDist = cameraTransform.position.x * (1 - parallaxEffectSpeed.x);
            if (relativeCameraDist > startPositionX + spriteSizeX)
            {
                startPositionX += spriteSizeX;
            }
            else if (relativeCameraDist < startPositionX - spriteSizeX)
            {
                startPositionX -= spriteSizeX;
            }
        }
    }

    public void SetLevelPlayerSpawnPoint(Transform levelPlayerSpawnPoint)
    {
        this.levelPlayerSpawnPoint = levelPlayerSpawnPoint;
    }
}
