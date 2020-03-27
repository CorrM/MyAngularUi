using MAU.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static MAU.Events.UiEventHandler;

namespace MAU.ReadyElement
{
	public class UiSelect : UiElement
	{
		public string OptionTagName { get; set; }

		public UiSelect(string id, string optionTagName) : base(id)
		{
			OptionTagName = optionTagName;
		}

		public Dictionary<string, string> GetValues()
		{
			return new Dictionary<string, string>();
		}

		public async Task AddOption(string value, string viewValue)
		{
			string code = $"var option = document.createElement(\"mat-option\");\r\noption.innerText = \"GG\";\r\noption.value = \"myvalue\";\r\n{Id}.appendChild(option);";

			var data = new JObject()
			{
				{ "code", code }
			};

			await MyAngularUi.SendRequest(Id, MyAngularUi.RequestType.ExecuteCode, data);
		}
	}
}
