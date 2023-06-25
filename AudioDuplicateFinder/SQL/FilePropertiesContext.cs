using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioDuplicateFinder.FileUtils;
using Microsoft.EntityFrameworkCore;

namespace AudioDuplicateFinder.SQL
{
    public partial class FilePropertiesContext : DbContext
    {
        public virtual DbSet<FileProperty> FileProperties { get; set; }

        public string ConnectionString { get; }
        public FilePropertiesContext(DbContextOptions<FilePropertiesContext> options) : base(options) 
        {
            ConnectionString = "Data Source=.\\Database\\mfdf.db";
        }
        public FilePropertiesContext(String fullPath = null)
        {
            //  Scaffold-DbContext "Data Source=.\Database\mfdf.db" Microsoft.EntityFrameworkCore.Sqlite -OutputDir Models
            //DbPath = (fullPath == null) ? System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mfdf.db") : fullPath;
            ConnectionString = (fullPath == null) ? "Data Source=.\\Database\\mfdf.db" : $"Data Source={fullPath}";
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(ConnectionString);
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileProperty>(entity => {
                entity.HasKey(e => new { e.ParentDir, e.Name });
                entity.Property(e => e.ParentDir).HasColumnType("TEXT");
                entity.Property(e => e.Name).HasColumnType("TEXT");
                entity.Property(e => e.Ext).HasColumnType("TEXT");
                entity.Property(e => e.MediaType).HasColumnType("INTEGER");
                entity.Property(e => e.Size).HasColumnType("INTEGER");
                entity.Property(e => e.Duration).HasColumnType("INTEGER");
                entity.Property(e => e.CheckSum).HasColumnType("INTEGER");
                entity.Property(e => e.FingerPrint).HasColumnType("BLOB");
            });
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        public void Add(FileProperty item)
        {
            FileProperties.Add(item);
            SaveChanges();
        }
        public static void AddItem(FileProperty item)
        {
            using ( FilePropertiesContext db = new () )
            {
                db.FileProperties.Add(item);
                db.SaveChanges();
            }
        }

        //Delete the item
        public static void DeleteItem(string ID)
        {
            using ( FilePropertiesContext db = new() )
            {
                var item = db.FileProperties.Find(ID);
                if ( item == null ) 
                    return;
                db.FileProperties.Remove(item);
                db.SaveChanges();
            }
        }
        //Update the price of the item
        public static void UpdateItem(FileProperty item)
        {
            using ( FilePropertiesContext db = new() )
            {
                db.FileProperties.Update(item);
                db.SaveChanges();
            }
        }
    }
}
