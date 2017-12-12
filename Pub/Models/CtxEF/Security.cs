namespace menu.Models.CtxEF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Security")]
    public partial class Security
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Security()
        {
            Maintenance_Flagboard = new HashSet<Maintenance_Flagboard>();
        }

        [Key]
        [StringLength(15)]
        public string UserID { get; set; }

        [StringLength(8)]
        public string Password { get; set; }

        public bool System_Design { get; set; }

        public bool System_User_Security { get; set; }

        public bool System_DB_Maintenance { get; set; }

        public bool System_DB_Modification { get; set; }

        public bool Utilization_Master { get; set; }

        public bool Utilization_Data_Entry { get; set; }

        public bool Utilization_View { get; set; }

        public bool Utilization_Reports { get; set; }

        public bool Utilization_Forms { get; set; }

        public bool Inventory_Data_Entry { get; set; }

        public bool Inventory_View { get; set; }

        public bool Inventory_Reports { get; set; }

        public bool Inventory_Forms { get; set; }

        public bool SRB_Master { get; set; }

        public bool SRB_Data_Entry { get; set; }

        public bool SRB_View { get; set; }

        public bool SRB_Reports { get; set; }

        public bool SRB_Forms { get; set; }

        public bool PM_Master { get; set; }

        public bool PM_Data_Entry { get; set; }

        public bool PM_View { get; set; }

        public bool PM_Reports { get; set; }

        public bool PM_Forms { get; set; }

        public bool Labor_Master { get; set; }

        public bool Labor_Data_Entry { get; set; }

        public bool Labor_View { get; set; }

        public bool Labor_Reports { get; set; }

        public bool Labor_Forms { get; set; }

        public bool Order_Data_Entry { get; set; }

        public bool Order_View { get; set; }

        public bool Order_Reports { get; set; }

        public bool Order_Forms { get; set; }

        public bool Garment_Master { get; set; }

        public bool Garment_Data_Entry { get; set; }

        public bool Garment_View { get; set; }

        public bool Garment_Reports { get; set; }

        public bool Garment_Forms { get; set; }

        public bool PackRoom_Master { get; set; }

        public bool PackRoom_Data_Entry { get; set; }

        public bool PackRoom_View { get; set; }

        public bool PackRoom_Reports { get; set; }

        public bool PackRoom_Forms { get; set; }

        public bool Handheld { get; set; }

        public bool Transportation_Master { get; set; }

        public bool Transportation_Data_Entry { get; set; }

        public bool Transportation_View { get; set; }

        public bool Transportation_Reports { get; set; }

        public bool Transportation_Forms { get; set; }

        public bool System_Accounting { get; set; }

        public bool CRM_Master { get; set; }

        public bool CRM_Entry { get; set; }

        public bool CRM_View { get; set; }

        public bool CRM_Reports { get; set; }

        public bool CRM_Forms { get; set; }

        public bool Facility_Access_All { get; set; }

        public bool Curtain_Master { get; set; }

        public bool Curtain_Entry { get; set; }

        public bool Curtain_View { get; set; }

        public bool Curtain_Reports { get; set; }

        [StringLength(50)]
        public string eMailAddress { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Maintenance_Flagboard> Maintenance_Flagboard { get; set; }
    }
}
