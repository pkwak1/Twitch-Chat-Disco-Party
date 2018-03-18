using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;

public class TwitchChat : MonoBehaviour {

	private TcpClient twitchClient;
	private StreamReader reader;
	private StreamWriter writer;

	public string username, password, channelName; // Get password from https:/twitchapps.com/tmi
	public Text chatBox;
	public Rigidbody player;
	public int speed;
	// Use this for initialization
	void Start () {
		Connect();
	}

	// Update is called once per frame
	void Update () {
		// reconnect if not connected
		if(!twitchClient.Connected){
			Connect();
		}
		ReadChat();
		// WriteChat();
	}

	// connect to twitch client
	private void Connect() {
		twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
		reader = new StreamReader(twitchClient.GetStream());
		writer = new StreamWriter(twitchClient.GetStream());

		writer.WriteLine("PASS " + password);
		writer.WriteLine("NICK " + username);
		writer.WriteLine("USER " + username + "8 * :" +username);
		writer.WriteLine("JOIN #" + channelName);
		writer.Flush();
	}

	private void ReadChat(){
		if(twitchClient.Available > 0) {
			var message = reader.ReadLine();
			//print(message);
			// if Twitch chat message
			if(message.Contains("PRIVMSG")) {
				// Get the users name by splitting it from the string
				var splitPoint = message.IndexOf("!", 1);
				var chatName = message.Substring(0, splitPoint);
				chatName = chatName.Substring(1);

				// Get users msg by splitting it from string
				splitPoint = message.IndexOf(":", 1);
				message = message.Substring(splitPoint + 1);
				//print(String.Format("{0}: {1}", chatName, message));
				chatBox.text = chatBox.text + "\n" + String.Format("{0}: {1}", chatName, message);

				// Run the chat's commands
				GameInputs(message);
			}

		}
	}

	private void GameInputs(string ChatInputs) {
		if(ChatInputs.ToLower() == "left"){
			player.AddForce(Vector3.left * (speed * 1000));
		}
		if(ChatInputs.ToLower() == "right"){
			player.AddForce(Vector3.right * (speed * 1000));
		}
		if(ChatInputs.ToLower() == "forward"){
			player.AddForce(Vector3.forward * (speed * 1000));
		}
	}

	// private void WriteChat(){
	// 	if(twitchClient.Available > 0) {
	// 		writer.WriteLine("PASS " + password);
	// 		writer.WriteLine("NICK " + username);
	// 		writer.WriteLine("USER " + username + "8 * :" +username);
	// 		writer.WriteLine("JOIN #" + channelName);
	// 		writer.WriteLine("PRIVMSG #" + channelName + ": KAPPA");
	// 		writer.Flush();
	// 	}
	// }
}
