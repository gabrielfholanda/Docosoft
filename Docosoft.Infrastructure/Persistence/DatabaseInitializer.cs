using Docosoft.Infrastructure.Persistence.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docosoft.Infrastructure.Persistence
{
    public class DatabaseInitializer
    {
        public static void EnsureDatabaseAndTableCreated(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = builder.InitialCatalog;
            var masterConnectionString = builder.ConnectionString;
            builder.InitialCatalog = "master";
            masterConnectionString = builder.ConnectionString;

            using (var connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();
                var checkDbCmd = $@"
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{databaseName}')
BEGIN
    CREATE DATABASE [{databaseName}];
END";
                using var cmd = new SqlCommand(checkDbCmd, connection);
                cmd.ExecuteNonQuery();
            }

            Log.Information("Database '{Database}' checked/created.", databaseName);

            using var dbConnection = new SqlConnection(connectionString);
            dbConnection.Open();

            var tableExistsCmd = @"
SELECT COUNT(*) 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Users';";

            using (var cmd = new SqlCommand(tableExistsCmd, dbConnection))
            {
                int tableCount = (int)cmd.ExecuteScalar();
                if (tableCount == 0)
                {
                    Log.Information("Table 'Users' does not exist. Creating...");

                    var createTableSql = @"
CREATE TABLE [dbo].[Users](
	[ID] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](150) NOT NULL,
	[PasswordHash] [text] NOT NULL,
	[PhoneNumber] [nvarchar](20) NOT NULL,
	[BirthDate] [date] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
	[UpdatedAt] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [IX_Users] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_IsActive]  DEFAULT ((1)) FOR [IsActive];";

                    using var createCmd = new SqlCommand(createTableSql, dbConnection);
                    createCmd.ExecuteNonQuery();

                    Log.Information("Table 'Users' created successfully.");
                }
                else
                {
                    Log.Information("Table 'Users' already exists.");
                }
            }
        }
    }
}
