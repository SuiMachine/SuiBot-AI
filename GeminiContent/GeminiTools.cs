using SuiBotAI.Components.Other.Gemini.FunctionTypes;
using System;
using System.Collections.Generic;

namespace SuiBotAI.Components.Other.Gemini
{
	public class GeminiTools
	{
		[Serializable]
		public class GeminiFunction
		{
			public string name;
			public string description;
			public ParametersContainer parameters;

			public GeminiFunction()
			{
				name = "";
				description = "";
				parameters = null;
			}

			public GeminiFunction(string name, string description, GeminiProperty functionDefinition)
			{
				this.name = name;
				this.description = description;
				this.parameters = new ParametersContainer()
				{
					type = "object",
					properties = functionDefinition,
					required = functionDefinition.GetRequiredFieldsNames()
				};
			}
		}

		public List<GeminiFunction> functionDeclarations = new List<GeminiFunction>();
	}
}
