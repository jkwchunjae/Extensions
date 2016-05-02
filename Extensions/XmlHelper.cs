using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace Extensions
{
	public static class XmlHelper
	{
		public static bool HasElement(this XElement xElement, string elementName)
		{
			return xElement.Elements(elementName).Count() > 0;
		}

		public static bool HasAttribute(this XElement xElement, string attributeName)
		{
			return xElement.Attributes(attributeName).Count() > 0;
		}

		public static int ToInt(this XAttribute xAttribute, int defaultValue = 0)
		{
			if (xAttribute == null) return defaultValue;
			return xAttribute.Value.ToInt(defaultValue);
		}

		public static long ToLong(this XAttribute xAttribute, long defaultValue = 0L)
		{
			if (xAttribute == null) return defaultValue;
			return xAttribute.Value.ToLong(defaultValue);
		}

		public static double ToDouble(this XAttribute xAttribute, double defaultValue = 0.0)
		{
			if (xAttribute == null) return defaultValue;
			return xAttribute.Value.ToDouble(defaultValue);
		}

		public static bool ToBoolean(this XAttribute xAttribute, bool defaultValue = false)
		{
			if (xAttribute == null) return defaultValue;
			return xAttribute.Value.ToBoolean(defaultValue);
		}

		public static string TryGetValue(this XAttribute xAttribute, string defaultValue = "")
		{
			if (xAttribute == null) return defaultValue;
			return xAttribute.Value;
		}

        public static bool GetBool(this XmlNode xmlNode, string attributeName, bool defaultValue = false)
        {
            bool changedValue = defaultValue;
            Boolean.TryParse(xmlNode.Attributes[attributeName].Value.ToString(), out changedValue);
            return changedValue;
        }

        public static int GetInt(this XmlNode xmlNode, string attributeName, int defaultValue = 0)
        {
            int changedValue = defaultValue;
            Int32.TryParse(xmlNode.Attributes[attributeName].Value.ToString(), out changedValue);
            return changedValue;
        }

        public static int GetInt(this XElement xmlNode, string attributeName, int defalutValue = 0)
        {
            int changedValue = defalutValue;
            if (xmlNode.NodeType != XmlNodeType.Element)
                throw new ArgumentException("only can used for xml Node!");
            else if (xmlNode.Attribute(attributeName) == null)
                return changedValue;

            Int32.TryParse(xmlNode.Attribute(attributeName).Value, out changedValue);
            return changedValue;
        }

        public static bool GetBool(this XElement xmlNode, string attributeName, bool defaultValue = false)
        {
            bool changedValue = defaultValue;
            if (xmlNode.NodeType != XmlNodeType.Element)
                throw new ArgumentException("only can used for xml Node!");
            else if (xmlNode.Attribute(attributeName) == null)
                return changedValue;

            bool.TryParse(xmlNode.Attribute(attributeName).Value, out changedValue);
            return changedValue;
        }

        public static string GetString(this XElement xmlNode, string attributeName, string defaultValue = "")
        {
            string changedValue = defaultValue;
            if (xmlNode.NodeType != XmlNodeType.Element)
                throw new ArgumentException("only can used for xml Node!");
            else if (xmlNode.Attribute(attributeName) == null)
                return changedValue;

            return xmlNode.Attribute(attributeName).Value.ToString();
        }

        public static float GetFloat(this XElement xmlNode, string attributeName, float defaultValue = 0)
        {
            float changedValue = defaultValue;
            if (xmlNode.NodeType != XmlNodeType.Element)
                throw new ArgumentException("only can used for xml Node!");
            else if (xmlNode.Attribute(attributeName) == null)
                return changedValue;

            float.TryParse(xmlNode.Attribute(attributeName).Value.ToString(), out changedValue);
            return changedValue;
        }

        public static float GetLong(this XElement xmlNode, string attributeName, long defaultValue = 0)
        {
            long changedValue = defaultValue;
            if (xmlNode.NodeType != XmlNodeType.Element)
                throw new ArgumentException("only can used for xml Node!");
            else if (xmlNode.Attribute(attributeName) == null)
                return changedValue;

            long.TryParse(xmlNode.Attribute(attributeName).Value.ToString(), out changedValue);
            return changedValue;
        }
	}
}
