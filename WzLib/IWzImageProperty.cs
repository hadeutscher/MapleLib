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

using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using MapleLib.WzLib.Util;
using MapleLib.WzLib.WzProperties;

namespace MapleLib.WzLib
{
	/// <summary>
	/// An interface for wz img properties
	/// </summary>
	public abstract class IWzImageProperty : IWzObject
    {
        #region Virtual\Abstrcat Members
        public virtual List<IWzImageProperty> WzProperties { get { return null; } }

        public virtual IWzImageProperty this[string name] { get { return null; } set { throw new NotImplementedException(); } }

        public virtual IWzImageProperty GetFromPath(string path)
        {
            return null;
        }

		public abstract WzPropertyType PropertyType { get; }
        
        /// <summary>
        /// The image that this property is contained in
        /// </summary>
		public WzImage ParentImage 
        {
            get
            {
                IWzObject parent = Parent;
                while (parent != null)
                {
                    if (parent is WzImage) return (WzImage)parent;
                    else parent = parent.Parent;
                }
                return null;
            }
        }

		public override WzObjectType ObjectType { get { return WzObjectType.Property; } }

		public abstract void WriteValue(WzBinaryWriter writer);

        public abstract IWzImageProperty DeepClone();

        public abstract void SetValue(object value);

        public override void Remove()
        {
            ((IPropertyContainer)Parent).RemoveProperty(this);
        }

		public virtual void ExportXml(StreamWriter writer, int level)
		{
			writer.WriteLine(XmlUtil.Indentation(level) + XmlUtil.OpenNamedTag(this.PropertyType.ToString(), this.Name, true));
			writer.WriteLine(XmlUtil.Indentation(level) + XmlUtil.CloseTag(this.PropertyType.ToString()));
        }

        public override /*I*/WzFile WzFileParent
        {
            get { return ParentImage.WzFileParent; }
        }
        #endregion

        #region Extended Properties Parsing
        internal static void WritePropertyList(WzBinaryWriter writer, List<IWzImageProperty> properties)
		{
			writer.Write((ushort)0);
			writer.WriteCompressedInt(properties.Count);
            for (int i = 0; i < properties.Count; i++)
			{
				writer.WriteStringValue(properties[i].Name, 0x00, 0x01);
                if (properties[i] is IExtended)
                    WriteExtendedValue(writer, (IExtended)properties[i]);
                else
                    properties[i].WriteValue(writer);
			}
		}

		internal static void DumpPropertyList(StreamWriter writer, int level, List<IWzImageProperty> properties)
		{
			foreach (IWzImageProperty prop in properties)
			{
				prop.ExportXml(writer, level + 1);
			}
		}

		internal static List<IWzImageProperty> ParsePropertyList(uint offset, WzBinaryReader reader, IWzObject parent, WzImage parentImg)
		{
			int entryCount = reader.ReadCompressedInt();
            List<IWzImageProperty> properties = new List<IWzImageProperty>(entryCount);
			for (int i = 0; i < entryCount; i++)
			{
				string name = reader.ReadStringBlock(offset);
				switch (reader.ReadByte())
				{
					case 0:
						properties.Add(new WzNullProperty(name) { Parent = parent/*, ParentImage = parentImg*/ });
						break;
					case 0x0B:
					case 2:
						properties.Add(new WzUnsignedShortProperty(name, reader.ReadUInt16()) { Parent = parent/*, ParentImage = parentImg*/ });
						break;
					case 3:
						properties.Add(new WzCompressedIntProperty(name, reader.ReadCompressedInt()) { Parent = parent/*, ParentImage = parentImg*/ });
						break;
					case 4:
						byte type = reader.ReadByte();
						if (type == 0x80)
							properties.Add(new WzByteFloatProperty(name, reader.ReadSingle()) { Parent = parent/*, ParentImage = parentImg*/ });
						else if (type == 0)
							properties.Add(new WzByteFloatProperty(name, 0f) { Parent = parent/*, ParentImage = parentImg*/ });
						break;
					case 5:
						properties.Add(new WzDoubleProperty(name, reader.ReadDouble()) { Parent = parent/*, ParentImage = parentImg*/ });
						break;
					case 8:
						properties.Add(new WzStringProperty(name, reader.ReadStringBlock(offset)) { Parent = parent });
						break;
					case 9:
						int eob = (int)(reader.ReadUInt32() + reader.BaseStream.Position);
                        IWzImageProperty exProp = ParseExtendedProp(reader, offset, eob, name, parent, parentImg);
						properties.Add(exProp);
						if (reader.BaseStream.Position != eob) reader.BaseStream.Position = eob;
						break;
                    default:
                        throw new Exception("Unknown property type at ParsePropertyList");
				}
			}
			return properties;
		}

        internal static IExtended ParseExtendedProp(WzBinaryReader reader, uint offset, int endOfBlock, string name, IWzObject parent, WzImage imgParent)
        {
            switch (reader.ReadByte())
            {
                case 0x1B:
                    return ExtractMore(reader, offset, endOfBlock, name, reader.ReadStringAtOffset(offset + reader.ReadInt32()), parent, imgParent);
                case 0x73:
                    return ExtractMore(reader, offset, endOfBlock, name, "", parent, imgParent);
                default:
                    throw new System.Exception("Invlid byte read at ParseExtendedProp");
            }
        }

        internal static IExtended ExtractMore(WzBinaryReader reader, uint offset, int eob, string name, string iname, IWzObject parent, WzImage imgParent)
        {
            if (iname == "")
                iname = reader.ReadString();
            switch (iname)
            {
                case "Property":
                    WzSubProperty subProp = new WzSubProperty(name) { Parent = parent };
                    reader.BaseStream.Position += 2;
                    subProp.AddProperties(IWzImageProperty.ParsePropertyList(offset, reader, subProp, imgParent));
                    return subProp;
                case "Canvas":
                    WzCanvasProperty canvasProp = new WzCanvasProperty(name) { Parent = parent };
                    reader.BaseStream.Position++;
                    if (reader.ReadByte() == 1)
                    {
                        reader.BaseStream.Position += 2;
                        canvasProp.AddProperties(IWzImageProperty.ParsePropertyList(offset, reader, canvasProp, imgParent));
                    }
                    canvasProp.PngProperty = new WzPngProperty(reader, imgParent.parseEverything) { Parent = canvasProp };
                    return canvasProp;
                case "Shape2D#Vector2D":
                    WzVectorProperty vecProp = new WzVectorProperty(name) { Parent = parent };
                    vecProp.X = new WzCompressedIntProperty("X", reader.ReadCompressedInt()) { Parent = vecProp };
                    vecProp.Y = new WzCompressedIntProperty("Y", reader.ReadCompressedInt()) { Parent = vecProp };
                    return vecProp;
                case "Shape2D#Convex2D":
                    WzConvexProperty convexProp = new WzConvexProperty(name) { Parent = parent };
                    int convexEntryCount = reader.ReadCompressedInt();
                    convexProp.WzProperties.Capacity = convexEntryCount; //performance thing
                    for (int i = 0; i < convexEntryCount; i++)
                    {
                        convexProp.AddProperty(ParseExtendedProp(reader, offset, 0, name, convexProp, imgParent));
                    }
                    return convexProp;
                case "Sound_DX8":
                    WzSoundProperty soundProp = new WzSoundProperty(name, reader, imgParent.parseEverything) { Parent = parent };
                    return soundProp;
                case "UOL":
                    reader.BaseStream.Position++;
                    switch (reader.ReadByte())
                    {
                        case 0:
                            return new WzUOLProperty(name, reader.ReadString()) { Parent = parent };
                        case 1:
                            return new WzUOLProperty(name, reader.ReadStringAtOffset(offset + reader.ReadInt32())) { Parent = parent };
                    }
                    throw new Exception("Unsupported UOL type");
                default:
                    throw new Exception("Unknown iname: " + iname);
            }
        }

        internal static void WriteExtendedValue(WzBinaryWriter writer, IExtended property)
        {
            writer.Write((byte)9);
            long beforePos = writer.BaseStream.Position;
            writer.Write((Int32)0); // Placeholder
            property.WriteValue(writer);
            int len = (int)(writer.BaseStream.Position - beforePos);
            long newPos = writer.BaseStream.Position;
            writer.BaseStream.Position = beforePos;
            writer.Write(len - 4);
            writer.BaseStream.Position = newPos;
        }
        #endregion

	}
}