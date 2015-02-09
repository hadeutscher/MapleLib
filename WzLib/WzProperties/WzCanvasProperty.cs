/*  MapleLib - A general-purpose MapleStory library
 * Copyright (C) 2009, 2010, 2015 Snow and haha01haha01
   
 * This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.*/

using System.Collections.Generic;
using System.IO;
using MapleLib.WzLib.Util;
using System;

namespace MapleLib.WzLib.WzProperties
{
	/// <summary>
	/// A property that can contain sub properties and has one png image
	/// </summary>
    public class WzCanvasProperty : IExtended, IPropertyContainer
	{
		#region Fields
		internal List<IWzImageProperty> properties = new List<IWzImageProperty>();
		internal WzPngProperty imageProp;
		internal string name;
		internal IWzObject parent;
		//internal WzImage imgParent;
		#endregion

		#region Inherited Members
        public override void SetValue(object value)
        {
            imageProp.SetValue(value);
        }

        public override IWzImageProperty DeepClone()
        {
            WzCanvasProperty clone = (WzCanvasProperty)MemberwiseClone();
            clone.properties = new List<IWzImageProperty>();
            foreach (IWzImageProperty prop in properties)
                clone.properties.Add(prop.DeepClone());
            clone.imageProp = (WzPngProperty)imageProp.DeepClone();
            return clone;
        }

		public override object WzValue { get { return PngProperty; } }
		/// <summary>
		/// The parent of the object
		/// </summary>
		public override IWzObject Parent { get { return parent; } internal set { parent = value; } }
		/// <summary>
		/// The WzPropertyType of the property
		/// </summary>
		public override WzPropertyType PropertyType { get { return WzPropertyType.Canvas; } }
		/// <summary>
		/// The properties contained in this property
		/// </summary>
		public override List<IWzImageProperty> WzProperties
		{
			get
			{
				return properties;
			}
		}
		/// <summary>
		/// The name of the property
		/// </summary>
		public override string Name { get { return name; } set { name = value; } }
		/// <summary>
		/// Gets a wz property by it's name
		/// </summary>
		/// <param name="name">The name of the property</param>
		/// <returns>The wz property with the specified name</returns>
		public override IWzImageProperty this[string name]
		{
			get
			{
				if (name == "PNG")
					return imageProp;
				foreach (IWzImageProperty iwp in properties)
					if (iwp.Name.ToLower() == name.ToLower())
                        return iwp;
				return null;
			}
		}

        public IWzImageProperty GetProperty(string name)
        {
            foreach (IWzImageProperty iwp in properties)
                if (iwp.Name.ToLower() == name.ToLower())
                    return iwp;
            return null;
        }

		/// Gets a wz property by a path name
		/// </summary>
		/// <param name="path">path to property</param>
		/// <returns>the wz property with the specified name</returns>
		public override IWzImageProperty GetFromPath(string path)
		{
			string[] segments = path.Split(new char[1] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);
			if (segments[0] == "..")
			{
				return ((IWzImageProperty)Parent)[path.Substring(name.IndexOf('/') + 1)];
			}
			IWzImageProperty ret = this;
			for (int x = 0; x < segments.Length; x++)
			{
				bool foundChild = false;
				if (segments[x] == "PNG")
				{
					return imageProp;
				}
				foreach (IWzImageProperty iwp in ret.WzProperties)
				{
					if (iwp.Name == segments[x])
					{
						ret = iwp;
						foundChild = true;
						break;
					}
				}
				if (!foundChild)
				{
					return null;
				}
			}
			return ret;
		}
		public override void WriteValue(MapleLib.WzLib.Util.WzBinaryWriter writer)
		{
			writer.WriteStringValue("Canvas", 0x73, 0x1B);
			writer.Write((byte)0);
			if (properties.Count > 0)
			{
				writer.Write((byte)1);
				IWzImageProperty.WritePropertyList(writer, properties);
			}
			else
			{
				writer.Write((byte)0);
			}
			writer.WriteCompressedInt(PngProperty.Width);
			writer.WriteCompressedInt(PngProperty.Height);
			writer.WriteCompressedInt(PngProperty.format);
			writer.Write((byte)PngProperty.format2);
			writer.Write((Int32)0);
            byte[] bytes = PngProperty.GetCompressedBytes(false);
            writer.Write(bytes.Length + 1);
			writer.Write((byte)0);
            writer.Write(bytes);
		}
		public override void ExportXml(StreamWriter writer, int level)
		{
			writer.WriteLine(XmlUtil.Indentation(level) + XmlUtil.OpenNamedTag("WzCanvas", this.Name, false, false) +
			XmlUtil.Attrib("width", PngProperty.Width.ToString()) +
			XmlUtil.Attrib("height", PngProperty.Height.ToString(), true, false));
			IWzImageProperty.DumpPropertyList(writer, level, this.WzProperties);
			writer.WriteLine(XmlUtil.Indentation(level) + XmlUtil.CloseTag("WzCanvas"));
		}
		/// <summary>
		/// Dispose the object
		/// </summary>
		public override void Dispose()
		{
			name = null;
			imageProp.Dispose();
			imageProp = null;
			foreach (IWzImageProperty prop in properties)
			{
				prop.Dispose();
			}
			properties.Clear();
			properties = null;
		}
		#endregion

		#region Custom Members
		/// <summary>
		/// The png image for this canvas property
		/// </summary>
		public WzPngProperty PngProperty { get { return imageProp; } set { imageProp = value; } }
		/// <summary>
		/// Creates a blank WzCanvasProperty
		/// </summary>
		public WzCanvasProperty() { }
		/// <summary>
		/// Creates a WzCanvasProperty with the specified name
		/// </summary>
		/// <param name="name">The name of the property</param>
		public WzCanvasProperty(string name)
		{
			this.name = name;
		}
		/// <summary>
		/// Adds a property to the property list of this property
		/// </summary>
		/// <param name="prop">The property to add</param>
		public void AddProperty(IWzImageProperty prop)
		{
            prop.Parent = this;
            properties.Add(prop);
		}
		public void AddProperties(List<IWzImageProperty> props)
		{
			foreach (IWzImageProperty prop in props)
			{
				AddProperty(prop);
			}
		}
		/// <summary>
		/// Remove a property
		/// </summary>
		/// <param name="name">Name of Property</param>
        public void RemoveProperty(IWzImageProperty prop)
		{
            prop.Parent = null;
			properties.Remove(prop);
		}

		/// <summary>
		/// Clears the list of properties
		/// </summary>
		public void ClearProperties()
        {
            foreach (IWzImageProperty prop in properties) prop.Parent = null;
			properties.Clear();
		}
		#endregion

        #region Cast Values
        internal override WzPngProperty ToPngProperty(WzPngProperty def)
        {
            return imageProp;
        }

        internal override System.Drawing.Bitmap ToBitmap(System.Drawing.Bitmap def)
        {
            return imageProp.GetPNG(false);
        }
        #endregion
	}
}