namespace menu.Models.EF
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ActiveAlert
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(5)]
        public string Code { get; set; }

        [Column(Order = 1)]
        [StringLength(6)]
        public string CID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(30)]
        public string Value { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(30)]
        public string Name { get; set; }
    }
}
