using System.Collections;
using UnityEngine;
//using System;
using System.Threading;
using System.Globalization;

public class Hero : MonoBehaviour{
	public float speed = 10.0F;
	public float rotationSpeed = 100.0F;
	private Stream stream;
	private float translation;
	private float rotation;

	void Start () {
		// Create socket stream object
		stream = new Stream("one");
	}

	private void Push(float tra, float rot) {	
		int t = (int)(tra * 1000);
		int r = (int)(rot * 1000);

		// Send the message
		stream.Send ("name=two&message="+t+","+r);
	}

	void Update () {
		// Using Update for pulling feed from Stream 
		string[] received = stream.Receive();

//		Debug.Log(received);
		
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

		// Move
		translation = Input.GetAxis("Vertical") * speed;
		rotation = Input.GetAxis("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;

		if (translation != 0 || rotation != 0) {
			Push(translation, rotation);
		}
	}
}
