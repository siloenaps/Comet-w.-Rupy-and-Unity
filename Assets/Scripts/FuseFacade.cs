using System.Collections;
using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;

public class FuseFacade : MonoBehaviour{
	public string username = "";
	public string key = "";
	public string host = "fuse.rupy.se";
	public int port = 80;


	private Stream stream;

	void Start(){
		Connect ();
	}

	void Update () {
		// Using Update for pulling feed from Stream 
		string[] received = stream.Receive();
		
		if(received != null) {
			for(int i = 0; i < received.Length; i++) {
				string data = received[i];
				Debug.Log(data);
			}
		}
	}

	public void Connect() {
		try {
			stream = new Stream();
			stream.host = host;
			stream.port = port;

			bool success = false;

			//string key = "xBnm5X9nqRChgABZ"; //"RGTAbo1UxHuCFppi"; //User(username);
			if(key != null) {
				success = Auth(username, key);
			}			
			if(success) {
				stream.Connect(username); 
			}
			
			Debug.Log("Login: " + success + ".");
		}
		catch(Exception e) {
			Debug.Log(e.ToString());
		}
	}
	
	public string User(string name) {
		string[] user = stream.Send(name, "user").Split('|');
		
		if(user[0].Equals("fail")) {
			if(user[1].IndexOf("bad") > 0) {
				// limit characters to alpha numeric.
			}
			else if(user[1].IndexOf("already") > 0) {
				// prompt for other name.
			}
			
			Debug.Log("User fail: " + user[1] + ".");
			return null;
		}
		
		return user[1];
	}
	
	public bool Auth(string name, string key) {
		string salt = stream.Send(name, "salt").Split('|')[1];
		string hash = MD5(key + salt);
		string[] auth = stream.Send(name, "auth|" + salt + "|" + hash).Split('|');
		if(auth[0].Equals("fail")) {
			Debug.Log("Auth fail: " + auth[1] + ".");
			return false;
		}
		
		return true;
	}
	
	public bool Room(string name, String type, int size) {
		string[] room = stream.Send(name, "room|" + type + "|" + size).Split('|');
		
		if(room[0].Equals("fail")) {
			Debug.Log("Room fail: " + room[1] + ".");
			return false;
		}
		
		return true;
	}
	
	public string[] List(string name) {
		string list = stream.Send(name, "list");
		
		if(list.StartsWith("fail")) {
			Debug.Log("List fail: " + list + ".");
			return null;
		}
		
		return list.Substring(list.IndexOf('|') + 1).Split('|');
	}
	
	public bool Join(string name) {
		string[] join = stream.Send(name, "join").Split('|');
		
		if(join[0].Equals("fail")) {
			Debug.Log("Join fail: " + join[1] + ".");
			return false;
		}
		
		return true;
	}
	
	public bool Exit(string name) {
		string[] exit = stream.Send(name, "exit").Split('|');
		
		if(exit[0].Equals("fail")) {
			Debug.Log("Exit fail: " + exit[1] + ".");
			return false;
		}
		
		return true;
	}
	
	public void Lock(string name, string text) {
		stream.Send(name, "lock");
	}
	
	public void Chat(string name, string text) {
		stream.Send(name, "chat|" + text);
	}
	
	public void Data(string name, string data) {
		stream.Send(name, "data|" + data);
	}
	
	public static string MD5(string input) {
		MD5 md5 = System.Security.Cryptography.MD5.Create();
		byte[] bytes = Encoding.ASCII.GetBytes(input);
		byte[] hash = md5.ComputeHash(bytes);
		StringBuilder sb = new StringBuilder();
		for(int i = 0; i < hash.Length; i++) {
			sb.Append(hash[i].ToString("X2"));
		}
		return sb.ToString();
	}
}