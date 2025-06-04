using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SuiBotAI.Components.Other.Gemini.FunctionTypes
{
	[Serializable]
	public class ParametersContainer
	{
		public string type = "object"; //return type
		public GeminiProperty properties = null;
		public List<string> required = new List<string>();

		public ParametersContainer() { }
	}

	[Serializable]
	public abstract class GeminiProperty
	{
		public abstract List<string> GetRequiredFieldsNames();

		public class Parameter_String
		{
			public string type = "string";
		}

		public class Parameter_Number
		{
			public string type = "number";
		}
	}

	[Serializable]
	public class PurgeMessage : GeminiProperty
	{
		public Parameter_String username;

		public PurgeMessage()
		{
			username = new Parameter_String();
		}

		public override List<string> GetRequiredFieldsNames() => new List<string>() { };
	}
}
