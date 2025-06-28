using Newtonsoft.Json;
using SuiBotAI.Components.Other.Gemini.FunctionTypes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

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
		}

		[Serializable]
		public abstract class FunctionCall
		{
			public abstract string FunctionName();
			public abstract string FunctionDescription();
		}

		public GeminiTools() { }
		public GeminiTools(params FunctionCall[] functions)
		{
			functionDeclarations = new List<GeminiFunction>();
			foreach (var function in functions)
			{
				if (function == null)
					continue;

				var newGeminiFunction = new GeminiFunction
				{
					name = function.FunctionName(),
					description = function.FunctionDescription(),
					parameters = new ParametersContainer()
					{
						type = "object",
					}
				};

				newGeminiFunction.parameters = new ParametersContainer()
				{
					type = "object",
					required = new List<string>(),
					properties = new Dictionary<string, Gemini_Parameter_Type>()
				};

				var fields = function.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

				foreach (var fieldToProcess in fields)
				{
					var attributeData = fieldToProcess.GetCustomAttribute<FunctionCallParameterAttribute>();
					if (attributeData == null)
						continue;

					var fieldType = fieldToProcess.FieldType;
					if (fieldType == typeof(string))
						newGeminiFunction.parameters.properties.Add(fieldToProcess.Name, new Gemini_Parameter_Type.Parameter_String());
					else if (fieldType == typeof(bool))
						newGeminiFunction.parameters.properties.Add(fieldToProcess.Name, new Gemini_Parameter_Type.Parameter_Number());
					else
						throw new Exception("Unhandled conversion type");

					if (attributeData.IsRequired)
						newGeminiFunction.parameters.required.Add(fieldToProcess.Name);

				}

				functionDeclarations.Add(newGeminiFunction);
				Calls.Add(function.FunctionName(), function.GetType());
			}
		}

		public List<GeminiFunction> functionDeclarations = new List<GeminiFunction>();
		[NonSerialized]
		[XmlIgnore]
		[JsonIgnore]
		public Dictionary<string, Type> Calls = new Dictionary<string, Type>();
	}

	public class FunctionCallParameterAttribute : Attribute
	{
		public bool IsRequired { get; private set; }

		public FunctionCallParameterAttribute(bool required)
		{
			this.IsRequired = required;
		}
	}
}
