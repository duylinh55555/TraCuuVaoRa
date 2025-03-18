using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraCuuVaoRa_WPF
{
    public class DataContext : DbContext
    {
        // Database credentials
        public string dataSource = "DUYLINH";
        public string initialCatalog = "SmartCard";
        public string userId = "sa";
        public string password = "123";
        public bool integratedSecurity = true;
        public bool encrypt = false;
        public bool trustServerCertificate = true;

        public TimeSpan[] timeSpans;

        public string xeVaoImageFolderUrl = "D:/XeVao/";
        public string xeRaImageFolderUrl = "D:/XeRa/";

        string ConnectionString
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append($"Data Source={dataSource};");
                builder.Append($"Initial Catalog={initialCatalog};");
                if (integratedSecurity)
                {
                    builder.Append("Integrated Security=True;");
                }
                else
                {
                    builder.Append($"User ID={userId};");
                    builder.Append($"Password={password};");
                }
                builder.Append($"Encrypt={encrypt};");
                builder.Append($"Trust Server Certificate={trustServerCertificate};");
                return builder.ToString();
            }
        }

        public DataContext() : base()
        {
            
            // morning
            TimeSpan lateMorningStartTime = new TimeSpan(6, 45, 0);
            TimeSpan lateMorningEndTime = new TimeSpan(7, 15, 0);
            TimeSpan earlyNoonStartTime = new TimeSpan(11, 15, 0);
            TimeSpan earlyNoonEndTime = new TimeSpan(11, 40, 0);

            // aternoon
            TimeSpan lateAfternoonStartTime = new TimeSpan(13, 15, 0);
            TimeSpan lateAfternoonEndTime = new TimeSpan(13, 45, 0);
            TimeSpan earlyAfternoonStartTime = new TimeSpan(15, 45, 0);
            TimeSpan earlyAfternoonEndTime = new TimeSpan(16, 15, 0);

            timeSpans = new TimeSpan[]
            {
                lateMorningStartTime,
                lateMorningEndTime,
                earlyNoonStartTime,
                earlyNoonEndTime,
                lateAfternoonStartTime,
                lateAfternoonEndTime,
                earlyAfternoonStartTime,
                earlyAfternoonEndTime
            };
        }

        public void SetDatabaseCredentials(string dataSource, string userId, string password)
        {
            this.dataSource = dataSource;
            this.userId = userId;
            this.password = password;
            this.integratedSecurity = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        public bool CheckDatabaseConnection(string dataSource, string userId, string password)
        {
            SetDatabaseCredentials(dataSource, userId, password);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    return true; // Connection successful
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"SQL Server Connection Error: {ex.Message}");
                    return false; // Connection failed
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Error: {ex.Message}");
                    return false; // Other errors
                }
            }
        }

        public DbSet<Car> Car { get; set; }
        public DbSet<VeThang> VeThang { get; set; }
        public DbSet<Part> Part { get; set; }
    }
}
