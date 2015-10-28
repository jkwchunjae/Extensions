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
		public int IndexId;
		public int IndexOrder;

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
		}

		public static Table ToDatabaseTable<T>()
		{
			var typeInfo = typeof(T);
			return ToDatabaseTable(typeInfo);
		}

		public static Table ToDatabaseTable(Type typeInfo)
		{
			if (!typeInfo.HasAttribute<TableAttribute>())
				return null;

			var table = new Table { Name = typeInfo.GetAttribute<TableAttribute>() != null ? typeInfo.GetAttribute<TableAttribute>().TableName : typeInfo.Name };
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
					VarcharLength = member.GetAttribute<VarcharAttribute>() != null ? member.GetAttribute<VarcharAttribute>().Length : 0,
					IndexId = member.HasAttribute<IndexAttribute>() ? member.GetAttribute<IndexAttribute>().IndexId : 0,
					IndexOrder = member.HasAttribute<IndexAttribute>() ? member.GetAttribute<IndexAttribute>().IndexOrder : 0,
				});
			}
			return table;
		}

		public static string CreateTableQuery(this Table table)
		{
			var queryCreateTable = @"
CREATE TABLE `{TableName}`
(
	{columnInfoList}
	{primaryKeyInfo}
	{indexInfo}
);
";
			#region ColumnInfo
			var columnInfoList = table.ColumnList
				.Select(column => "`{ColumnName}` {Type} {Null} {AutoInc}".WithVar(new
					{
						ColumnName = column.Name.ToCamelCase(),
						Type = column.SqlType,
						Null = column.IsNull ? "NULL" : "NOT NULL",
						AutoInc = column.IsAutoIncrementKey ? "AUTO_INCREMENT" : "",
					}))
				.StringJoin(",\n\t");
			#endregion

			#region PrimaryKey
			var primaryKeyInfo = !table.ColumnList.Where(x => x.IsKey).Any() ? string.Empty :
				", PRIMARY KEY ({0})".With(table.ColumnList
				.Where(x => x.IsKey)
				.Select(x => "`{0}`".With(x.Name.ToCamelCase()))
				.StringJoin(", "));
			#endregion

			#region IndexInfo
			var indexInfo1 = table.ColumnList
				.Where(x => x.IsIndex && x.IndexId == 0)
				.Select(x => "INDEX Index_{TableName}_{ColumnName} (`{ColumnName}`)".WithVar(new
					{
						TableName = table.Name,
						ColumnName = x.Name,
					}));
			var indexInfo2 = table.ColumnList
				.Where(x => x.IsIndex && x.IndexId != 0)
				.GroupBy(x => x.IndexId)
				.Select(x => "INDEX Index_{TableName}_{ColumnNames1} ({ColumnNames2})".WithVar(new
					{
						TableName = table.Name,
						ColumnNames1 = x.Select(e => e.Name).StringJoin("_"),
						ColumnNames2 = x.OrderBy(e => e.IndexOrder).Select(e => "`{0}`".With(e.Name)).StringJoin(", "),
					}));
			var indexInfo = (indexInfo1.Concat(indexInfo2).Any() ? ", " : "") + indexInfo1.Concat(indexInfo2)
				.StringJoin(",\n\t");
			#endregion

			return queryCreateTable.WithVar(new { TableName = table.Name, columnInfoList, primaryKeyInfo, indexInfo });
		}
	}
}
