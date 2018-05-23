using System;

namespace DoneByMe.Matching.Model
{
	public class Keywords
    {
		public string[] List { get; private set; }

		public Keywords(string[] keywords)
		{
			List = keywords;
		}
	}
}
