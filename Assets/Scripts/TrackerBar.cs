using UnityEngine;
using System.Collections;			// You will need this for IEnumerator, which Coroutines use
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public class TrackerBar : MonoBehaviour
{
	[SerializeField]
	private int trackerCellCount = 32;
	[SerializeField]
	private Vector3 trackerWidth;

	[SerializeField]
	private GameObject TrackerCellPrefab;

	[SerializeField]
	private Color mainColor;
	[SerializeField]
	private Color alternateColor;

	[SerializeField]
	private float tickTime = 0.5f;

	private TrackerList trackerList;

	[SerializeField]
	private int writeNodeValue = 16;

	private Coroutine activeCoroutine;

	private Vector3 trackerPoint;

	void Start()
	{
		trackerList = new TrackerList();

		trackerPoint = Vector3.zero;
		bool altColor = false;
		for (int j = 0; j < trackerCellCount; j++)
		{
			// Create all our tracker cells from the prefab:
			GameObject createdObject = Instantiate(TrackerCellPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			// Parent them to us so they are held within us in the heirarchy
			// (There is also a SetParent method but this is mostly useful to apply aspects of our transform)
			// (I'm always just lazy and reposition it after creation:)
			createdObject.transform.parent = transform;

			createdObject.transform.localPosition = trackerPoint;
			// spaced out by the specified width
			trackerPoint += trackerWidth;

			// Create an instance of our trackerData struct
			var data = new TrackerData();
			// and populate it
			data.obj = createdObject;
			data.trackerCell = createdObject.GetComponent<TrackerCell>();

			// Alternate the background colour of the bar for every other cell
			SpriteRenderer rend = data.trackerCell.spriteRenderer;
			if (rend != null)
			{
				var selectedColor = mainColor;
				if (altColor)
					selectedColor = alternateColor;
				rend.color = selectedColor;
				altColor = !altColor;
			}

			// Add this data to the end of the linked list
			trackerList.Add(data);
		}

		activeCoroutine = StartCoroutine(tickCoroutine());
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Z))
		{
			trackerList[writeNodeValue].trackerCell.TakeInput(BUTTON.A);
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			trackerList[writeNodeValue].trackerCell.TakeInput(BUTTON.B);
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			trackerList[writeNodeValue].trackerCell.TakeInput(BUTTON.X);
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			trackerList[writeNodeValue].trackerCell.TakeInput(BUTTON.Y);
		}
	}

	IEnumerator tickCoroutine()
	{
		for(;;) {
			yield return new WaitForSeconds(tickTime);


			if (trackerList == null)
				return false;

			transform.position -= trackerWidth;

			// take the first node and then remove it
			var firstNode = trackerList[0];

			trackerList.Step();
			
			// reposition it to the end of the pile:
			firstNode.obj.transform.localPosition = trackerPoint;
			trackerPoint += trackerWidth;

			firstNode.trackerCell.ResetAllBars();
		}
	}
}
