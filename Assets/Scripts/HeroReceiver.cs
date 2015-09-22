using System.Collections;
using UnityEngine;
//using System;
using System.Threading;
using System.Globalization;

public class HeroReceiver : MonoBehaviour{
	private Stream stream;

	void Start () {
		// Create socket stream object
		stream = StreamFactory.stream;
	}

	void Update () {
		// Using Update for pulling feed from Stream 
		string[] received = stream.Receive();

		// Receive data
		if(received != null) {
			for(int i = 0; i < received.Length; i++) {
				string data = received[i];
				if (data != null && data != "noop") {					
					char[] del = { ',' };
					string[] arr = data.Split(del);
					float tra = float.Parse(arr[0], CultureInfo.InvariantCulture.NumberFormat);
					float rot = float.Parse(arr[1], CultureInfo.InvariantCulture.NumberFormat);
					
					tra = tra / 1000f;
					rot = rot / 1000f;

					transform.Translate(0, 0, tra);
					transform.Rotate(0, rot, 0);
				}
			}
		}
	}
}
