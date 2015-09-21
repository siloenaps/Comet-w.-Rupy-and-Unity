using System.Collections;
using UnityEngine;
//using System;
using System.Threading;
using System.Globalization;

public class Communicator : MonoBehaviour{
	private Stream stream;
	private IEnumerator coroutinePush;
	private IEnumerator coroutinePull;

	public delegate void MyDelegate(string msg);
	MyDelegate myDelegate;

	void Start () {
		// Create socket stream object
		stream = new Stream("one");

		// PUSH message once per second test
		coroutinePush = Push (1.0F);
		StartCoroutine(coroutinePush);
	}

	IEnumerator Push(float delay) {
		while (true) {
			yield return new WaitForSeconds (delay);

			float x = Random.Range(-3.0F, 3.0F);
			float z = Random.Range(-3.0F, 3.0F);

			// Send the message
			stream.Send ("name=two&message="+x+","+z);
		}
	}

	void Update () {
		// Using Update for pulling feed from Stream 
		string[] received = stream.Receive();
		
		if(received != null) {
			for(int i = 0; i < received.Length; i++) {
				string feed = received[i];
				Debug.Log ("Update: "+feed);
				if (feed != null && feed != "noop") {					
					char[] del = { ',' };
					string[] arr = feed.Split(del);
					Vector3 pos = gameObject.transform.position;
					pos.x = float.Parse(arr[0], CultureInfo.InvariantCulture.NumberFormat);;
					pos.z = float.Parse(arr[1], CultureInfo.InvariantCulture.NumberFormat);;
					gameObject.transform.position = pos;
				}
			}
		}

		if (Input.GetKeyDown("space")){
			// (Stopping coroutine stops loop sending messages)
			StopCoroutine(coroutinePush);
			Debug.Log ("Stop");
		}
	}
}
