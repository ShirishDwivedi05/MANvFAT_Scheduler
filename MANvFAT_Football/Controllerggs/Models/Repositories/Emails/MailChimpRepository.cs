using MailChimp;
using MailChimp.Types;
using MANvFAT_Football.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MANvFAT_Football.Models.Repositories
{
    public class MailChimpRepository
    {
        //*** USMAN TESTING MAILCHIMP ACOUNT ***//
        //public const string apiKey = "2da59cc0c0d5ad4ad5f5c1c359ef0884-us13";
        //public const string listId = "2f5ace46b7";

        //*** MANvFAT Football LIVE MAILCHIMP ACCOUNT ***//
        public const string apiKey = "441fa232aa21d2573e800a7c87c9424c-us3";
        public const string listId = "f9b5e5386d"; //List Name = "MVF Football"
        public const string NonPaidPlayers_ListId = "d826ea114f"; //List Name = MVF Football No-Payment
        public const string MANvFATOfficialMembers_ListId = "b5df764894"; //List Name = MAN v FAT Official Members

        public const string Testing_ListId = "325d4d438f"; //We'll use this list id to test Mailchimp

        public bool Subscribe(PlayersExt model)
        {
            bool status = true;

            try
            {
                var mcApi = new MCApi(apiKey, false);

                //var lists = mcApi.Lists();

                //foreach (var item in lists.Data)
                //{
                //    string listID = item.ListID;
                //}

                List<List.Merges> SubscriptionInfo = new List<List.Merges>();

                var SubscriptionOptions = new MailChimp.Types.List.SubscribeOptions();
                SubscriptionOptions.DoubleOptIn = false;
                SubscriptionOptions.ReplaceInterests = false;
                SubscriptionOptions.EmailType = List.EmailType.Html;
                SubscriptionOptions.SendWelcome = false;
                SubscriptionOptions.UpdateExisting = true;

                var merges = GetListOfMerges(model);

                var returnvalue = mcApi.ListSubscribe(listId, model.EmailAddress, merges, SubscriptionOptions);
            }
            catch (Exception ex)
            {
                if (IgnoreMailchimpErrors(ex.Message))
                { }
                else
                {
                    ErrorHandling.HandleException(ex);
                    status = false;
                }
            }

            return status;
        }

        public bool Subscribe_NonPaidPlayers(PlayersExt model)
        {
            bool status = true;

            try
            {
                var mcApi = new MCApi(apiKey, false);

                //var lists = mcApi.Lists();

                //foreach (var item in lists.Data)
                //{
                //    string listID = item.ListID;
                //}

                List<List.Merges> SubscriptionInfo = new List<List.Merges>();

                var SubscriptionOptions = new MailChimp.Types.List.SubscribeOptions();
                SubscriptionOptions.DoubleOptIn = false;
                SubscriptionOptions.ReplaceInterests = false;
                SubscriptionOptions.EmailType = List.EmailType.Html;
                SubscriptionOptions.SendWelcome = false;
                SubscriptionOptions.UpdateExisting = true;

                var merges = GetListOfMerges(model);

                var returnvalue = mcApi.ListSubscribe(NonPaidPlayers_ListId, model.EmailAddress, merges, SubscriptionOptions);
            }
            catch (Exception ex)
            {
                if (IgnoreMailchimpErrors(ex.Message))
                { }
                else
                {
                    ErrorHandling.HandleException(ex);
                    status = false;
                }
            }

            return status;
        }

        //Footer Signup 
        public bool Subscribe_SingleSignUp(string EmailAddress)
        {
            bool status = true;

            try
            {
                var mcApi = new MCApi(apiKey, false);

                List<List.Merges> SubscriptionInfo = new List<List.Merges>();

                var SubscriptionOptions = new MailChimp.Types.List.SubscribeOptions();
                SubscriptionOptions.DoubleOptIn = false;
                SubscriptionOptions.ReplaceInterests = false;
                SubscriptionOptions.EmailType = List.EmailType.Html;
                SubscriptionOptions.SendWelcome = false;
                SubscriptionOptions.UpdateExisting = true;

                var merges = GetListOfMerges_SingleSignUp(EmailAddress);

                var returnvalue = mcApi.ListSubscribe(MANvFATOfficialMembers_ListId, EmailAddress, merges, SubscriptionOptions);
            }
            catch (Exception ex)
            {
                if (IgnoreMailchimpErrors(ex.Message))
                { }
                else
                {
                    ErrorHandling.HandleException(ex);
                    status = false;
                }
            }

            return status;
        }
        public bool UnSubscribe(string EmailAddress)

        {
            bool status = true;

            try
            {
                var mcApi = new MCApi(apiKey, false);

                var unsuboptions = new List.UnsubscribeOptions();
                unsuboptions.DeleteMember = true;
                unsuboptions.SendGoodby = false;
                unsuboptions.SendNotify = false;

                var batchUnSubscribe = mcApi.ListUnsubscribe(listId, EmailAddress, unsuboptions);
            }
            catch (Exception ex)
            {
                if (IgnoreMailchimpErrors(ex.Message))
                { }
                else
                {
                    ErrorHandling.HandleException(ex);
                    status = false;
                }
            }

            return status;
        }

        public bool UpdateSubscriber(PlayersExt model)
        {
            bool status = true;

            try
            {
                var mcApi = new MCApi(apiKey, false);
                var merges = GetListOfMerges(model);

                var returnval = mcApi.ListUpdateMember(listId, model.EmailAddress, merges, List.EmailType.Html, true);
            }
            catch (Exception ex)
            {
                //If try to update and didn't found any record so it means it is not already Subscribed.
                //in this case Subscribe the player
                if (IgnoreMailchimpErrors(ex.Message))
                {
                    status = Subscribe(model);
                }
                else
                {
                    ErrorHandling.HandleException(ex);
                    status = false;
                }
            }

            return status;
        }

        public bool SyncMailChimp_FromMVFFDB()
        {
            bool status = true;

            try
            {
                PlayersRepository pRepo = new PlayersRepository();
                var players = pRepo.ReadAll().Take(10);

                foreach (var item in players)
                {
                    status = UpdateSubscriber(item);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
                status = false;
            }
          

            return status;

        }

        public bool IgnoreMailchimpErrors(string ErrorMsg)
        {
            if (ErrorMsg.ToLower().Contains("there is no record") ||
                  ErrorMsg.ToLower().Contains("215") ||
                  ErrorMsg.ToLower().Contains("232") ||
                  ErrorMsg.ToLower().Contains("212") ||
                  ErrorMsg.ToLower().Contains("213"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public List.Merges GetListOfMerges(PlayersExt model)
        {
            var merge = new List.Merges(model.EmailAddress, List.EmailType.Html)
                    {
                        {"FNAME", model.FirstName},
                        {"LNAME", model.LastName},
                        {"PHONE", model.Mobile},
                        {"DOB", model.DOB},
                        {"MOBNUM", model.Mobile},
                        {"REGDATE", model.RegistrationDate},
                        {"WEIGHT", model.Weight},
                        {"HEIGHT", model.Height},
                        {"AVGACTLEV", model.HowActiveAreYou},
                        {"POSITION", model.PrefferredPosition},
                        {"BMI", model.BMI},
                        {"BODYFAT", model.BodyFat},
                        {"ADDLINE1", model.AddressLine1},
                        {"ADDLINE2", model.AddressLine2},
                        {"CITY", model.City},
                        {"POSTCODE", model.PostCode},
                        {"LEAGUE", model.SelectedLeagueName},

                    };

            return merge;
        }

        public List.Merges GetListOfMerges_SingleSignUp(string EmailAddress)
        {
            var merge = new List.Merges(EmailAddress, List.EmailType.Html)
                    {
                        {"SOURCE", "MANvFAT Footer Signup"}
                    };

            return merge;
        }
    }
}