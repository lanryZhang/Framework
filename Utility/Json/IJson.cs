using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Ifeng.Json
{
	public interface IJson
	{
		string ToJsonString();
	}

	internal class IJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteRawValue((value as IJson).ToJsonString());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(IJson).IsAssignableFrom(objectType);
		}
	}
}
