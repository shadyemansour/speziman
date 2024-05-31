using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PickUpUIMove : MonoBehaviour
{
	[Range(-1000f, 1000f)]
	public float TargetPositionLeft = -450f;
	[Range(-1000f, 1000f)]
	public float TargetPositionTop = 160f;
	[Range(0.001f, 1000f)]
	public float Speed = 1.0f;
	public float closeEnough;

	private Vector2 targetPosition;
	private Vector2 currentPosition;
	private Transform objTrans;
	private float distanceToTarget;

	public GameObject ResourceImage;
	public GameObject ResourceText;
	public string ResourceTypeName;
	public int textAmount;
	public string getText;

	// Use this for initialization
	void Start ()
	{
		//Get a reference to the UI object
		objTrans = GetComponent<Transform> ();

		//Get its current position
		currentPosition = objTrans.position;

		//Get a reference to where we want it to go
		targetPosition = new Vector2(TargetPositionLeft, TargetPositionTop);

		//Run the UpdateTotal, which updates the UI Text for the number of this resource
		UpdateTotal ();
	}

	//This Sets the Resource Type
public void SetResourceType(ResourceType resourceType)
{
    switch (resourceType)
    {
        case ResourceType.Orange:
            ResourceImage = GameObject.FindGameObjectWithTag("Orange_Tag");
            ResourceText = GameObject.FindGameObjectWithTag("Orange_Text");
            getText = ResourceText.GetComponent<TextMeshProUGUI>().text;
            break;
        case ResourceType.Cola:
            ResourceImage = GameObject.FindGameObjectWithTag("Cola_Tag");
            ResourceText = GameObject.FindGameObjectWithTag("Cola_Text");
            getText = ResourceText.GetComponent<TextMeshProUGUI>().text;
            break;
    }
}

	// Update is called once per frame
	void Update ()
	{
		//Get a position that is a little bit closer to our goal position
		objTrans.position = Vector2.Lerp(currentPosition, targetPosition, Speed * Time.deltaTime);

		//Set our object to that new position
		currentPosition = objTrans.position;

		//How far are we from our goal?
		distanceToTarget = Vector2.Distance (currentPosition, targetPosition);
	}

	void LateUpdate()
	{
		//If we are close enough and we want the icon to disappear...
		if (distanceToTarget < closeEnough)
		{
			//Bonus: Make the default icon animate as the new resource is brought in
			//Swell the resource icon

			//Destroy the object, we are done with it!
			Destroy (gameObject);
		}
	}

	void UpdateTotal()
	{
		//Increase the resource count

		//Turn the displayed text into an integer (int) (a number)
		int.TryParse (getText, out textAmount);

		//Increase this number by 1
		textAmount++;

		//Turn this number back into text
		getText = textAmount.ToString ();

		//Update the text object's display
		ResourceText.GetComponent<TextMeshProUGUI> ().text = getText;

		//Force a refresh of the mesh display
		ResourceText.GetComponent<TextMeshProUGUI> ().ForceMeshUpdate ();
	}
}
