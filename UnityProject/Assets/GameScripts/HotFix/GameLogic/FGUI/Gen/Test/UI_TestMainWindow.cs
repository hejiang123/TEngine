//代码自动生成，切勿修改!!!

using FairyGUI;
using FairyGUI.Utils;

namespace Test
{
	public sealed partial class UI_TestMainWindow : GComponent
	{
		public GGraph safeArea;
		public GRichTextField Txt1;
		public Common.UI_TestBtn Btn1;
		public const string URL = "ui://ecnap8i9gkoy0";

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			safeArea = (GGraph)GetChildAt(1);
			Txt1 = (GRichTextField)GetChildAt(2);
			Btn1 = (Common.UI_TestBtn)GetChildAt(3);
		}
	}
}