using System;
using System.Globalization;
using System.Xml.Linq;

namespace LH.Configuration
{
    internal static class XValueHelper
    {
        internal static object GetDictionarySectionValue(XElement content)
        {
            string type;
            if (content.Attribute("type") is null)
            {
                type = "System.String";
            }
            else
            {
                type = content.Attribute("type").Value;
                if (string.IsNullOrWhiteSpace(type))
                {
                    type = "System.String";
                }
            }
            string value = content.Attribute("value").Value;
            switch (type)
            {
                case "System.Boolean": return bool.Parse(value);
                case "System.Char": return char.Parse(value);
                case "System.SByte": return sbyte.Parse(value, CultureInfo.InvariantCulture);
                case "System.Byte": return byte.Parse(value, CultureInfo.InvariantCulture);
                case "System.Int16": return short.Parse(value, CultureInfo.InvariantCulture);
                case "System.UInt16": return ushort.Parse(value, CultureInfo.InvariantCulture);
                case "System.Int32": return int.Parse(value, CultureInfo.InvariantCulture);
                case "System.UInt32": return uint.Parse(value, CultureInfo.InvariantCulture);
                case "System.Int64": return long.Parse(value, CultureInfo.InvariantCulture);
                case "System.UInt64": return ulong.Parse(value, CultureInfo.InvariantCulture);
                case "System.Single": return float.Parse(value, CultureInfo.InvariantCulture);
                case "System.Double": return double.Parse(value, CultureInfo.InvariantCulture);
                case "System.Decimal": return decimal.Parse(value, CultureInfo.InvariantCulture);
                case "System.String": return value;
                case "System.Byte[]": return GetHexBytes(value);
                default: throw new InvalidCastException();
            }
        }

        internal static void SetDictionarySectionValue(object value, XElement content)
        {
            switch (value)
            {
                case bool _:
                case sbyte _:
                case byte _:
                case short _:
                case ushort _:
                case int _:
                case uint _:
                case long _:
                case ulong _:
                case float _:
                case double _:
                case decimal _:
                case char _:
                case string _:
                    content.SetAttributeValue("value", value.ToString());
                    break;

                case byte[] val:
                    content.SetAttributeValue("value", BitConverter.ToString(val).Replace("-", string.Empty));
                    break;

                default: throw new NotSupportedException();
            }
            content.SetAttributeValue("type", value.GetType().FullName);
        }

        private static byte[] GetHexBytes(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                throw new ArgumentNullException(nameof(hex));
            }
            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return result;
        }
    }
}