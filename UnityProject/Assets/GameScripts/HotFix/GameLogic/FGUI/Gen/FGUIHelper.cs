//代码自动生成，切勿修改!!!

using FairyGUI;

public sealed class FGUIHelper
{
	public static string GetPackageNameByURL(string itemUrl)
	{
		string pkg = itemUrl.Substring(5, 8);
		return GetPackageNameById(pkg);
	}

	public static string GetPackageNameById(string pkgId)
	{
		switch(pkgId)
		{
			case Common.CommonPackage.packageId:
				return "Common";
			case Login.LoginPackage.packageId:
				return "Login";
			case Test.TestPackage.packageId:
				return "Test";
		}
		return "";
	}

	public static GComponent GetComponent(string itemUrl)
	{
		string pkg = itemUrl.Substring(5, 8);
		switch(pkg)
		{
			case Common.CommonPackage.packageId:
				return Common.CommonPackage.GetComponent(itemUrl);
			case Login.LoginPackage.packageId:
				return Login.LoginPackage.GetComponent(itemUrl);
			case Test.TestPackage.packageId:
				return Test.TestPackage.GetComponent(itemUrl);
		}
		return null;
	}
}