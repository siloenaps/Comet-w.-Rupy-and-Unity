//using UnityEngine; // policy ###
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Text;

/* A real-time comet stream plugin for rupy.
 * For unity seach for ###
 * For usage scroll down to main() method.
 */
public class Stream {
	public string host = "fuse.rupy.se";
	public int port = 80;
	
	private Queue<string> queue;
	private Socket pull, push;
	private bool connected;
	
	private class State {
		public Socket socket = null;
		public const int size = 32768;
		public byte[] data = new byte[size];
	}
	
	public Stream() {
		bool policy = true;
		
		//policy = Security.PrefetchSocketPolicy(host, port); // policy ###
		
		if(!policy)
			throw new Exception("Policy (" + host + ":" + port + ") failed.");
		
		IPAddress address = Dns.Resolve(host).AddressList[0];
		IPEndPoint remote = new IPEndPoint(address, port);
		
		//Console.WriteLine("Address: " + address + ".");
		
		push = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		push.NoDelay = true;
		push.Connect(remote);
	}
	
	public void Connect(string name) {
		queue = new Queue<string>();
		
		IPAddress address = Dns.Resolve(host).AddressList[0];
		IPEndPoint remote = new IPEndPoint(address, port);
		
		pull = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		pull.NoDelay = true;
		pull.Connect(remote);
		
		String data = "GET /pull?name=" + name + " HTTP/1.1\r\n"
			+ "Host: " + host + "\r\n"
				+ "Head: less\r\n\r\n"; // enables TCP no delay
		
		pull.Send(Encoding.ASCII.GetBytes(data));
		
		State state = new State();
		state.socket = pull;
		
		pull.BeginReceive(state.data, 0, State.size, 0, new AsyncCallback(Callback), state);
		
		connected = true;
	}
	
	public string Send(String name, String message) {
		byte[] data = new byte[1024];
		String text = "POST /push HTTP/1.1\r\n"
			+ "Host: " + host + "\r\n"
				+ "Head: less\r\n\r\n" // enables TCP no delay
				+ "name=" + name + "&message=" + message;
		
		push.Send(Encoding.ASCII.GetBytes(text));
		int read = push.Receive(data);
		text = Encoding.ASCII.GetString(data, 0, read);
		
		//Console.WriteLine("Read: " + read + ".");
		//Console.WriteLine("Text: " + text + ".");
		
		string[] split = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
		return split[2];
	}
	
	public string[] Receive() {
		if(!connected)
			return null;
		
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
				string text = Encoding.ASCII.GetString(state.data, 0, read);
				string[] split = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
				
				if(!split[0].StartsWith("HTTP")) {
					string[] messages = split[1].Split('\n');
					
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
}