
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDR.Domain.Model.CustomLines
{
	/// <summary>
	/// Effort line
	/// </summary>
	public class EffortLine : CustomLine
	{
		public virtual CarInspection CarInspection { get; set; }
	}
}
