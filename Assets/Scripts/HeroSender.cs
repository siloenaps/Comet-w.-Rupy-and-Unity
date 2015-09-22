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

	private void Push(Vector3 pos, Quaternion rot) {	
		int tmpx = (int)(pos.x * 100);
		int tmpz = (int)(pos.z * 100);
		int tmproty = (int)(rot.eulerAngles.y * 10);

		// Send the message
		stream.Send ("name=two&message="+tmpx+","+tmpz+","+tmproty);
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
			Push(transform.position, transform.rotation);
		}
	}
}
