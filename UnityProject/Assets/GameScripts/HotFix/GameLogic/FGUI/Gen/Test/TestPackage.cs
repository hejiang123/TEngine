//代码自动生成，切勿修改!!!

using FairyGUI;

namespace Test
{
	public sealed class TestPackage
	{
		public const string packageId = "ecnap8i9";
		public static GComponent GetComponent(string itemUrl)
		{
			switch(itemUrl)
			{
				case UI_TestMainWindow.URL:
					return new UI_TestMainWindow();
				default:
					return null;
			}
		}
	}
}