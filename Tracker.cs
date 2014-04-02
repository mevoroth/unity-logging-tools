using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class Tracker : MonoBehaviour
{
	private struct PushedData
	{
		float _time;
		TrackedObject _to;
		public float Time
		{
			get { return _time; }
			set { _time = value; }
		}
		public TrackedObject Object
		{
			get { return _to; }
			set { _to = value; }
		}
	}

	const int BUFFER_SIZE = 1048576; // 1024*1024

	public float m_refreshFrequency = 0.5f;

	private int _index = 0;
	private byte[] _buffer = new byte[BUFFER_SIZE];
	private string _filename;
	private Queue<PushedData> _pushedDatas = new Queue<PushedData>();

	private FileStream _excelFs;
	private int _excelBufferSize = 0;
	private string _excelBuffer = "";
	private string _excelFilename;

	private bool _enabled = false;

	IEnumerator log()
	{
		TrackedObjectProcessor.InitRefreshFrequency(m_refreshFrequency);
		PushedData pd;
		while (true)
		{
			while (_pushedDatas.Count > 0)
			{
				pd = _pushedDatas.Dequeue();
				_Copy(BitConverter.GetBytes(pd.Time));
				_Copy(pd.Object.Serialize());
				TrackedObjectProcessor.TrackVariation(pd.Object);
			}

			float time = Time.realtimeSinceStartup;
			_Copy(BitConverter.GetBytes(time));
			Position p = new Position();
			p.SetObject(transform.position);
			_Copy(p.Serialize());
			TrackedObjectProcessor.TrackVariation(p);

			_Copy(BitConverter.GetBytes(time));
			Focus f = new Focus();
			f.SetObject(transform.eulerAngles);
			_Copy(f.Serialize());
			TrackedObjectProcessor.TrackVariation(f);

			m_refreshFrequency = TrackedObjectProcessor.GetRefreshFrequency();

			yield return new WaitForSeconds(m_refreshFrequency);
		}
	}

	void Awake()
	{
		string path = Application.dataPath.ToString() + "/PlayerLogs";
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		int i = 0;
		for (; File.Exists(path + "/PlayerLog_" + i + ".log"); ++i);

		_filename = path + "/PlayerLog_" + i + ".log";
		_excelFilename = path + "/PlayerLog_" + i + ".xls";

		_excelFs = System.IO.File.Open(_excelFilename, FileMode.Append);
		_excelFs.Write(System.Text.Encoding.UTF8.GetBytes("<table>"), 0, 7);

		_StartCoroutine();
	}

	private void _StartCoroutine()
	{
		if (!_enabled)
		{
			_enabled = true;
			StartCoroutine("log");
		}
	}

	void OnDestroy()
	{
		_StopCoroutine();
		_excelFs.Write(System.Text.Encoding.UTF8.GetBytes("</table>"), 0, 8);
		_excelFs.Close();
	}

	private void _StopCoroutine()
	{
		if (_enabled)
		{
			_enabled = false;
			StopCoroutine("log");
			_ArchiveExcel();
			_Archive();
		}
	}

	void OnDisable()
	{
		_StopCoroutine();
	}

	void OnEnable()
	{
		_StartCoroutine();
	}

	private void _Archive()
	{
		FileStream fs = System.IO.File.Open(_filename, FileMode.Append);
		fs.Write(_buffer, 0, _index);
		fs.Close();
		_index = 0;
	}

	private void _ArchiveExcel()
	{
		_excelFs.Write(System.Text.Encoding.UTF8.GetBytes(_excelBuffer), 0, _excelBuffer.Length);
		_excelBufferSize = 0;
	}

	private void _Copy(byte[] src)
	{
		if (_index + src.Length > BUFFER_SIZE)
		{
			_Archive();
		}
		Buffer.BlockCopy(src, 0, _buffer, _index, src.Length);
		_index += src.Length;
	}

	public void Push(TrackedObject to)
	{
		PushedData pd = new PushedData();
		pd.Time = Time.realtimeSinceStartup;
		pd.Object = to;
		_pushedDatas.Enqueue(pd);
	}

	public void PushExcelLine(string[] cells)
	{
		string buffer = "<tr>";
		buffer += "<td>" + Time.realtimeSinceStartup + "</td>";
		for (int i = 0, c = cells.Length; i < c; ++i)
		{
			buffer += "<td>" + cells[i] + "</td>";
		}
		buffer += "</td>";
		_excelBuffer += buffer;
		_excelBufferSize += buffer.Length;
		if (_excelBufferSize >= BUFFER_SIZE)
		{
			_ArchiveExcel();
		}
	}
}
