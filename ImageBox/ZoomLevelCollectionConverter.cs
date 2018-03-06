using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ImageGlass
{
    public class ZoomLevelCollectionConverter: TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string data;
            //ZoomLevelCollection result;
            ImageGlass.ImageBoxZoomLevelCollection result;

            data = value as string;
            if (!string.IsNullOrEmpty(data))
            {
                char separator;
                string[] items;
                TypeConverter converter;

                if (culture == null)
                    culture = CultureInfo.CurrentCulture;

                //result = new ZoomLevelCollection();
                result = new ImageGlass.ImageBoxZoomLevelCollection();
                separator = culture.TextInfo.ListSeparator[0];
                items = data.Split(separator);
                converter = TypeDescriptor.GetConverter(typeof(int));

                foreach (string item in items)
                    result.Add((int)converter.ConvertFromString(context, culture, item));
            }
            else
                result = null;

            return result;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result;

            if (destinationType == null)
                throw new ArgumentNullException("destinationType");

            if (value is ImageGlass.ImageBoxZoomLevelCollection)
            {
                if (destinationType == typeof(string))
                {
                    ImageGlass.ImageBoxZoomLevelCollection collection;
                    StringBuilder data;
                    string separator;
                    TypeConverter converter;

                    collection = (ImageGlass.ImageBoxZoomLevelCollection)value;
                    if (culture == null)
                        culture = CultureInfo.CurrentCulture;
                    separator = culture.TextInfo.ListSeparator + " ";
                    converter = TypeDescriptor.GetConverter(typeof(int));
                    data = new StringBuilder();

                    foreach (int item in collection)
                    {
                        if (data.Length != 0)
                            data.Append(separator);

                        data.Append(converter.ConvertToString(context, culture, item));
                    }

                    result = data.ToString();
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    ImageGlass.ImageBoxZoomLevelCollection collection;
                    ConstructorInfo constructor;

                    collection = (ImageGlass.ImageBoxZoomLevelCollection)value;
                    constructor = typeof(ImageGlass.ImageBoxZoomLevelCollection).GetConstructor(new Type[] { typeof(IList<int>) });

                    result = new InstanceDescriptor(constructor, new object[] { collection.ToArray() });
                }
                else
                    result = null;
            }
            else
                result = null;

            if (result == null)
                result = base.ConvertTo(context, culture, value, destinationType);

            return result;
        }
    }
}
