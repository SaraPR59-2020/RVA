namespace BookstoreBackend.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        AuthorId = c.Int(nullable: false, identity: true),
                        ShortDesc = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                    })
                .PrimaryKey(t => t.AuthorId);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        BookId = c.Int(nullable: false, identity: true),
                        AuthorId = c.Int(nullable: false),
                        Username = c.String(maxLength: 128),
                        Title = c.String(),
                        PublishYear = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BookId)
                .ForeignKey("dbo.Authors", t => t.AuthorId, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.Username)
                .Index(t => t.AuthorId)
                .Index(t => t.Username);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        Username = c.String(nullable: false, maxLength: 128),
                        Password = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        IsAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Username);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Books", "Username", "dbo.Members");
            DropForeignKey("dbo.Books", "AuthorId", "dbo.Authors");
            DropIndex("dbo.Books", new[] { "Username" });
            DropIndex("dbo.Books", new[] { "AuthorId" });
            DropTable("dbo.Members");
            DropTable("dbo.Books");
            DropTable("dbo.Authors");
        }
    }
}
