using System.Collections;
using UnityEngine;
using System;
using System.Threading;
using System.Globalization;

public class HeroSender : MonoBehaviour{
	public string username = "three";
	public float speed = 10.0F;
	public float rotationSpeed = 100.0F;

	private Stream stream;
	private float translation;
	private float rotation;

	private bool maySend = false;

//	void Start () {
//		try {
//			stream = new Stream();
//			
//			// if no key is stored try
//			//string key = stream.join(name);
//			//   then store name and key
//			// otherwise
//			//   get name and key
//			string key = stream.join(username);
//			bool success = false;
//			
//			if(key != null) {
//				success = stream.auth(username, key);
//			}
//			
//			if(success) {				
//				// this will allow you to Stream.Receive();
//				stream.Connect(username); 
//			}
//			
//			Debug.Log("Login: " + username+" is "+success + ".");
//
//			StartCoroutine(Push());
//		}
//		catch(Exception e) {
//			Debug.LogException(e);
//		}
//	}
//
////	private void Push(Vector3 pos, Quaternion rot) {	
////		int tmpx = (int)(pos.x * 100);
////		int tmpz = (int)(pos.z * 100);
////		int tmproty = (int)(rot.eulerAngles.y * 10);
////
////		// Send the message
////		stream.chat(username, tmpx+","+tmpz+","+tmproty);
////	}
//
//	IEnumerator Push() {
//
//		yield return new WaitForSeconds(0.1f);
//
//		int tmpx = (int)(transform.position.x * 100);
//		int tmpz = (int)(transform.position.z * 100);
//		int tmproty = (int)(transform.rotation.eulerAngles.y * 10);
//		
//		// Send the message
//		stream.chat(username, tmpx+","+tmpz+","+tmproty);
//
//		StartCoroutine(Push());
//	}
//
//
//	void Update () {
//		// Move
//		translation = Input.GetAxis("Vertical") * speed;
//		rotation = Input.GetAxis("Horizontal") * rotationSpeed;
//		translation *= Time.deltaTime;
//		rotation *= Time.deltaTime;
//
//		if (translation != 0 || rotation != 0) {
//			// Move myself
//			transform.Translate(0, 0, translation);
//			transform.Rotate(0, rotation, 0);
//
//			// Push the same movement data
////			Push(transform.position, transform.rotation);
//		}
//	}
}
