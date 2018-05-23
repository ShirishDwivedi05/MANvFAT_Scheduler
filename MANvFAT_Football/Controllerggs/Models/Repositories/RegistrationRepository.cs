using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MANvFAT_Football.Models.Repositories
{
    public class RegistrationRepository
    {
     

    }

    public class RegistrationViewModel
    {
        RegistrationExt model = null;
        public RegistrationExt NewRegistration(Controller ctrl, RegistrationExt RegExt)
        {
            model = RegExt;
            if (model == null)
                model = new RegistrationExt();

            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            bool? RegSuccess = null;

            if (ctrl.Request.QueryString["Reg"] != null)
            {
                RegSuccess = true;

                //When Registration Success then Delete the Cookie and Remove all Sessions

                ctrl.Session.RemoveAll();

                ctrl.Session.Clear();
            }
            else
            {
                #region Initialize Viewmodels

                ctrl.ViewBag.Heights = DropDownLists.GetHeights(model.HeightID);
                ctrl.ViewBag.HowActives = DropDownLists.GetHowActives(model.HowActiveID);
                ctrl.ViewBag.Advertisements = DropDownLists.GetAdvertisements(model.AdvertisementID);

                #endregion
            }

            ctrl.ViewBag.RegSuccess = RegSuccess;

            return model;
        }
    }

    public class RegistrationExt
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
       
        public long HeightID { get; set; }
        public long HowActiveID { get; set; }
        public long PositionID { get; set; }
        public long AdvertisementID { get; set; }
        public long SelectedLeagueID { get; set; }
        public long Player_PlannedLeagueID { get; set; }

        [Required(ErrorMessage = "Please select Date Of Birth")]
        [DisplayName("DOB")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DOB { get; set; }
        [DisplayName("Weight (KG)")]
        [Required(ErrorMessage = "Please enter Weight")]
        public decimal Weight { get; set; }
        [Required(ErrorMessage = "Please enter Email Address")]
        //[EmailAddress(ErrorMessage = "Please enter Valid Email Address")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,8}$", ErrorMessage = "Please enter Valid Email Address")]
        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }
        public string Mobile { get; set; }
        public string PostCode { get; set; }
        public bool AdvertisementOther { get; set; }
        public string AdvertisementOtherDetails { get; set; }
        public string Notes { get; set; }
        public decimal RegBMI { get; set; }

        //For Emergency Details
        public long PlayerID { get; set; }
        public string FullName { get; set; }
        public string Emergency_ContactName { get; set; }
        public string Emergency_ContactPhone { get; set; }
        [AllowHtml]
        public string Emergency_Medication { get; set; }
        public string ReferralCode { get; set; }
        public string ReferralPageLink { get; set; }

        public long? PreSelectedLeagueID { get; set; }
        public string PreSelectedLeagueName { get; set; }
        public bool IsLiveLeague { get; set; }
        public bool IsPlannedLeague { get; set; }
        public bool IsFreeLeague { get; set; }
        public string RegFeeWithBook { get; set; }
        public string RegFeeWithoutBook { get; set; }
        public RegistrationExt()
        {
           
        }
    }

    public class PayNowViewModel
    {
        public PayNowExt PayNow { get; set; }

        public PayNowViewModel(string PayLinkID, string ReferralCode, bool IsClaimFreePlace_Payment)
        {
            PayNow = new PayNowExt();

            PayNow.ReferralCode = ReferralCode;
            PayNow.IsClaimFreePlace_Payment = IsClaimFreePlace_Payment;
        }
    }
    public class PayNowExt
    { 
        public string ReferralCode { get; set; }
        public bool IsClaimFreePlace_Payment { get; set; }

    }
}