using System;
using System.Collections;
using System.Collections.Generic;

namespace dt
{
	public class Attribute
	{
		public string name;
		public Dictionary<string, int> values;

		public Attribute (string name)
		{
			this.name = name;
			this.values = new Dictionary<string, int>();
		}
	}
}

