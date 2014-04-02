using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

abstract public class TrackedObject
{
	abstract public void SetObject(Object o);
	abstract public void SetBytes(Byte[] o);
	abstract public bool Validates(Object o);
	abstract public bool Validates(Byte[] o);
	abstract public Byte[] Serialize();
	abstract public Object Unserialize();
	abstract public string GetTypeCode();
	abstract public void Show(FileStream fs, byte[] b, float time);
	protected bool _get12Bytes(FileStream fs, byte[] b)
	{
		int read = fs.Read(b, 0, 12);
		return read == 12;
	}
	protected bool _get16Bytes(FileStream fs, byte[] b)
	{
		int read = fs.Read(b, 0, 16);
		return read == 16;
	}
	abstract public void Reset();
	abstract public void TrackVariation();
}
