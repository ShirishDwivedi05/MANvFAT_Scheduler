using MANvFAT_Football.Helpers;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System.Text;
using MANvFAT_Football.Models.Enumerations;
using System.Text.RegularExpressions;

namespace MANvFAT_Football.Models.Repositories
{
    public class UsersRepository : BaseRepository
    {
        public List<UsersExt> ReadAll()
        {
            return db.Users.Include("Roles").ToList().Select(m => Map(m)).ToList();
        }

        public UsersExt ReadOne(long UserID)
        {
            var task = db.Users.Include("Roles").Where(m => m.UserID == UserID).FirstOrDefault();
            return Map(task);
        }

        public UsersExt ReadOne(string Username)
        {
            var user = db.Users.Include("Roles").Where(m => m.EmailAddress.ToLower().Equals(Username)).FirstOrDefault();
            return Map(user);
        }

        public bool Create(UsersExt model, ref string Msg, Controller ctrl)
        {
            bool status = true;
            try
            {
                model.Mobile = model.Mobile.Replace(" ", string.Empty);
                if(ValidateMobileNumber(model.Mobile, ref Msg) ==false)
                {
                    status = false;
                    return status;
                }

                //TODO: Map to DB Object
                var dbmodel = Map(model);
                //TODO: Save DB Changes and Set the Return Primary Key ID
                db.Users.Add(dbmodel);
                db.SaveChanges();
                //TOD: Add to Audit Log
                AuditLog(ctrl, AuditAction.Create, ReadOne(dbmodel.UserID), null);
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }

            return status;
        }

        public bool Update(UsersExt model, ref string Msg, Controller ctrl)
        {
            //Wrap it all in a transaction
            bool status = true;
            try
            {
                model.Mobile = model.Mobile.Replace(" ", string.Empty);
                if (ValidateMobileNumber(model.Mobile, ref Msg) == false)
                {
                    status = false;
                    return status;
                }

                var dbmodel = db.Users.FirstOrDefault(p => p.UserID == model.UserID);
                var ForAuditLog = Map(dbmodel);
                //TODO: Map to DB Object
                MapUpdate(ref dbmodel, model);
                //TODO: Update DB Changes
                db.SaveChanges();

                //TOD: Add to Audit Log
                AuditLog(ctrl, AuditAction.Update, ForAuditLog, ReadOne(dbmodel.UserID));
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }

            return status;
        }

        public void UpdateUserMobileVerificationCode(long UserID, int MobileVerificationCode)
        {
            try
            {
                var dbmodel = db.Users.FirstOrDefault(p => p.UserID == UserID);
                if (dbmodel != null)
                {
                    dbmodel.MobileVerficationCode = MobileVerificationCode;
                    db.SaveChanges();

                    //TOD: Add to Audit Log
                    SecurityUtils.AddAuditLog("Mobile Verification Code Updated", "User = " + dbmodel.EmailAddress);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
            }
        }

        public void UpdateUserMobileVerification(long UserID, bool IsMobileNumVerified)
        {
            try
            {
                var dbmodel = db.Users.FirstOrDefault(p => p.UserID == UserID);
                if (dbmodel != null)
                {
                    dbmodel.IsMobileNumVerified = IsMobileNumVerified;
                    db.SaveChanges();

                    //TOD: Add to Audit Log
                    SecurityUtils.AddAuditLog("Mobile Verification Completed ", "set IsMobileNumVerified = true for User = " + dbmodel.EmailAddress);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
            }
        }

        public bool Delete(UsersExt model, ref string Msg, Controller ctrl)
        {
            bool status = true;
            try
            {
                //TODO: Get Current Object from DB
                var ItemToDelete = db.Users.FirstOrDefault(m => m.UserID == model.UserID);
                if (ItemToDelete != null)
                {
                   

                    var ForAudiLog = Map(ItemToDelete);
                    
                    //Delete from DB and any other dependencies
                    db.DeleteUser(model.UserID);

                    //Add To Log
                    AuditLog(ctrl, AuditAction.Delete, null, ForAudiLog);
                }
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);

                if (ex.InnerException.Message.Contains("Leagues"))
                {
                    Msg = "Selected User is Assigned as Admin/Coach for a league, Please replace the Admin/Coach for League to continue";
                }
                else if (ex.InnerException.Message.Contains("Teams"))
                {
                    Msg = "Selected User is Assigned as Coach for a Team, Please replace the Coach for such Team to continue";
                }

                status = false;
            }

            return status;
        }

        public bool ValidateMobileNumber(string MobileNumber, ref string ErrorMsg)
        {
            bool IsValid = true;

            if (string.IsNullOrEmpty(MobileNumber))
            {
                IsValid = false;
                ErrorMsg = "Please enter Mobile Number";
            }
            else if (MobileNumber.StartsWith("07") == false)
            {
                IsValid = false;
                ErrorMsg = "Mobile Number should start with 07";
            }
            else if (MobileNumber.Length != 11)
            {
                IsValid = false;
                ErrorMsg = "Mobile Number is Not valid.";
            }

            return IsValid;
        }

        public Users Map(UsersExt model)
        {
            Users tblModel = new Users()
            {
                UserID = model.UserID,
                FullName = model.FullName,
                EmailAddress = model.EmailAddress,
                Password = SecurityUtils.EncryptText(model.Password),
                Locked = model.Locked,
                Deleted = model.Deleted,
                RoleID = model.RoleID,
                DailyStatEmails = model.DailyStatEmails,
                DailyLiveLeagueNumEmails = model.DailyLiveLeagueNumEmails,
                Mobile = model.Mobile,
                Address = model.Address,
                MobileVerficationCode = model.MobileVerficationCode,
                IsMobileNumVerified = model.IsMobileNumVerified
            };

            return tblModel;
        }

        public UsersExt Map(Users model)
        {
            UsersExt tblModel = new UsersExt()
            {
                UserID = model.UserID,
                FullName = model.FullName,
                EmailAddress = model.EmailAddress,
                Password = SecurityUtils.DecryptCypher(model.Password),
                Locked = model.Locked,
                Deleted = model.Deleted,
                RoleID = model.RoleID,
                Role = model.Roles.Role,
                DailyStatEmails = model.DailyStatEmails,
                DailyLiveLeagueNumEmails = model.DailyLiveLeagueNumEmails,
                Mobile = model.Mobile.Replace(" ",string.Empty),
                Address = model.Address,
                MobileVerficationCode  = model.MobileVerficationCode,
                IsMobileNumVerified = model.IsMobileNumVerified
            };


            return tblModel;
        }

        public void MapUpdate(ref Users dbmodel, UsersExt model)
        {
            dbmodel.UserID = model.UserID;
            dbmodel.FullName = model.FullName;
            dbmodel.EmailAddress = model.EmailAddress;
            dbmodel.Password = SecurityUtils.EncryptText(model.Password);
            dbmodel.Locked = model.Locked;
            dbmodel.Deleted = model.Deleted;
            dbmodel.RoleID = model.RoleID;
            dbmodel.DailyStatEmails = model.DailyStatEmails;
            dbmodel.DailyLiveLeagueNumEmails = model.DailyLiveLeagueNumEmails;
            dbmodel.Mobile = model.Mobile;
            dbmodel.Address = model.Address;
        }

        /// <summary>
        /// Add the Action to Audit Log
        /// </summary>
        /// <param name="model">The Object for which this Auditlog took place</param>
        /// <param name="Action">"Added New User OR Updated User Details OR Deleted User"</param>

        private void AuditLog(Controller ctrl, AuditAction auditAction, UsersExt dbmodel = null, UsersExt model = null)
        {
            string AuditLogShortDesc = "", AuditLogLongDesc = "";

            //StringBuilder sb = new StringBuilder();

            if (auditAction == AuditAction.Create) //Creating new Record
            {
                AuditLogShortDesc = "New User has been Added";
                AuditLogLongDesc = "UserID: "+dbmodel.UserID+" FullName: " + dbmodel.FullName + " EmailAddress: " + dbmodel.EmailAddress + " Role: " + dbmodel.Role;
                //sb.Append("<table class='table table-stripped auditLogStyle'>");
                //sb.Append("<tr><th colspan='2'>New User has been Added</th></tr>");
                //sb.Append("<tr><th>Full Name</th><td>" + dbmodel.FullName + "</td></tr>");
                //sb.Append("<tr><th>Email</th><td>" + dbmodel.EmailAddress + "</td></tr>");
                //sb.Append("<tr><th>Role</th><td>" + dbmodel.Role + "</td></tr>");
                //sb.Append("<tr><th>Locked</th><td>" + dbmodel.Locked + "</td></tr>");
                //sb.Append("<tr><th>Deleted</th><td>" + dbmodel.Deleted + "</td></tr>");
                //sb.Append("</table>");

                //AuditLogLongDesc = sb.ToString();
            }
            else if (auditAction == AuditAction.Update)
            {
                AuditLogShortDesc = "User has been Updated";
                AuditLogLongDesc = "UserID: " + model.UserID + " FullName: " + model.FullName + " EmailAddress: " + model.EmailAddress + " Role: " + model.Role;
                //sb.Append("<table class='table table-stripped auditLogStyle'>");

                //sb.Append("<tr><th colspan='2'>User has been Updated</th></tr>");
                //sb.Append("<tr><th colspan='2'><label>BEFORE</label></th></tr>");

                //sb.Append("<tr><th>Full Name</th><td>" + dbmodel.FullName + "</td></tr>");
                //sb.Append("<tr><th>Email</th><td>" + dbmodel.EmailAddress + "</td></tr>");
                //sb.Append("<tr><th>Role</th><td>" + dbmodel.Role + "</td></tr>");
                //sb.Append("<tr><th>Locked</th><td>" + dbmodel.Locked + "</td></tr>");
                //sb.Append("<tr><th>Deleted</th><td>" + dbmodel.Deleted + "</td></tr>");


                //sb.Append("<tr><th colspan='2'><label>AFTER</label></th></tr>");

                //sb.Append("<tr><th>Full Name</th><td>" + dbmodel.FullName + "</td></tr>");
                //sb.Append("<tr><th>Email</th><td>" + dbmodel.EmailAddress + "</td></tr>");
                //sb.Append("<tr><th>Role</th><td>" + dbmodel.Role + "</td></tr>");
                //sb.Append("<tr><th>Locked</th><td>" + dbmodel.Locked + "</td></tr>");
                //sb.Append("<tr><th>Deleted</th><td>" + dbmodel.Deleted + "</td></tr>");

                //sb.Append("</table>");

                //AuditLogLongDesc = sb.ToString();
            }
            else if (auditAction == AuditAction.Delete)
            {
                AuditLogShortDesc = "User has been Deleted";
                AuditLogLongDesc = "UserID: " + model.UserID + " FullName: " + model.FullName + " EmailAddress: " + model.EmailAddress + " Role: " + model.Role;
                //sb.Append("<tr><th>Full Name</th><td>" + dbmodel.FullName + "</td></tr>");
                //sb.Append("<tr><th>Email</th><td>" + dbmodel.EmailAddress + "</td></tr>");
                //sb.Append("<tr><th>Role</th><td>" + dbmodel.Role + "</td></tr>");
                //sb.Append("<tr><th>Locked</th><td>" + dbmodel.Locked + "</td></tr>");
                //sb.Append("<tr><th>Deleted</th><td>" + dbmodel.Deleted + "</td></tr>");

                //sb.Append("</table>");

                //AuditLogLongDesc = sb.ToString();
            }

            SecurityUtils.AddAuditLog(AuditLogShortDesc, AuditLogLongDesc, ctrl);
        }
    }

    public class UsersExt
    {
        public long UserID { get; set; }
        [Required(ErrorMessage = "Full Name Required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email Required")]
        [EmailAddress(ErrorMessage = "Please enter valid email address")]
        public string EmailAddress { get; set; }
        [StringLength(20, ErrorMessage = "Minimum 6 characters required", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "at least one Capital letter and a Number required")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select User Role")]
        public long RoleID { get; set; }
        public string Role { get; set; }
        public string ClientName { get; set; }
        public string EmployeeName { get; set; }
        public bool Locked { get; set; }
        public bool Deleted { get; set; }
        public bool DailyStatEmails { get; set; }
        public bool DailyLiveLeagueNumEmails { get; set; }


        [Required(ErrorMessage = "Mobile number required")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Address required")]
        public string Address { get; set; }
        public Nullable<int> MobileVerficationCode { get; set; }
        public bool IsMobileNumVerified { get; set; }
        public UsersExt()
        {
            Locked = false;
            Deleted = false;
            DailyStatEmails = true;
            DailyLiveLeagueNumEmails = false;
        }

    }
}