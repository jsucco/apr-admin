namespace menu.Models.EF
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("WorkroomActivator")]
    public partial class WorkroomActivator
    {
        public int Id { get; set; }

        public int LocationMasterId { get; set; }

        public int WorkroomId { get; set; }

        public bool Status { get; set; }

        public DateTime Inserted_Timestamp { get; set; }
    }
}
