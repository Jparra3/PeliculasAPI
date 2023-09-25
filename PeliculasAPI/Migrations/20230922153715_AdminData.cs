using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeliculasAPI.Migrations
{
    public partial class AdminData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"            
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] ON;
INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
VALUES (N'4218f49a-896e-4360-aea4-f0cc4a4c7861', N'7f2a1fd2-0433-4f0a-bb31-6038ccd33fcd', N'Admin', N'Admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'ConcurrencyStamp', N'Email', N'EmailConfirmed', N'LockoutEnabled', N'LockoutEnd', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
    SET IDENTITY_INSERT [AspNetUsers] ON;
INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [ConcurrencyStamp], [Email], [EmailConfirmed], [LockoutEnabled], [LockoutEnd], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName])
VALUES (N'e701deef-1235-4fae-99b5-c2c3e11104a3', 0, N'7cccb0d5-9f9c-4908-a920-0f3451d0f1bc', N'jhonatanjopa@gmail.com', CAST(0 AS bit), CAST(0 AS bit), NULL, N'jhonatanjopa@gmail.com', N'jhonatanjopa@gmail.com', N'AQAAAAEAACcQAAAAEChNLDj4FV+2gbPF3rQvs4KZ3IH69FUZ26EkOkObyTPmvNL3U6Vlt7jXqDRZpmYZxQ==', NULL, CAST(0 AS bit), N'769cb3c7-4799-45ac-b287-624de8f0aef4', CAST(0 AS bit), N'jhonatanjopa@gmail.com');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'ConcurrencyStamp', N'Email', N'EmailConfirmed', N'LockoutEnabled', N'LockoutEnd', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
    SET IDENTITY_INSERT [AspNetUsers] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClaimType', N'ClaimValue', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserClaims]'))
    SET IDENTITY_INSERT [AspNetUserClaims] ON;
INSERT INTO [AspNetUserClaims] ([Id], [ClaimType], [ClaimValue], [UserId])
VALUES (1, N'http://schemas.microsoft.com/ws/2008/06/identity/claims/role', N'Admin', N'e701deef-1235-4fae-99b5-c2c3e11104a3');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClaimType', N'ClaimValue', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserClaims]'))
    SET IDENTITY_INSERT [AspNetUserClaims] OFF;
GO
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "4218f49a-896e-4360-aea4-f0cc4a4c7861");

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e701deef-1235-4fae-99b5-c2c3e11104a3");

        }
    }
}
