//代码自动生成，切勿修改!!!

using FairyGUI;

namespace Common
{
	public sealed class CommonPackage
	{
		public const string packageId = "2twj9h7g";
		public static GComponent GetComponent(string itemUrl)
		{
			switch(itemUrl)
			{
				case UI_CommonInput.URL:
					return new UI_CommonInput();
				case UI_TestBtn.URL:
					return new UI_TestBtn();
				case UI_CommonDialogBox.URL:
					return new UI_CommonDialogBox();
				default:
					return null;
			}
		}
	}
}