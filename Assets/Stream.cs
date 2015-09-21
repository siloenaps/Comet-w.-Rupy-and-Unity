//using UnityEngine; // policy
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

/* A real-time comet stream plugin for rupy.
 * For unity policy check, uncomment lines with: // policy
 * For usage scroll down to main() method.
 */
using UnityEngine;
using System.Collections.Generic;

public class Stream {
    public String host = "talk.rupy.se";
    public int port = 80;

    private Socket pull, push;

    public class State {
        public Socket socket = null;
        public const int size = 32768;
        public byte[] data = new byte[size];
    }

	public Buffer buffer;
	public class Buffer {
		public List<string> queue;
		public Buffer(){
			queue = new List<string>();
		}
		public void Add(string msg){
			try{
				queue.Add (msg);
			} catch (Exception e) {  
				Debug.LogException(e);
			}
		}
		public string Get(){
			string msg = null;
			if (queue.Count > 0) {
				try{
					msg = queue[0];
					queue.RemoveAt(0);
				} catch (Exception e) {  
					Debug.LogException(e);
				}
			}
			return msg;
		}
	}
	
	public Stream() {
        bool policy = true;

		buffer = new Buffer();


        //policy = Security.PrefetchSocketPolicy(host, port); // policy
        if(!policy)
            throw new Exception("Policy (" + host + ":" + port + ") Failed");

        IPAddress address = Dns.Resolve(host).AddressList[0];
        IPEndPoint remote = new IPEndPoint(address, port);

        push = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        pull = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        push.Connect(remote);
        pull.Connect(remote);

        String data = "GET /pull?name=one HTTP/1.1\r\n"
                    + "Host: " + host + "\r\n"
                    + "Head: less\r\n\r\n"; // enables TCP no delay

        int sent = pull.Send(Encoding.ASCII.GetBytes(data));

        State state = new State();
        state.socket = pull;

        pull.BeginReceive(state.data, 0, State.size, 0, new AsyncCallback(callback), state);
    }

    /*
     * Not thread safe, only call from one thread.
     */
    public void send(String message) {
        byte[] data = new byte[64];
        String text = "POST /push HTTP/1.1\r\n"
                    + "Host: " + host + "\r\n"
                    + "Head: less\r\n\r\n" // enables TCP no delay
                    + message;

        int sent = push.Send(Encoding.ASCII.GetBytes(text));
        int read = push.Receive(data);
    }

    private void callback(IAsyncResult ar) {
		try {
            State state = (State) ar.AsyncState;
            int read = state.socket.EndReceive(ar);

            if(read > 0) {
                String text = Encoding.ASCII.GetString(state.data, 0, read);
                String[] split = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                if(!split[0].StartsWith("HTTP")) {
                    int length = split[0].Length + int.Parse(split[0]) + 4;

                    if(length != read) {
						Debug.Log("AooB: " + read + "/" + length);
                    }

                    String[] messages = split[1].Split('\n');

                    for(int i = 0; i < messages.Length; i++) {
                        if(messages[i].Length > 0) {
//                            this.async.Receive(messages[i]);
							buffer.Add(messages[i]);
                        }
                    }
                }

                state.socket.BeginReceive(state.data, 0, State.size, 0, new AsyncCallback(callback), state);
            }
        } catch (Exception e) {            
			Debug.LogException(e);
        }
    }

	public void Close(){
		// Release the socket.
		push.Shutdown(SocketShutdown.Both);
		push.Close();
		pull.Shutdown(SocketShutdown.Both);
		pull.Close();
	}
}