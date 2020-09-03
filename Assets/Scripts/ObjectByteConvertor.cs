using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

public class ObjectByteConvertor
{

    public static byte[] SerializeObject<T>(T objectToSerialize)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream memStr = new MemoryStream();


        bf.Serialize(memStr, objectToSerialize);

        memStr.Position = 0;


        //return "";
        return memStr.ToArray();
    }

    public static T DeserializeObject<T>(byte[] dataStream)
    {
        MemoryStream stream = new MemoryStream(dataStream);
        stream.Position = 0;
        BinaryFormatter bf = new BinaryFormatter();
        bf.Binder = new VersionFixer();
        T retV = (T)bf.Deserialize(stream);
        return retV;
    }

    sealed class VersionFixer : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;
            String assemVer1 = Assembly.GetExecutingAssembly().FullName;
            if (assemblyName != assemVer1)
            {
                // To use a type from a different assembly version, 
                // change the version number.
                // To do this, uncomment the following line of code.
                assemblyName = assemVer1;
                // To use a different type from the same assembly, 
                // change the type name.
            }
            // The following line of code returns the type.
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
            return typeToDeserialize;
        }
    }
}
