using System.Collections;
using UnityEngine;
//using System;
using System.Threading;
using System.Globalization;

public class Communicator : MonoBehaviour{
	private Stream stream;
	private IEnumerator coroutine;

	void Start () {
		// Create socket stream object
		stream = new Stream();

		// (Coroutine is for testing purposes. Sends message once per second)
		coroutine = Send (1.0F);
		StartCoroutine(coroutine);
	}

	IEnumerator Send(float delay) {
		while (true) {
			yield return new WaitForSeconds (delay);

			float x = Random.Range(-3.0F, 3.0F);
			float z = Random.Range(-3.0F, 3.0F);

			// Send the message
			stream.send ("name=two&message="+x+","+z);
		}
	}

	void Update () {
		// Using Update for pulling feed from Stream 
		string feed = stream.buffer.Get ();
		if (feed != null && feed != "noop") {

			char[] del = { ',' };
			string[] arr = feed.Split(del);
			Vector3 pos = gameObject.transform.position;
			pos.x = float.Parse(arr[0], CultureInfo.InvariantCulture.NumberFormat);;
			pos.z = float.Parse(arr[1], CultureInfo.InvariantCulture.NumberFormat);;
			gameObject.transform.position = pos;
		}

		if (Input.GetKeyDown("space")){
			// (Stopping coroutine stops loop sending messages)
			StopCoroutine(coroutine);
			stream.Close();
			Debug.Log ("Stop");
		}
	}

	#region Async implementation

	public void Receive (string message)
	{
		Debug.Log ("Receive: "+message);
	}

	#endregion
}