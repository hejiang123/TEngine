//代码自动生成，切勿修改!!!

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
	public sealed partial class UI_CommonDialogBox : GComponent
	{
		public GGraph Mask;
		public UI_TestBtn OK;
		public UI_TestBtn Cancel;
		public UI_TestBtn Cancel2;
		public const string URL = "ui://2twj9h7gwpko3";

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			Mask = (GGraph)GetChildAt(0);
			OK = (UI_TestBtn)GetChildAt(2);
			Cancel = (UI_TestBtn)GetChildAt(3);
			Cancel2 = (UI_TestBtn)GetChildAt(4);
		}
	}
}