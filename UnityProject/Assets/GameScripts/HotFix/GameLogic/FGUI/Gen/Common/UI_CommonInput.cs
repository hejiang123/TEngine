//代码自动生成，切勿修改!!!

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
	public sealed partial class UI_CommonInput : GComponent
	{
		public GTextInput title;
		public const string URL = "ui://2twj9h7gvagg5";

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			title = (GTextInput)GetChildAt(1);
		}
	}
}