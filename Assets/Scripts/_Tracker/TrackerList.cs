using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public struct TrackerData
{
	public GameObject obj;
	public ITrackerDisplay trackerDisplay;
}

public class TrackerList<T>
{
	private List<T> trackerList;
	public int Index { get; private set; }
	private int listLength;

	public TrackerList()
	{
		listLength = -1;
		trackerList = new List<T>();
	}

	public void Add(T newData)
	{
		trackerList.Add(newData);
		listLength = trackerList.Count;
	}

	public T Step()
	{
		T lastEntry = trackerList[Index];
		Index = ++Index % listLength;
		return lastEntry;
	}

	public T this[int key]
	{
		get
		{
			return trackerList[(Index + key) % listLength];
		}
	}

	public override string ToString()
	{
		string o = "";
		foreach (T t in trackerList)
		{
			o += t.ToString() + ", ";
		}
		return o;
	}
}

