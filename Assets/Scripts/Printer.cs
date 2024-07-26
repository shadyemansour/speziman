using UnityEngine;

public class Printer : MonoBehaviour
{

  public Transform shotPrefab;

  public float shootingRate = 0.25f;

  private float shootCooldown;
  public bool isEnabled = false;

  void Start()
  {
    shootCooldown = 0f;
  }

  void Update()
  {
    if (isEnabled)
    {
      if (shootCooldown > 0)
      {
        shootCooldown -= Time.deltaTime;
      }
      else
      {
        Print();
      }
    }
  }

  public void Print()
  {
    Transform shotTransform = Instantiate(shotPrefab, transform.position, Quaternion.identity);
    shotTransform.localScale = new Vector3(3f, 3f, 3f);
    Transform target = GameObject.FindGameObjectWithTag("Player").transform;
    while (CanAttack)
    {
      shootCooldown = shootingRate;

      if (shotTransform.TryGetComponent(out Shot shot))
      {
        shot.Init(target);
      }
    }
  }



  public bool CanAttack
  {
    get
    {
      return shootCooldown <= 0f;
    }
  }
}
