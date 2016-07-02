using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public struct TrackerData
{
	public GameObject obj;
	public TrackerCell trackerCell;
}

public class TrackerList
{
	private List<TrackerData> trackerList;
	public int Index { get; private set; }
	private int listLength;

	public TrackerList()
	{
		listLength = -1;
		trackerList = new List<TrackerData>();
	}

	public void Add(TrackerData newData)
	{
		trackerList.Add(newData);
		listLength = trackerList.Count;
	}

	public void Step()
	{
		Index = ++Index % listLength;
	}

	public TrackerData this[int key]
	{
		get
		{
			return trackerList[(Index + key) % listLength];
		}
	}
}

