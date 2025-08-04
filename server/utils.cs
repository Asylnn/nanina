using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Nanina.Crafting;
using Newtonsoft.Json;

public static class Utils
{
    public static ulong GetTimestamp()
    {
        return (ulong)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }
    public static string CreateId()
    {   
        Random rng = new Random();
        return Utils.GetTimestamp().ToString() + (rng.Next(89_999_999) + 10_000_000).ToString();
    }
    public static ulong CreateIdUlong()
    {   
        Random rng = new Random();
        return GetTimestamp() + (ulong)(rng.Next(89_999_999) + 10_000_000);
    }

    //yoinked from the internet, sorry, best next choice was serializing and deserializing.
    public static T DeepCopyReflection<T>(T input)
    {
        var type = input.GetType();
        var properties = type.GetProperties();
        T clonedObj = (T)Activator.CreateInstance(type);
        foreach (var property in properties)
        {
            if (property.CanWrite)
            {

                object value = property.GetValue(input);
                if (value != null && property.PropertyType.FullName.StartsWith("System.Collections.Generic.List"))
                {
                    
                    //var ListElementType = property.PropertyType.GetGenericArguments().First();
                    dynamic newList = Activator.CreateInstance(property.PropertyType);
                    Action<object> forEach = (elem) => {
                        if (elem != null && elem.GetType().IsClass && !elem.GetType().FullName.StartsWith("System."))
                        {
                            newList.Add((dynamic) DeepCopyReflection(elem));
                        }
                        else
                            newList.Add(elem);
                    };

                    //((dynamic)value).ForEach(uwu2); Simpler way with dynamic
                    property.PropertyType.GetMethod("ForEach").Invoke(value, [forEach]);
                    property.SetValue(clonedObj, newList);

                    //Couldn't make ConvertAll work
                    /* Converter<object, object> uwu2 = (x) => {
                         return DeepCopyReflection(x);
                    };
                    Type generic = typeof(Converter<, >).MakeGenericType(ListElementType, ListElementType);
                    var uwu3 = uwu2.GetMethodInfo().CreateDelegate(generic);
                    //Cannot bind to the target method because its signature is not compatible with that of the delegate type.
                    /*var uuu = property.GetValue(input).GetType().GetMethod("ConvertAll").MakeGenericMethod(ListElementType).Invoke(value, [uwu3]);*/

                    
                }
                else if (value != null && property.PropertyType.IsClass && !property.PropertyType.FullName.StartsWith("System."))
                {
                    property.SetValue(clonedObj, DeepCopyReflection(value));
                }
                else
                {
                    property.SetValue(clonedObj, value);
                }
            }
        }
        return clonedObj;
    }
    public static T ScuffedJsonSerializationDeepCopy<T>(T input)
    {
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(input));
    }

    public static float RandomMultiplicator(float plusMinus)
    {
        return 1 + (float) (new Random().NextDouble()*2 - 1)*plusMinus;
    }
}