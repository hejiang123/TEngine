//代码自动生成，切勿修改!!!

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
	public sealed partial class UI_LoginMainWindow : GComponent
	{
		public Common.UI_CommonInput UserNameInput;
		public GButton LoginBtn;
		public const string URL = "ui://wgwpucc7vagg0";

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			UserNameInput = (Common.UI_CommonInput)GetChildAt(0);
			LoginBtn = (GButton)GetChildAt(1);
		}
	}
}