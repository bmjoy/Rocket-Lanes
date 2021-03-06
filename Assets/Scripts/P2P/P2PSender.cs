﻿using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class P2PSender
{
	public static void Send(int hostId, int connectionId, int channelId, MessageBase message, short messageType)
	{
		NetworkWriter writer = new NetworkWriter();
		writer.StartMessage(messageType);
		message.Serialize(writer);
		writer.FinishMessage();
		byte[] writerData = writer.ToArray();
		int bufferLength = P2PController.bufferLength;

		NetworkTransport.Send(hostId, connectionId, channelId, writerData, P2PController.bufferLength, out P2PController.error);
		P2PController.CheckError("Send");

		
		if(Recorder.session != null)
		{
			Recorder.session.messagesSent ++;
			Recorder.session.AddLeavingBandwidth(bufferLength);
			if(channelId == P2PChannels.ReliableChannelId)
				Recorder.session.importantMessagesSent ++;
		}
	}

	public static void SendToAll(int channelId, MessageBase message, short messageType)
	{
		foreach(P2PConnection connection in P2PConnectionManager.connections)
		{
			if(connection.ConnectionSuccessful())
				Send(connection.hostId, connection.connectionId, channelId, message, messageType);
		}
	}
}
