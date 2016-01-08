using System;
using System.Text;

namespace Sean.World
{
    public static class MessageParser
    {
        public static CommsMessages.Message ReadMessage (byte[] data)
        {
            byte[] msgBuffer = new byte[data [0]];
            Array.Copy (data, 1, msgBuffer, 0, data [0]); // Skip length byte

            {
                var builder = new StringBuilder ();
                for (int i=0; i<data[0]; i++) {
                    builder.Append (msgBuffer [i].ToString ());
                    builder.Append (",");
                }
                Console.WriteLine ("{0}", builder.ToString());
            }
            var recv = CommsMessages.Message.ParseFrom(msgBuffer);
            var msgType = (CommsMessages.MsgType)recv.Msgtype;
            Console.WriteLine ("Msg Type: {0}", msgType);
            switch (msgType) {
                case CommsMessages.MsgType.eLogin:
                Console.WriteLine ("Login Message Received");
                break;
                case CommsMessages.MsgType.eMap:
                Console.WriteLine ("Map Message Received");
                break;
                case CommsMessages.MsgType.eMapCharacterUpdate:
                Console.WriteLine ("Map Character Update Message Received");
                break;
                case CommsMessages.MsgType.eMapIgnore:
                Console.WriteLine ("Map Ignore Message Received");
                break;
                case CommsMessages.MsgType.eMapRequest:
                Console.WriteLine ("Map Request Message Received");
                break;
                case CommsMessages.MsgType.eMapUpdate:
                Console.WriteLine ("Map Update Message Received");
                break;
                case CommsMessages.MsgType.eNewUser:
                Console.WriteLine ("New User Message Received");
                break;
                case CommsMessages.MsgType.ePing:
                Console.WriteLine ("Ping Message Received");
                break;
                case CommsMessages.MsgType.ePong:
                Console.WriteLine ("Pong Message Received");
                break;
                case CommsMessages.MsgType.eQueryServer:
                Console.WriteLine ("Query Server Message Received");
                break;
                case CommsMessages.MsgType.eQueryServerResponse:
                Console.WriteLine ("Query Server Response Message Received");
                break;
                case CommsMessages.MsgType.eResponse:
                Console.WriteLine ("Response Message Received");
                break;
                case CommsMessages.MsgType.eSay:
                Console.WriteLine ("Say Message Received: {0}", recv.Say.Text);
                break;
                default :
                Console.WriteLine ("Warning! Unknown Message Received");
                break;
            }
            return recv;
        }

        public static byte[] WriteMessage (CommsMessages.Message message)
        {
            using (var memoryStream = new System.IO.MemoryStream()) {
                memoryStream.WriteByte (0); // reserve for length
                message.WriteTo (memoryStream);
                var messageBytes = memoryStream.ToArray ();
                messageBytes [0] = (byte)(messageBytes.Length - 1); // ignore nul at end
                return messageBytes;
            }
        }
    }
}

