using UnityEngine;

// Simple struct that is used in the editor to store needed information about an individual audio clip
[System.Serializable]
public struct SongStruct
{
	public AudioClip audioTrack;
	public float beatTime;
	public int beatsPerBar;
}