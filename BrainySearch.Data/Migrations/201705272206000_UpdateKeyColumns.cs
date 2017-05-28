namespace BrainySearch.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.Core.Metadata.Edm;

    public partial class UpdateKeyColumns : DbMigration
    {
        public override void Up()
        {
            AlterColumn("KeyWords", "Id", (columnBuilder) =>
            {
                return new ColumnModel(PrimitiveTypeKind.Guid) { IsIdentity = true };
            });

            AlterColumn("InitialInfoes", "Id", (columnBuilder) =>
            {
                return new ColumnModel(PrimitiveTypeKind.Guid) { IsIdentity = true };
            });

            AlterColumn("Lectures", "Id", (columnBuilder) =>
            {
                return new ColumnModel(PrimitiveTypeKind.Guid) { IsIdentity = true };
            });
        }
        
        public override void Down()
        {
            AlterColumn("KeyWords", "Id", (columnBuilder) =>
            {
                return new ColumnModel(PrimitiveTypeKind.Guid) { IsIdentity = false };
            });

            AlterColumn("InitialInfoes", "Id", (columnBuilder) =>
            {
                return new ColumnModel(PrimitiveTypeKind.Guid) { IsIdentity = false };
            });

            AlterColumn("Lectures", "Id", (columnBuilder) =>
            {
                return new ColumnModel(PrimitiveTypeKind.Guid) { IsIdentity = false };
            });
        }
    }
}
