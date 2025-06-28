using System;
using System.Collections.Generic;

namespace SuiBotAI.Components.Other.Gemini.FunctionTypes
{
	[Serializable]
	public class ParametersContainer
	{
		public string type = "object"; //return type
		public Dictionary<string, Gemini_Parameter_Type> properties = null;
		public List<string> required = new List<string>();

		public ParametersContainer() { }
	}

	public class Gemini_Parameter_Type
	{
		public string type;

		public class Parameter_String : Gemini_Parameter_Type
		{
			public Parameter_String() => type = "string";
		}

		public class Parameter_Number : Gemini_Parameter_Type
		{
			public Parameter_Number() => type = "number";
		}
	}
}
