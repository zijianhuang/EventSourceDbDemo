using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Fonlow.DemoApp.Models
{
	[Serializable]
	public class DemoAppException : Exception
	{
		public DemoAppException(string messaage) : base(messaage)
		{

		}

		public DemoAppException() : base()
		{

		}

		public DemoAppException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}


	[Serializable]
	public class DemoAppArgumentException : ArgumentException
	{
		public DemoAppArgumentException(string messaage) : base(messaage)
		{

		}

		public DemoAppArgumentException() : base()
		{

		}

		public DemoAppArgumentException(string message, Exception innerException) : base(message, innerException)
		{

		}

		public DemoAppArgumentException(string message, string paramName) : base(message, paramName)
		{

		}


		public DemoAppArgumentException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
		{

		}

	}

	[Serializable]
	public class DemoAppArgumentNullException : DemoAppArgumentException
	{
		public DemoAppArgumentNullException(string messaage) : base(messaage)
		{

		}

		public DemoAppArgumentNullException() : base()
		{

		}

		public DemoAppArgumentNullException(string message, Exception innerException) : base(message, innerException)
		{

		}

		public DemoAppArgumentNullException(string message, string paramName) : base(message, paramName)
		{

		}
	}

}
