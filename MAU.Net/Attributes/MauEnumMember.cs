using System;
using System.Linq;
using System.Reflection;

namespace MAU.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class MauEnumMember : Attribute
	{
		public enum EnumParser
		{
			String,
			Number
		}

		public string ValueAsStr { get; }
		public long ValueAsNum { get; }
		public EnumParser Parser { get; }

		public MauEnumMember(string value)
		{
			ValueAsStr = value;
			Parser = EnumParser.String;
		}
		public MauEnumMember(long value)
		{
			ValueAsNum = value;
			Parser = EnumParser.Number;
		}

		public static bool HasAttribute(PropertyInfo propertyInfo)
		{
			return propertyInfo.GetCustomAttributes<MauEnumMember>(false).Any();
		}
		public static bool HasAttribute(FieldInfo fieldInfo)
		{
			return fieldInfo.GetCustomAttributes<MauEnumMember>(false).Any();
		}
		public static bool HasAttribute(Enum enumValue)
		{
			Type enumType = enumValue.GetType();
			string name = Enum.GetName(enumType, enumValue);
			return enumType.GetField(name).GetCustomAttributes<MauEnumMember>(false).Any();
		}
		public static bool HasNotSetValue(Type enumType)
		{
			// NotSet must to be in any MauProperty value is `Enum`
			// And also must to be first value in Enum
			// For sure will equal == 0
			return Enum.GetNames(enumType)[0] == "NotSet";
		}

		public object GetValue()
		{
			return Parser switch
			{
				EnumParser.Number => ValueAsNum,
				EnumParser.String => ValueAsStr,
				_ => null
			};
		}
		public static object GetValue(Enum enumValue)
		{
			Type enumType = enumValue.GetType();
			string name = Enum.GetName(enumType, enumValue);
			MauEnumMember instance = enumType.GetField(name).GetCustomAttributes<MauEnumMember>(false).FirstOrDefault();
			if (instance == null)
				return null;

			return instance.Parser == EnumParser.Number ? (object)instance.ValueAsNum : (object)instance.ValueAsStr;
		}
	}
}
