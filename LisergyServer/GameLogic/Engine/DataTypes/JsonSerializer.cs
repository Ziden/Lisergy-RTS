using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Game.Engine.DataTypes
{
	public class FieldsOnlyContractResolver : DefaultContractResolver
	{
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);

			// check member types
			if (member.MemberType == MemberTypes.Property)
				property.ShouldSerialize = _ => false; // Ignore all properties

			return property;
		}
	}

	public class ByteBackedStructConverter<T> : JsonConverter<T> where T : struct
	{
		public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
		{
			var idField = typeof(T).GetField("Id"); // Get the "Id" field dynamically
			if (idField != null)
			{
				var idValue = (byte) idField.GetValue(value); // Get the byte value
				writer.WriteValue(idValue);
			}
			else
			{
				throw new JsonSerializationException($"No 'Id' field found in {typeof(T).Name}");
			}
		}

		public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue,
			JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.String)
			{
				var idValue = Convert.ToByte(reader.Value);
				return (T) Activator.CreateInstance(typeof(T), idValue);
			}

			throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing {typeof(T).Name}");
		}
	}
}