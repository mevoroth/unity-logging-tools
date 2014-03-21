using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class TrackedObjectProcessor
{
	private static List<TrackedObject> OBJECTS = new List<TrackedObject>();

	private static float _refreshFrequencyInit = 0.0f;
	public static float _refreshFrequency = 0.0f;
	
	static TrackedObjectProcessor()
	{
		OBJECTS.Add(new Position());
		OBJECTS.Add(new Focus());
	}
	
	public static TrackedObject getObject(string type)
	{
		foreach (TrackedObject to in OBJECTS)
		{
			if (to.GetTypeCode() == type)
			{
				return to;
			}
		}
		return null;
	}

	public static void Reset()
	{
		foreach (TrackedObject to in OBJECTS)
		{
			to.Reset();
		}
	}

	public static void InitRefreshFrequency(float m_refreshFrequency)
	{
		//Debug.Log("INTERNAL : " + _refreshFrequency);
		//Debug.Log("IN : " + m_refreshFrequency);
 		_refreshFrequency = m_refreshFrequency;
		_refreshFrequencyInit = m_refreshFrequency;
	}

	public static void TrackVariation(TrackedObject to)
	{
		to.TrackVariation();
	}

	public static float GetRefreshFrequency()
	{
		return _refreshFrequencyInit;
	}
}
