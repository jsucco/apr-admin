namespace menu.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ButtonLibrary")]
    public partial class ButtonLibrary
    {
        [Key]
        public int ButtonId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Text { get; set; }

        public byte[] DefectImage { get; set; }

        public string DefectImage_Desc { get; set; }

        [StringLength(10)]
        public string DefectCode { get; set; }

        public bool Hide { get; set; }
    }
}
