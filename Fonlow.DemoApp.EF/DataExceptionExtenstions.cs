using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Fonlow.DemoApp.EF
{
	public static  class DataExceptionExtenstions
	{
		public static string ToErrorsText(this DbUpdateException ex)
		{
			StringBuilder builder = new();
			builder.AppendLine(ex.Message);

			foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry item in ex.Entries)
			{
				builder.AppendLine(item.Entity.ToString());
			}

			System.Exception innerException = ex.InnerException;
			if (innerException != null)
			{
				builder.AppendLine(ex.InnerException.Message);
				if (innerException.InnerException != null)
				{
					builder.AppendLine("  " + innerException.InnerException.Message);
				}
			}
			return builder.ToString();
		}
	}
}
