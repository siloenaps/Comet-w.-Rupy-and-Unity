using System.Collections;
using UnityEngine;
//using System;
using System.Threading;
using System.Globalization;

public class HeroSender : MonoBehaviour{
	public float speed = 10.0F;
	public float rotationSpeed = 100.0F;
	private Stream stream;
	private float translation;
	private float rotation;

	void Start () {
		// Create socket stream object
		stream = StreamFactory.stream;
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

		// Move
		translation = Input.GetAxis("Vertical") * speed;
		rotation = Input.GetAxis("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;

		if (translation != 0 || rotation != 0) {
			// Move myself
			transform.Translate(0, 0, translation);
			transform.Rotate(0, rotation, 0);

			// Push the same movement data
			Push(translation, rotation);
		}
	}
}
