using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public class IndexAttribute : Attribute
	{
	}

	public class KeyAttribute : Attribute
	{
		public bool IsAutoIncrement;
		public KeyAttribute(bool isAutoIncrement = false)
		{
			IsAutoIncrement = isAutoIncrement;
		}
	}

	public class VarcharAttribute : Attribute
	{
		public int Length;
		public VarcharAttribute(int length)
		{
			Length = length;
		}
	}

	public class IsNullAttribute : Attribute
	{
	}

	public class TableAttribute : Attribute
	{
		public string TableName;
		public TableAttribute(string tableName = null)
		{
			TableName = tableName;
		}
	}
}
