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

               // ctrl.ViewBag.Heights = DropDownLists.GetHeights(model.HeightID);
               // ctrl.ViewBag.Advertisements = DropDownLists.GetAdvertisements(model.AdvertisementID);

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
        [Required(ErrorMessage = "Please enter Email Address")]
        //[EmailAddress(ErrorMessage = "Please enter Valid Email Address")]
       // [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,8}$", ErrorMessage = "Please enter Valid Email Address")]
        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }

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