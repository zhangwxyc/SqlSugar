﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class DemoE_CodeFirst
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### CodeFirst Start ####");
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.QuestDB,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.DbMaintenance.CreateDatabase();
            db.CodeFirst.InitTables(typeof(CodeFirstTable1));//Create CodeFirstTable1 
            db.Insertable(new CodeFirstTable1() { Name = "a", Text = "a" }).ExecuteCommand();
            var list = db.Queryable<CodeFirstTable1>().ToList();
            db.CodeFirst.InitTables<IndexClass>();
            db.CodeFirst.InitTables<SplitTableEntity>();
            TestBool(db);
            TestGuid(db);
            Console.WriteLine("#### CodeFirst end ####");
        }
        private static void TestGuid(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<GuidTest>();
            db.DbMaintenance.TruncateTable("BoolTest");
            var Id = 1;
            db.Insertable<GuidTest>(new GuidTest() { A = Guid.Empty, Id = Id }).ExecuteCommand();
            Console.Write(db.Queryable<GuidTest>().First().A);
            db.Updateable<GuidTest>(new GuidTest() { A = Guid.NewGuid(), Id = Id }).ExecuteCommand();
            Console.Write(db.Queryable<GuidTest>().First().A);
        }
        private static void TestBool(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<BoolTest2>();
            db.DbMaintenance.TruncateTable("BoolTest");
            var Id = 1;
            db.Insertable<BoolTest2>(new BoolTest2() { A = true, Id = Id }).ExecuteCommand();
            Console.Write(db.Queryable<BoolTest2>().First().A);
            db.Updateable<BoolTest2>(new BoolTest2() { A = false, Id = Id }).ExecuteCommand();
            Console.Write(db.Queryable<BoolTest2>().First().A);
        }
    }
    public class GuidTest
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }
        public Guid A { get; set; }
    }
    public class BoolTest2
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }
        public bool A { get; set; }
    }

    [SugarIndex(null, nameof(IndexClass.Name), OrderByType.Asc)]
    public class IndexClass
    {
        public int Id { get; set; }
        [SugarColumn(ColumnDataType = "symbol")]
        public string Name { get; set; }
    }



    public class SplitTableEntity
    {
        public string Id { get; set; }
        [TimeDbSplitField(DateType.Day)]
        public DateTime Ts { get; set; }
    }


    public class CodeFirstTable1
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "string")]//custom
        public string Text { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
    }
}
