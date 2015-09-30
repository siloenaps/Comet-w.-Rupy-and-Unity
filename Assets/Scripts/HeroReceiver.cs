using System.Collections;
using UnityEngine;
using System.Threading;
using System.Globalization;
using System;

public class HeroReceiver : MonoBehaviour{
	public string username = "two";
	private Stream stream;

//	void Start () {
//		try {
//			stream = new Stream();
//			
//			// if no key is stored try
//			//string key = stream.join(name);
//			//   then store name and key
//			// otherwise
//			//   get name and key
////			string key = "SFwPWQLZcBAES7BZ";
//			string key = stream.join(username);
//			Debug.Log(key);
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
//		}
//		catch(Exception e) {
//			Debug.LogException(e);
//		}
//	}
//
//	void Update () {
//
//		// Using Update for pulling feed from Stream 
//		string[] received = stream.Receive();
//
//		// Receive data
//		if(received != null) {
//			for(int i = 0; i < received.Length; i++) {
//				string data = received[i];
//				if (data != null && data != "noop") {
//
//					char[] del = { '|', ',' };
//					string[] arr = data.Split(del);
//
//					float x = float.Parse(arr[2], CultureInfo.InvariantCulture.NumberFormat) / 100f;
//					float z = float.Parse(arr[3], CultureInfo.InvariantCulture.NumberFormat) / 100f;
//					float roty = float.Parse(arr[4], CultureInfo.InvariantCulture.NumberFormat) / 10f;
//
//					transform.position = Vector3.Slerp(transform.position, new Vector3(x, 0, z), Time.deltaTime*300f); //new Vector3(x, 0, z);
//					transform.rotation = Quaternion.Euler(0, roty, 0);
//				}
//			}
//		}
//	}
}
