using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

  public Transform shotPrefab;

  public float shootingRate = 0.25f;

  private float shootCooldown;

  void Start()
  {
    shootCooldown = 0f;
  }

  void Update()
  {
    if (shootCooldown > 0)
    {
      shootCooldown -= Time.deltaTime;
    }
  }

public void Attack(Transform target)
{
    if (CanAttack)
    {
        shootCooldown = shootingRate;

        // Create a new shot
        Transform shotTransform = Instantiate(shotPrefab, transform.position, Quaternion.identity);

        // Initialize the shot's target
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
