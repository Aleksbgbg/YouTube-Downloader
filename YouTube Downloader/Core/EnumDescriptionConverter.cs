namespace YouTube.Downloader.Core
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;

    internal class EnumDescriptionConverter : EnumConverter
    {
        private readonly Type _enumType;

        public EnumDescriptionConverter(Type type) : base(type)
        {
            _enumType = type;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(_enumType.GetField(Enum.GetName(_enumType, value)), typeof(DescriptionAttribute));

            return descriptionAttribute == null ? value.ToString() : descriptionAttribute.Description;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            foreach (FieldInfo fieldInfo in _enumType.GetFields())
            {
                DescriptionAttribute descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));

                if (descriptionAttribute != null && (string)value == descriptionAttribute.Description)
                {
                    return Enum.Parse(_enumType, fieldInfo.Name);
                }
            }

            return Enum.Parse(_enumType, (string)value);
        }
    }
}