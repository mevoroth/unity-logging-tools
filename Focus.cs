using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UnityEngine;

public class Focus : TrackedObject
{
	const int SIZE = 15; //3 + 4 + 4 + 4;
	private Byte[] _raw = new Byte[SIZE];
	private Vector3 _obj = Vector3.zero;

	public Focus()
	{
		_raw[0] = Convert.ToByte('f');
		_raw[1] = Convert.ToByte('o');
		_raw[2] = Convert.ToByte('c');
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
			&& o[0] == Convert.ToByte('f')
			&& o[1] == Convert.ToByte('o')
			&& o[2] == Convert.ToByte('c');
	}

	public override string GetTypeCode()
	{
		return "foc";
	}

	public override void Show(System.IO.FileStream fs, byte[] b, float time)
	{
		Focus foc = new Focus();
		if (!_get12Bytes(fs, b))
		{
			Debug.LogError("ERROR: Corrupted file! Can't get data!");
			return;
		}
		foc.SetBytes(b);
		Vector3 f = (Vector3)foc.Unserialize();
		f.x *= Mathf.Deg2Rad;
		f.y *= Mathf.Deg2Rad;

		Gizmos.color = Color.blue;
		Gizmos.DrawRay(Position.PREV_POS, new Vector3(
			Mathf.Cos(f.x) * Mathf.Sin(f.y),
			Mathf.Sin(f.x),
			Mathf.Cos(f.x) * Mathf.Cos(f.y)
		));
	}

	public override void Reset()
	{
		//throw new NotImplementedException();
	}

	public override void TrackVariation()
	{
		//throw new NotImplementedException();
	}
}
