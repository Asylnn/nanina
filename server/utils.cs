using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

public static class Utils
{
    public static ulong GetTimestamp()
    {
        return (ulong)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
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
                if (value != null && value.GetType().IsClass && !value.GetType().FullName.StartsWith("System."))
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
}