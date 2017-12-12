using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;

namespace menu.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class ManageIndexViewModel
    {
        public string NavRowSel { get; set; }
        public bool AlertsSelected { get; set; }
        public bool UtilitySelected { get; set; }
        public bool FlagboardsSelected { get; set; }
        public bool ActivitySelected { get; set; }
        public bool QASelected { get; set; }
        public QAPartialViewModel ChildViewModel { get; set; }
        public FbPartialViewModel FbChildViewModel { get; set; }
        public AlPartialViewModel AlChildViewModel { get; set; }
        public UtilityViewModel UlChildViewModel { get; set; }
    }

    public class MethodResponse
    {
        public bool Result { get; set; }
        public WorkroomsUserObject[] Content { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class WorkroomsUserObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public bool Status { get; set; }
    }

    public class jqdata
    {
        public object total { get; set; }
        public object page { get; set; }
        public object records { get; set; }
        public object rows { get; set; }
    }

    public class QAPartialViewModel
    {
        public EF.WorkRoom[] wrdata { get; set; }
        public EF.LocationMaster[] locdata { get; set; }
        public string WorkroomOptionsStr { get; set; }
    }

    public class FbPartialViewModel
    {
        public CtxEF.Maintenance_Flagboard[] fbdata { get; set; }
        public CtxEF.Corporate[] corpdata { get; set; }
        public string SelCID {get; set;}
    }

    public class AlPartialViewModel
    {
        public AlertManager[] aldata { get; set; }
        public string ErrorMessage { get; set; }
        public string LocSerOptions { get; set; }
    }

    public class UtilityViewModel
    {
        public string Key { get; set; }
        public string IframeUrl { get; set; }
    }

    public class AlertManager
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string CID { get; set; }
        public string Loc { get; set; }
        public string Email { get; set; }
        public bool INSCOMP { get; set; }
        public bool AUTOCOMP { get; set; }
        public bool INSCOMPEX { get; set; }
    }

    public class Option
    {
        public string value { get; set; }
        public string text { get; set; }
    }

    public class ApplicationsViewModel : ViewModelBase
    {
        public IList<Models.ButtonModel> Apps { get; set; }
        public string CID { get; set; }
        public string LocationName { get; set; }
    }
    public abstract class ViewModelBase
    {
        public string CorporateName { get; set; }
    }
    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}