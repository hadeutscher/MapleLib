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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
//using HaCreator.MapEditor;
using MapleLib.WzLib.WzStructure;

namespace MapleLib.WzLib.WzStructure
{
    public static class InfoTool
    {
        public static string GetString(IWzImageProperty source)
        {
            return ((WzStringProperty)source).Value;
        }

        public static WzStringProperty SetString(string value)
        {
            return new WzStringProperty("", value);
        }

        public static string GetOptionalString(IWzImageProperty source)
        {
            return source == null ? null : ((WzStringProperty)source).Value;
        }

        public static WzStringProperty SetOptionalString(string value)
        {
            return value == null ? null : SetString(value);
        }

        public static double GetDouble(IWzImageProperty source)
        {
            return ((WzDoubleProperty)source).Value;
        }

        public static WzDoubleProperty SetDouble(double value)
        {
            return new WzDoubleProperty("", value);
        }

        public static int GetInt(IWzImageProperty source)
        {
            return ((WzCompressedIntProperty)source).Value;
        }

        public static WzCompressedIntProperty SetInt(int value)
        {
            return new WzCompressedIntProperty("", value);
        }

        public static int? GetOptionalInt(IWzImageProperty source)
        {
            return source == null ? (int?)null : ((WzCompressedIntProperty)source).Value;
        }

        public static WzCompressedIntProperty SetOptionalInt(int? value)
        {
            return value.HasValue ? SetInt(value.Value) : null;
        }

        public static bool GetBool(IWzImageProperty source)
        {
            return ((WzCompressedIntProperty)source).Value == 1;
        }

        public static WzCompressedIntProperty SetBool(bool value)
        {
            return new WzCompressedIntProperty("", value ? 1 : 0);
        }

        public static MapleBool GetOptionalBool(IWzImageProperty source)
        {
            if (source == null) return MapleBool.NotExist;
            else return ((WzCompressedIntProperty)source).Value == 1;
        }

        public static WzCompressedIntProperty SetOptionalBool(MapleBool value)
        {
            return value.HasValue ? SetBool(value.Value) : null;
        }

        public static float GetFloat(IWzImageProperty source)
        {
            return ((WzByteFloatProperty)source).Value;
        }

        public static WzByteFloatProperty SetFloat(float value)
        {
            return new WzByteFloatProperty("", value);
        }

        public static float? GetOptionalFloat(IWzImageProperty source)
        {
            return source == null ? (float?)null : ((WzByteFloatProperty)source).Value;
        }

        public static WzByteFloatProperty SetOptionalFloat(float? value)
        {
            return value.HasValue ? SetFloat(value.Value) : null;
        }

        public static int? GetOptionalTranslatedInt(IWzImageProperty source)
        {
            string str = InfoTool.GetOptionalString(source);
            if (str == null) return null;
            return int.Parse(str);
        }

        public static WzStringProperty SetOptionalTranslatedInt(int? value)
        {
            if (value.HasValue)
            {
                return SetString(value.Value.ToString());
            }
            else
            {
                return null;
            }
        }
    }
}
