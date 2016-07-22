using System.Collections.Generic;

public abstract class EasyPrint
{
	private EasyPrint() {}

	public static string MakeString<T> (IEnumerable<T> inputCollection)
	{
		if (inputCollection == null)
			return "Collection is null";
		else
		{
			string oStr = "";

			foreach (T entry in inputCollection)
			{
				oStr += entry.ToString() + " ";
			}
			return oStr;
		}
		
	}
}