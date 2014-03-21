using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;

public class Position : TrackedObject
{
	public static Vector3 PREV_POS = new Vector3(float.NaN, float.NaN, float.NaN);
	private static float _LAST_TICK = -1f;
	private static float _LAST_SPEED = -1f;

	const int SIZE = 15; //3 + 4 + 4 + 4;
	private Byte[] _raw = new Byte[SIZE];
	private Vector3 _obj = Vector3.zero;

	public Position()
	{
		_raw[0] = Convert.ToByte('p');
		_raw[1] = Convert.ToByte('o');
		_raw[2] = Convert.ToByte('s');
	}

	public override Byte[] Serialize()
	{
		return _raw;
	}

	public override object Unserialize()
	{
		return _obj;
	}

	/// <summary>
	/// Copy value in byte array
	/// </summary>
	/// <param name="val">float value</param>
	/// <param name="_index">byte displacement</param>
	private void _copy(float val, int index)
	{
		Byte[] tmp = BitConverter.GetBytes(val);
		for (int i = 0; i < 4; ++i)
		{
			_raw[index + i] = tmp[i];
		}
	}

	public override void SetObject(object o)
	{
		_obj = (Vector3)o;
		_copy(((Vector3)o).x, 3);
		_copy(((Vector3)o).y, 7);
		_copy(((Vector3)o).z, 11);
	}

	public override void SetBytes(byte[] o)
	{
		_obj = new Vector3(
			BitConverter.ToSingle(o, 0),
			BitConverter.ToSingle(o, 4),
			BitConverter.ToSingle(o, 8)
		);
	}

	public override bool Validates(object o)
	{
		return o is Vector3;
	}

	public override bool Validates(byte[] o)
	{
		return o.Length == SIZE
			&& o[0] == Convert.ToByte('p')
			&& o[1] == Convert.ToByte('o')
			&& o[2] == Convert.ToByte('s');
	}

	public override string GetTypeCode()
	{
		return "pos";
	}

	public override void Show(System.IO.FileStream fs, byte[] b, float time)
	{
		Position pos = new Position();
		if (!_get12Bytes(fs, b))
		{
			Debug.LogError("ERROR: Corrupted file! Can't get data!");
			return;
		}
		pos.SetBytes(b);

		if (PREV_POS.x != float.NaN)
		{
			Gizmos.color = (time < 0.5f ? Color.red : Color.green);
			Gizmos.DrawLine(PREV_POS, (Vector3)pos.Unserialize());
		}
		PREV_POS = (Vector3)pos.Unserialize();
	}

	public override void Reset()
	{
		PREV_POS = new Vector3(float.NaN, float.NaN, float.NaN);
	}

	public override void TrackVariation()
	{
		float now = Time.realtimeSinceStartup;
		if (_LAST_TICK > 0f)
		{
			float speed = Vector3.Distance(PREV_POS, _obj) / (now - _LAST_TICK);
		}
		PREV_POS = _obj;
		_LAST_TICK = now;
	}
}
