namespace menu.Models.EF
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AprManager : DbContext
    {
        public AprManager()
            : base("name=AprManager")
        {
        }

        public virtual DbSet<AlertMaster> AlertMasters { get; set; }
        public virtual DbSet<EmailMaster> EmailMasters { get; set; }
        public virtual DbSet<LocationMaster> LocationMasters { get; set; }
        public virtual DbSet<ActiveAlert> ActiveAlerts { get; set; }
        public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AlertMaster>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<AlertMaster>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<AlertMaster>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<EmailMaster>()
                .Property(e => e.HomeLocation)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<LocationMaster>()
                .Property(e => e.Name)
                .IsFixedLength();

            modelBuilder.Entity<LocationMaster>()
                .Property(e => e.Abreviation)
                .IsUnicode(false);

            modelBuilder.Entity<LocationMaster>()
                .Property(e => e.DBname)
                .IsFixedLength();

            modelBuilder.Entity<LocationMaster>()
                .Property(e => e.CID)
                .IsFixedLength();

            modelBuilder.Entity<LocationMaster>()
                .Property(e => e.ConnectionString)
                .IsFixedLength();

            modelBuilder.Entity<LocationMaster>()
                .Property(e => e.CtxCID)
                .IsFixedLength();

            modelBuilder.Entity<ActiveAlert>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<ActiveAlert>()
                .Property(e => e.CID)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<ActiveAlert>()
                .Property(e => e.Value)
                .IsUnicode(false);

            modelBuilder.Entity<ActiveAlert>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<UserActivityLog>()
                .Property(e => e.DBOrigin)
                .IsUnicode(false);

            modelBuilder.Entity<UserActivityLog>()
                .Property(e => e.UserID)
                .IsUnicode(false);

            modelBuilder.Entity<UserActivityLog>()
                .Property(e => e.DeviceType)
                .IsUnicode(false);

            modelBuilder.Entity<UserActivityLog>()
                .Property(e => e.CID)
                .IsUnicode(false);
        }
    }
}
