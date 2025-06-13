//代码自动生成，切勿修改!!!

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
	public sealed partial class UI_TestBtn : GButton
	{
		public GGraph P0;
		public GGraph P1;
		public GGraph P2;
		public const string URL = "ui://2twj9h7gw8ub1";

		public override void ConstructFromXML(XML xml)
		{
			base.ConstructFromXML(xml);

			P0 = (GGraph)GetChildAt(0);
			P1 = (GGraph)GetChildAt(1);
			P2 = (GGraph)GetChildAt(2);
		}
	}
}