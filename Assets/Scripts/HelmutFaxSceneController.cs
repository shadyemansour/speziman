using UnityEngine;

public class HelmutFaxSceneController : MonoBehaviour
{

    private bool Done = false;


    // Start is called before the first frame update
    void Start()
    {


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Done && other.gameObject.name.ToLower().Contains("document"))
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            Done = true;
        }

    }

}
