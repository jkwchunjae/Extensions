using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

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

		public string SqlType
		{
			get
			{
				return IsVarchar ? "NVARCHAR({0})".With(VarcharLength) : Type;
			}
		}
	}

	public static class DatabaseHelper
	{
		static Dictionary<string, string> _typeDic = new Dictionary<string, string>()
		{
			{"int32", "INT"},
			{"int64", "BIGINT"},
			{"double", "DOUBLE"},
			{"string", "NVARCHAR"},
			{"datetime", "DATETIME"},
		};

		public static string ToDatabaseType(this string type)
		{
			if (_typeDic.ContainsKey(type.ToLower()))
				return _typeDic[type.ToLower()];
			return type.ToUpper();

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
			queryBuilder.AppendLine("CREATE TABLE `{0}`".With(table.Name));
			queryBuilder.AppendLine("(");
			foreach (var column in table.ColumnList)
			{
				queryBuilder.AppendLine(
					"\t`{ColumnName}` {Type} {Null} {AutoInc},".WithVar(new
					{
						ColumnName = column.Name,
						Type = column.SqlType,
						Null = column.IsNull ? "NULL" : "NOT NULL",
						AutoInc = column.IsAutoIncrementKey ? "AUTO_INCREMENT" : "",
					}));
					/*
						elem.Name,
						elem.IsVarchar ? "NVARCHAR({0})".With(elem.VarcharLength) : elem.Type,
						elem.IsNull ? " NULL " : " NOT NULL ",
						elem.IsAutoIncrementKey ? " AUTO_INCREMENT " : "",
						",",
						Environment.NewLine));
				*/
			}
			if (table.ColumnList.Where(e => e.IsKey).Count() > 0)
			{
				queryBuilder.AppendLine(string.Format("\tPRIMARY KEY ({0})",
					table.ColumnList.Where(e => e.IsKey).Select(e => "`{0}`".With(e.Name)).StringJoin(", ")));
			}

			foreach (var elem in table.ColumnList.Where(e => e.IsIndex))
			{
				queryBuilder.AppendLine(
					string.Format(",{0}\tINDEX Index_{1}_{2} ({2})",
						Environment.NewLine,
						table.Name,
						elem.Name));
			}

			queryBuilder.AppendLine(Environment.NewLine + ")");

			return queryBuilder.ToString();
		}
	}
}
