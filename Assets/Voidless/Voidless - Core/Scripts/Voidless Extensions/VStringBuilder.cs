using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Voidless
{
	public static class VStringBuilder
	{
		public static void Clear(this StringBuilder _builder)
		{
			_builder.Length = 0;
		}
	}
}