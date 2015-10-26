using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public class Table
	{
		public string Name;
		public List<Column> ColumnList = new List<Column>();	
	}

	public class Column
	{
		public string Name;
		public string Type;
		public bool IsKey;
		public bool IsIndex;
		public bool IsAutoIncrementKey;
		public bool IsVarchar;
		public bool IsNull;
		public int VarcharLength;
	}

	public static class DatabaseHelper
	{

		public static string ToDatabaseType(this string type)
		{
			var lowerType = type.ToLower();
			if (lowerType == "int32") return "int";
			if (lowerType == "int64") return "bigint";
			if (lowerType == "double") return "float";
			if (lowerType == "string") return "text";
			if (lowerType == "datetime") return "datetime";

			return type.ToLower();
		}

		public static Table ToDatabaseTable<T>()
		{
			var typeInfo = typeof(T);
			if (!typeInfo.HasAttribute<TableAttribute>())
				return null;

			var table = new Table { Name = typeInfo.Name };
			foreach (var member in typeInfo.GetFields())
			{
				table.ColumnList.Add(new Column()
				{
					Name = member.Name,
					Type = member.FieldType.Name.ToDatabaseType(),
					IsKey = member.HasAttribute<KeyAttribute>(),
					IsAutoIncrementKey = member.HasAttribute<KeyAttribute>() && member.GetAttribute<KeyAttribute>().IsAutoIncrement,
					IsIndex = member.HasAttribute<IndexAttribute>(),
					IsVarchar = member.HasAttribute<VarcharAttribute>(),
					IsNull = member.HasAttribute<IsNullAttribute>(),
					VarcharLength = member.HasAttribute<VarcharAttribute>() ? member.GetAttribute<VarcharAttribute>().Length : 0,
				});
			}
			return table;
		}

		public static string CreateTableQuery(this Table table)
		{
			var queryBuilder = new StringBuilder();
			queryBuilder.Append(string.Format("CREATE TABLE {0}", table.Name) + Environment.NewLine);
			queryBuilder.Append("(" + Environment.NewLine);
			foreach (var elem in table.ColumnList)
			{
				queryBuilder.Append(
					string.Format("\t{0} {1}{2}{3}{4}{5}",
						elem.Name,
						elem.IsVarchar ? string.Format("nvarchar({0})", elem.VarcharLength) : elem.Type,
						elem.IsNull ? " NULL " : " NOT NULL ",
						elem.IsAutoIncrementKey ? " AUTO_INCREMENT " : "",
						",",
						Environment.NewLine));
			}
			if (table.ColumnList.Where(e => e.IsKey).Count() > 0)
			{
				queryBuilder.Append(string.Format("\tPRIMARY KEY ({0})",
					string.Join(", ", table.ColumnList.Where(e => e.IsKey).Select(e => e.Name))));
			}

			foreach (var elem in table.ColumnList.Where(e => e.IsIndex))
			{
				queryBuilder.Append(
					string.Format(",{0}\tINDEX Index_{1}_{2} ({2})",
						Environment.NewLine,
						table.Name,
						elem.Name));
			}

			queryBuilder.Append(Environment.NewLine + ")" + Environment.NewLine);

			return queryBuilder.ToString();
		}
	}
}
