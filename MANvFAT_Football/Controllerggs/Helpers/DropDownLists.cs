using MANvFAT_Football.Models.Enumerations;
using MANvFAT_Football.Models.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MANvFAT_Football.Helpers
{
    /// <summary>
    /// Used to Generate Select List to pass from Controller to View to bind it to the Dropdown or ComboBox Controls
    /// </summary>
    public static class DropDownLists
    {
        public static List<SelectListItem> CreateBlankListItem(string Text)
        {
            SelectListItem it = new SelectListItem();
            it.Text = Text;
            it.Value = "";
            it.Selected = true;

            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(it);

            return list;
        }

        public static SelectList GetAllUsers(long? UserID)
        {
            List<SelectListItem> _ListOfObjects = new List<SelectListItem>();

            UsersRepository modelRepo = new UsersRepository();
            var admins = modelRepo.ReadAll().Where(m => m.Deleted != true).OrderBy(o => o.FullName);
            foreach (var item in admins)
            {
                SelectListItem itr = new SelectListItem();
                itr.Text = item.FullName;
                itr.Value = item.UserID.ToString();
                itr.Selected = false;

                _ListOfObjects.Add(itr);
            }

            return new SelectList(_ListOfObjects, "Value", "Text", UserID);
        }

        public static SelectList GetAdminUsers(long? UserID)
        {
            List<SelectListItem> _ListOfObjects = new List<SelectListItem>();

            UsersRepository modelRepo = new UsersRepository();
            var admins = modelRepo.ReadAll().Where(m => m.RoleID == (int)Permissions.Administrator && m.Deleted != true);
            foreach (var item in admins)
            {
                SelectListItem itr = new SelectListItem();
                itr.Text = item.FullName;
                itr.Value = item.UserID.ToString();
                itr.Selected = false;

                _ListOfObjects.Add(itr);
            }

            return new SelectList(_ListOfObjects, "Value", "Text", UserID);
        }

        public static SelectList GetCoachUsers(long? UserID)
        {
            List<SelectListItem> _ListOfObjects = new List<SelectListItem>();

            UsersRepository modelRepo = new UsersRepository();
            var coaches = modelRepo.ReadAll().Where(m => m.RoleID == (int)Permissions.Coaches && m.Deleted != true);
            foreach (var item in coaches)
            {
                SelectListItem itr = new SelectListItem();
                itr.Text = item.FullName;
                itr.Value = item.UserID.ToString();
                itr.Selected = false;

                _ListOfObjects.Add(itr);
            }

            return new SelectList(_ListOfObjects, "Value", "Text", UserID);
        }

        public static SelectList GetHeights(long? HeightID)
        {
            List<SelectListItem> _ListOfObjects = new List<SelectListItem>();

            _ListOfObjects.AddRange(CreateBlankListItem("-- Select your height --"));

            PlayersRepository modelRepo = new PlayersRepository();

            foreach (var item in modelRepo.ReadAll_Heights())
            {
                SelectListItem itr = new SelectListItem();
                itr.Text = item.Height_Display;
                itr.Value = item.HeightID.ToString();
                itr.Selected = false;

                _ListOfObjects.Add(itr);
            }

            return new SelectList(_ListOfObjects, "Value", "Text", HeightID);
        }

        public static SelectList GetHowActives(long? HowActiveID)
        {
            List<SelectListItem> _ListOfObjects = new List<SelectListItem>();

            PlayersRepository modelRepo = new PlayersRepository();
            var _HowActive = modelRepo.ReadAll_HowActive().OrderBy(o => o.HowActiveID);
            foreach (var item in _HowActive)
            {
                SelectListItem itr = new SelectListItem();
                itr.Text = item.HowActive;
                itr.Value = item.HowActiveID.ToString();
                itr.Selected = false;

                _ListOfObjects.Add(itr);
            }

            return new SelectList(_ListOfObjects, "Value", "Text", HowActiveID);
        }

        public static SelectList GetPaymentModes(string PaymentMode)
        {
            List<SelectListItem> _ListOfPaymentModes = new List<SelectListItem>();
            {
                YesNo pmodes = new YesNo();

                foreach (var item in pmodes.buildSandBoxLive())
                {
                    SelectListItem itr = new SelectListItem();
                    itr.Text = item.Text;
                    itr.Value = item.ValueStr.ToString();
                    itr.Selected = false;

                    _ListOfPaymentModes.Add(itr);
                }
            }
            return new SelectList(_ListOfPaymentModes.OrderBy(o => o.Text), "Value", "Text", PaymentMode);
        }

        public static SelectList GetAdvertisements(long? AdvertisementID)
        {
            List<SelectListItem> _ListOfObjects = new List<SelectListItem>();

            AdvertisementsRepository modelRepo = new AdvertisementsRepository();

            foreach (var item in modelRepo.ReadAll())
            {
                SelectListItem itr = new SelectListItem();
                itr.Text = item.Advertisement;
                itr.Value = item.AdvertisementID.ToString();
                itr.Selected = false;

                _ListOfObjects.Add(itr);
            }

            return new SelectList(_ListOfObjects, "Value", "Text", AdvertisementID);
        }
    }

    public class YesNo
    {
        public bool Value { get; set; }
        public string Text { get; set; }
        public string ValueStr { get; set; }

        public List<YesNo> buildYesNoNA()
        {
            List<YesNo> listofYesNoNA = new List<YesNo>();

            YesNo yes = new YesNo();
            yes.Text = "Yes";
            yes.ValueStr = "Yes";
            listofYesNoNA.Add(yes);

            YesNo no = new YesNo();
            no.Text = "No";
            no.ValueStr = "No";
            listofYesNoNA.Add(no);

            YesNo na = new YesNo();
            na.Text = "N/A";
            na.ValueStr = "N/A";
            listofYesNoNA.Add(na);

            return listofYesNoNA;
        }

        public List<YesNo> buildSandBoxLive()
        {
            List<YesNo> listofSandBoxLive = new List<YesNo>();

            YesNo Sandbox = new YesNo();
            Sandbox.Text = "Sandbox";
            Sandbox.ValueStr = "Sandbox";
            listofSandBoxLive.Add(Sandbox);

            YesNo Live = new YesNo();
            Live.Text = "Live";
            Live.ValueStr = "Live";
            listofSandBoxLive.Add(Live);

            return listofSandBoxLive;
        }
    }
}