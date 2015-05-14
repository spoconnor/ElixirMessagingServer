using System;
using System.Xml;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sean.World
{
	internal abstract class GameObject
	{
		protected GameObject()
		{
			Id = WorldData.NextGameObjectId;
		}

		protected GameObject(ref Coords coords, int id = -1)
		{
			if (//!(this is Player) && 
                !coords.IsValidItemLocation) throw new Exception(string.Format("Invalid item location: {0}", coords));
			Id = WorldData.NextGameObjectId; //if this is a server we need to select our own IDs, ignore what the client said
			Coords = coords;
		}

		protected GameObject(XmlNode xmlNode)
		{
			if (xmlNode.Attributes == null) throw new Exception("Node attributes is null.");
			Id = int.Parse(xmlNode.Attributes["ID"].Value);
			if (Id >= WorldData.GameObjectIdSeq) System.Threading.Interlocked.Add(ref WorldData.GameObjectIdSeq, Id + 1); //ensure this loaded objects id will not conflict with the sequence
			Coords = new Coords(float.Parse(xmlNode.Attributes["X"].Value), float.Parse(xmlNode.Attributes["Y"].Value), float.Parse(xmlNode.Attributes["Z"].Value));
		}

		internal readonly int Id;
		public override int GetHashCode()
		{
			return Id;
		}

		/// <summary>Coords of the game object. Field instead of property so that individual components of the struct can be set directly.</summary>
		internal Coords Coords;

		/// <summary>Is this game object affected by light. Can be overridden with false for objects that are not affected.</summary>
		protected virtual bool IsAffectedByLight { get { return true; } }

		/// <summary>Statically track the currently set game object color so we only make the GL call to change color when needed (ie: the color will be 255 a large percent of the time for game objects in sunlight)</summary>
		//private static byte? _currentLightColor;

		/// <summary>Reset GL color and currently set game object color so it does not interfere with the next set of game objects rendered.</summary>
		
		internal abstract string XmlElementName { get; }
		internal virtual XmlNode GetXml(XmlDocument xmlDocument)
		{
			var xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, XmlElementName, string.Empty);
			if (xmlNode.Attributes == null) throw new Exception("Node attributes is null.");
			xmlNode.Attributes.Append(xmlDocument.CreateAttribute("ID")).Value = Id.ToString();
			xmlNode.Attributes.Append(xmlDocument.CreateAttribute("X")).Value = Coords.Xf.ToString("0.##"); //this format uses the smallest number of chars possible to represent the coords to a precision of 2
			xmlNode.Attributes.Append(xmlDocument.CreateAttribute("Y")).Value = Coords.Yf.ToString("0.##"); //this format uses the smallest number of chars possible to represent the coords to a precision of 2
			xmlNode.Attributes.Append(xmlDocument.CreateAttribute("Z")).Value = Coords.Zf.ToString("0.##"); //this format uses the smallest number of chars possible to represent the coords to a precision of 2
			return xmlNode;
		}
	}
}