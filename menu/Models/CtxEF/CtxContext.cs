namespace menu.Models.CtxEF
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CtxContext : DbContext
    {
        public CtxContext()
            : base("name=CtxContext")
        {
        }

        public CtxContext(string sConnectionString)
            : base(sConnectionString)
        {
        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Maintenance_Flagboard> Maintenance_Flagboard { get; set; }
        public virtual DbSet<Maintenance_Schedule> Maintenance_Schedule { get; set; }
        public virtual DbSet<Security> Securities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .Property(e => e.EMP_Pay_Rate)
                .HasPrecision(19, 4);
        }
    }
}
