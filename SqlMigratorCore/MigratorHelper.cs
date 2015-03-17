using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace SqlMigratorCore
{
    public static class MigratorHelper
    {
        public static class DataBaseParm
        {
            public const string scriptPath = @"Database\";
        }


        [Migration(2015031201)]
        public class AddTableUsers : Migration
        {
            public override void Up()
            {
                Create.Table("Users")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Name").AsString(255).NotNullable().WithDefaultValue("Anonymous");
            }

            public override void Down()
            {
                Delete.Table("Users");
            }
        }

        [Migration(2015031202)]
        public class AddTableDept : Migration
        {
            public override void Up()
            {
                Create.Table("Dept")
                .WithColumn("DeptId").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("DeptName").AsString(255).NotNullable().WithDefaultValue("Tech");
            }

            public override void Down()
            {
                Delete.Table("Dept");
            }
        }

        [Migration(2015031203)]
        public class AddLog : Migration
        {

            public override void Up()
            {
                Execute.Script(DataBaseParm.scriptPath + "CreateLog.sql");
            }

            public override void Down()
            {
                Execute.Script(DataBaseParm.scriptPath + "DropLog.sql");
            }
        }

        /// <summary>
        ///添加列
        /// </summary>
        [Migration(2015031301)]
        public class AlertUser : Migration
        {
            public override void Up()
            {
                Alter.Table("Users")
                    .AddColumn("Age")
                    .AsInt16()
                    .Nullable();
            }

            public override void Down()
            {
                Delete.Column("Age").FromTable("Users");
            }
        }

        /// <summary>
        /// 添加行数据
        /// </summary>
        [Migration(2015031302)]
        public class AddDeptRows : Migration
        {
            public override void Up()
            {
                Insert.IntoTable("Dept").Row(new { DeptName = "maoyatest" });
            }

            public override void Down()
            {
                Delete.FromTable("Dept").Row(new { DeptName = "maoyatest" });
            }
        }

        /// <summary>
        /// 修改表名称
        /// </summary>
        [Migration(2015031303)]
        public class RenameUsers : Migration
        {
            public override void Up()
            {
                Rename.Table("Users").To("UsersNew");
            }

            public override void Down()
            {
                Rename.Table("UsersNew").To("Users");
            }
        }

    }
}

