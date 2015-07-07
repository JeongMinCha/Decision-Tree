using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dt
{
	public class Tuple
	{
		public int[] values;

		public Tuple ()
		{
			this.values = new int[Globals.attrCount];
		}
	}
}