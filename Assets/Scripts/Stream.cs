//using UnityEngine; // policy
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Text;

/* A real-time comet stream plugin for rupy.
 * For unity policy check, uncomment lines with: // policy
 * For usage scroll down to main() method.
 */
using UnityEngine;


public class Stream {
	public string host = "talk.rupy.se";
	public int port = 80;
	
	private Queue<string> queue;
	private Socket pull, push;
	
	private class State {
		public Socket socket = null;
		public const int size = 32768;
		public byte[] data = new byte[size];
	}
	
	public Stream(String name) {
		queue = new Queue<string>();
		
		bool policy = true;
		
		//policy = Security.PrefetchSocketPolicy(host, port); // policy
		
		if(!policy)
			throw new Exception("Policy (" + host + ":" + port + ") failed.");
		
		IPAddress address = Dns.Resolve(host).AddressList[0];
		IPEndPoint remote = new IPEndPoint(address, port);
		
		push = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		pull = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		
		push.Connect(remote);
		pull.Connect(remote);
		
		String data = "GET /pull?name=" + name + " HTTP/1.1\r\n"
			+ "Host: " + host + "\r\n"
				+ "Head: less\r\n\r\n"; // enables TCP no delay
		
		pull.Send(Encoding.ASCII.GetBytes(data));
		
		State state = new State();
		state.socket = pull;
		
		pull.BeginReceive(state.data, 0, State.size, 0, new AsyncCallback(Callback), state);
	}
	
	public void Send(String message) {
		byte[] data = new byte[64];
		String text = "POST /push HTTP/1.1\r\n"
			+ "Host: " + host + "\r\n"
				+ "Head: less\r\n\r\n" // enables TCP no delay
				+ message;
		
		push.Send(Encoding.ASCII.GetBytes(text));
		push.Receive(data);

		Debug.Log (message);
	}
	
	public string[] Receive() {
		lock(queue) {
			if(queue.Count > 0) {
				string[] messages = new string[queue.Count];
				
				for(int i = 0; i < messages.Length; i++) {
					messages[i] = queue.Dequeue();
				}
				
				return messages;
			}
		}
		
		return null;
	}
	
	private void Callback(IAsyncResult ar) {
		try {
			State state = (State) ar.AsyncState;
			int read = state.socket.EndReceive(ar);
			
			if(read > 0) {
				String text = Encoding.ASCII.GetString(state.data, 0, read);
				String[] split = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
				
				if(!split[0].StartsWith("HTTP")) {
					int length = split[0].Length + int.Parse(split[0]) + 4;
					
					if(length != read) {
						System.Console.WriteLine("AooB: " + read + "/" + length);
					}
					
					String[] messages = split[1].Split('\n');
					
					lock(queue) {
						for(int i = 0; i < messages.Length; i++) {
							if(messages[i].Length > 0) {
								queue.Enqueue(messages[i]);
							}
						}
					}
				}
				
				state.socket.BeginReceive(state.data, 0, State.size, 0, new AsyncCallback(Callback), state);
			}
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
	}
};