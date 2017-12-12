namespace menu.Models.EF
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class InspectionContext : DbContext
    {
        public InspectionContext()
            : base("name=InspectionContext")
        {
        }

        public virtual DbSet<ButtonLibrary> ButtonLibraries { get; set; }
        public virtual DbSet<WorkRoom> WorkRooms { get; set; }
        public virtual DbSet<WorkroomActivator> WorkroomActivators { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ButtonLibrary>()
                .Property(e => e.DefectImage_Desc)
                .IsUnicode(false);

            modelBuilder.Entity<WorkRoom>()
                .Property(e => e.Abbreviation)
                .IsUnicode(false);
        }
    }
}
