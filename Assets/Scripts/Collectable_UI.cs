using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible_UI : MonoBehaviour
{
	public Camera cam;
	public Canvas canvas;
	public Vector2 WorldToScreen;
	public GameObject pickup;
	public GameObject collectible_type;
	public GameObject newCollectionUIImage;
	public Vector2 startingpos;
	public Vector2 endingpos;
	public float speed = 1.0f;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void OnTriggerEnter(Collider col)
	{
		//Get a refernece to the thing we ran into
		//Note: you should use an if statement here to make sure it is a pickup item
		pickup = col.gameObject;
		collectible_type = pickup.GetComponent<CollectibleType>().CollectType;

		//Get the point on the screen that relates to the point in the world that we picked this up
		WorldToScreen = cam.WorldToScreenPoint (col.transform.position);
		newCollectionUIImage = 	Instantiate (collectible_type, canvas.transform, false);

		//Get the type of Resource from the CollectibleType
		ResourceType ResourceType = pickup.GetComponent<CollectibleType> ().ResourceType;

		//Pass the Resource Type along
		newCollectionUIImage.GetComponent<PickUpUIMove> ().SetResourceType (ResourceType);

		//Set the inital position
		startingpos = newCollectionUIImage.GetComponent<RectTransform> ().anchoredPosition;
		startingpos = WorldToScreen;

		//Destroy this object
		Destroy (col.gameObject);
	}
}
