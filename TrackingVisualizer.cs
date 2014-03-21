using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

public class TrackingVisualizer : MonoBehaviour {
	//private Vector3 _prevFoc = new Vector3(float.NaN, float.NaN, float.NaN);
	public float m_timeline = 0.0f;

	void OnDrawGizmosSelected()
	{
		string path = Application.dataPath.ToString() + "/PlayerLogs";
		int i = 0;
		byte[] buffer = new byte[1024];
		//Debug.Log("START VISUALIZATION");
		for (; File.Exists(path + "/PlayerLog_" + i + ".log"); ++i)
		{
			//Debug.Log("START OF LOG: " + path + "/PlayerLog_" + i + ".log");
			FileStream fs = File.OpenRead(path + "/PlayerLog_" + i + ".log");
			TrackedObjectProcessor.Reset();
			
			int read = fs.Read(buffer, 0, 4);
			while (read > 0)
			{
				// PROCESS TIME
				// LOL AINT NOBODY GOT TIME FO' DAT
				float time = BitConverter.ToSingle(buffer, 0);

				read = fs.Read(buffer, 0, 3);
				if (read == 0)
				{
					Debug.LogError("ERROR: Corrupted file!");
					break;
				}

				string type = Encoding.UTF8.GetString(buffer, 0, 3);
				TrackedObject to = TrackedObjectProcessor.getObject(type);

				if (to == null)
				{
					Debug.LogError("ERROR: Corrupted file!");
					break;
				}

				to.Show(fs, buffer, Mathf.Abs(time - m_timeline));

				read = fs.Read(buffer, 0, 4);
			}
			//Debug.Log("END OF LOG");

			fs.Close();
		}
		//Debug.Log("END VISUALIZATION");
	}
}
