using System;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace Sean.World
{
	// ReSharper disable PossibleNullReferenceException
	internal static class WorldSettings
	{
		#region Load
		/// <summary>Load world settings from a byte array in XML format that comes from the zipped world save file or sent from the server in multiplayer.</summary>
		/// <remarks>Remember XML xpaths are case sensitive.</remarks>
		/// <param name="settings">xml byte array</param>
		internal static void LoadSettings()
		{
			Debug.WriteLine("Loading world settings...");
			try
			{
                WorldData.RawSeed = "123456"; //settingsNode.Attributes["RawSeed"].Value;
                WorldData.GeneratorVersion = "1.0";// settingsNode.Attributes["GeneratorVersion"].Value;
                WorldData.GameObjectIdSeq = 1; //int.Parse(settingsNode.Attributes["GameObjectIdSeq"].Value);
                WorldData.WorldType = WorldType.Grass;// (WorldType)Convert.ToInt32(settingsNode.Attributes["WorldType"].Value);
                WorldData.SizeInChunksX = 32;// int.Parse(settingsNode.Attributes["SizeX"].Value);
				//world size Y is only there for future use, no need to load it
                WorldData.SizeInChunksZ = 32; //int.Parse(settingsNode.Attributes["SizeZ"].Value);

                WorldData.Chunks = new Chunks(WorldData.SizeInChunksX, WorldData.SizeInChunksZ); //initialize world chunks (must be done prior to assigning clutter to the chunks)
			}
			catch (Exception ex)
			{
				//todo: exceptions here make the client crash hard, may want to write to event log or find a way to handle nicer
				//-can test by just changing any of the select node xpaths to an incorrect one
				throw new Exception("Error loading world settings: " + ex.Message);
			}
		}
		#endregion

		#region Save
		/// <summary>Get world settings in an XML format and return as a byte array to be written in the zipped world save file or sent to clients in multiplayer.</summary>
		/// <remarks>Remember XML xpaths are case sensitive.</remarks>
		/// <returns>xml byte array</returns>
		internal static byte[] GetXmlByteArray()
		{
			try
			{
				var xml = new XmlDocument();
				xml.LoadXml("<?xml version=\"1.0\" ?>\n<World />");

				//settings
				var settingsNode = xml.DocumentElement.AppendChild(xml.CreateNode(XmlNodeType.Element, "Settings", ""));
				settingsNode.Attributes.Append(xml.CreateAttribute("RawSeed")).Value = WorldData.RawSeed;
				settingsNode.Attributes.Append(xml.CreateAttribute("GeneratorVersion")).Value = WorldData.GeneratorVersion;
				settingsNode.Attributes.Append(xml.CreateAttribute("GameObjectIdSeq")).Value = WorldData.GameObjectIdSeq.ToString();
				settingsNode.Attributes.Append(xml.CreateAttribute("WorldType")).Value = ((int)WorldData.WorldType).ToString();
				settingsNode.Attributes.Append(xml.CreateAttribute("SizeX")).Value = WorldData.SizeInChunksX.ToString();
				settingsNode.Attributes.Append(xml.CreateAttribute("SizeY")).Value = Chunk.CHUNK_HEIGHT.ToString(); //for possible future use
				settingsNode.Attributes.Append(xml.CreateAttribute("SizeZ")).Value = WorldData.SizeInChunksZ.ToString();

				//chunks / clutter / light sources
//				var chunksNode = xml.DocumentElement.AppendChild(xml.CreateNode(XmlNodeType.Element, "Chunks", ""));
//				foreach (Chunk chunk in WorldData.Chunks)
//				{
//					chunksNode.AppendChild(chunk.GetXml(xml));
//				}

				return Encoding.UTF8.GetBytes(xml.OuterXml);
			}
			catch (Exception ex)
			{
				//gm: exceptions caught here are handled nicely and end up in the windows forms messagebox for the client
				throw new Exception("Error saving world settings: " + ex.Message);
			}
		}
		#endregion
	}
	// ReSharper restore PossibleNullReferenceException
}
