using System;
using System.Collections.Generic;
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
			string[] names = Enum.GetNames(enumType);
			return names.Contains("NotSet");
		}
		public static bool GetValidEnumValue(Type valueType, ref object originalValue)
		{
			if (!valueType.IsEnum)
				return false;

			if (!MauEnumMember.HasNotSetValue(valueType))
				throw new Exception($"'MauEnumMember.NotSet' must to be in any 'Enum' used as 'MauProperty' type or return of 'MauMethod'. {valueType.FullName}");

			object o = originalValue;
			string enumValName = valueType.GetFields()
				.Where(MauEnumMember.HasAttribute)
				.FirstOrDefault(f =>
				{
					MauEnumMember f1 = f.GetCustomAttributes<MauEnumMember>(false)
						.FirstOrDefault(memEnum => memEnum.IsEqual(o));

					return f1 != null && f1.IsEqual(o);
				})?.Name;

			originalValue = Enum.Parse(valueType, string.IsNullOrEmpty(enumValName) ? "NotSet" : enumValName);
			return true;
		}

		public bool IsEqual(object cmpValue)
		{
			// Pattern matching
			return GetValue() switch
			{
				string cmpAsStr => (string)cmpValue == cmpAsStr,
				long cmpAsLong => Convert.ToInt64(cmpValue) == cmpAsLong,
				_ => false
			};
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

			return instance.Parser == EnumParser.Number
				? (object)instance.ValueAsNum
				: (object)instance.ValueAsStr;
		}
	}
}
