using System.Web.Optimization;

namespace MANvFAT_Football
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            IItemTransform cssFixer = new CssRewriteUrlTransform();

            #region Back-end bundles

            bundles.Add(new ScriptBundle("~/bundles/jquery-backend").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/css-kendo-backend")
                    .Include("~/Content/kendo/2017.3.1026/kendo.common.min.css", cssFixer)
                    .Include("~/Content/kendo/2017.3.1026/kendo.mobile.all.min.css")
                    .Include("~/Content/kendo/2017.3.1026/kendo.dataviz.min.css")
                    .Include("~/Content/kendo/2017.3.1026/kendo.default.min.css", cssFixer)
                    .Include("~/Content/kendo/2017.3.1026/kendo.dataviz.default.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/js-kendo-backend").Include(
                                 "~/Scripts/kendo/2017.3.1026/jquery.min.js",
                                 "~/Scripts/kendo/2017.3.1026/jszip.min.js",
                                 "~/Scripts/kendo/2017.3.1026/kendo.all.min.js",
                                 "~/Scripts/kendo/2017.3.1026/kendo.aspnetmvc.min.js",
                                 "~/Scripts/kendo.modernizr.custom.js",
                                 "~/Scripts/kendo/2017.3.1026/kendo.culture.en-GB.js",
                                 "~/Scripts/jquery.maskedinput.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js-custom-backend").Include(
                             "~/Scripts/GlobalValidations.js",
                             "~/Scripts/jquery.maskedinput.min.js",
                             "~/Scripts/bootstrap-dialog-override.js"));

            bundles.Add(new StyleBundle("~/Content/css-custom-backend").Include(
              "~/Content/style.css", cssFixer));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //Player Views
            bundles.Add(new ScriptBundle("~/bundles/LinkedProfile-js").Include("~/Scripts/views_js/Players/LinkedProfile.js"));
            bundles.Add(new ScriptBundle("~/bundles/Players_Details-js").Include("~/Scripts/views_js/Players/Players_Details.js"));

            //League Views
            bundles.Add(new ScriptBundle("~/bundles/Leagues-Index-js").Include("~/Scripts/views_js/Leagues/Leagues_Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/Leagues-Details-js").Include("~/Scripts/views_js/Leagues/Leagues_Details.js"));
            bundles.Add(new ScriptBundle("~/bundles/League_tabFixtures-js").Include("~/Scripts/views_js/Leagues/League_tabFixtures.js"));
            bundles.Add(new ScriptBundle("~/bundles/League_tabPlayerWeightWeek-js").Include("~/Scripts/views_js/Leagues/League_tabPlayerWeightWeek.js"));
            #endregion Back-end bundles

            #region Common Bundles

            bundles.Add(new StyleBundle("~/Content/css-bootstrap")
                .Include("~/Content/bootstrap.css", cssFixer)
               .Include("~/Content/bootstrap-dialog.min.css", cssFixer)
                .Include("~/Content/bootstrapToggle/css/bootstrap-toggle.min.css", cssFixer));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                   "~/Scripts/bootstrap.js",
                   "~/Scripts/respond.js",
                   "~/Scripts/bootstrap-waitingfor.js",
                   "~/Scripts/bootstrap-dialog.min.js",
                   "~/Content/bootstrapToggle/js/bootstrap-toggle.min.js"));

            //Individual View Js
            bundles.Add(new ScriptBundle("~/bundles/Alerts-Index-js").Include("~/Scripts/views_js/Alerts_Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/Leagues-FixtureByTag-js").Include("~/Scripts/views_js/Leagues/Leagues_FixtureByTag.js"));

            
            bundles.Add(new ScriptBundle("~/bundles/Alerts_Global-js").Include("~/Scripts/views_js/Alerts_Global.js"));
            bundles.Add(new ScriptBundle("~/bundles/Grid_Resizer-js").Include("~/Scripts/views_js/Grid_Resizer.js"));
            bundles.Add(new ScriptBundle("~/bundles/PlayersArchive_Common-js").Include("~/Scripts/views_js/PlayersArchive/common.js"));
            bundles.Add(new ScriptBundle("~/bundles/Documents_Details-js").Include("~/Scripts/views_js/Documents_Details.js"));
            // bundles.Add(new ScriptBundle("~/bundles/DownForMaintenance-js").Include("~/Scripts/views_js/DownForMaintenance.js"));
            bundles.Add(new ScriptBundle("~/bundles/tab-PlayerImages-js").Include("~/Scripts/views_js/tab_PlayerImages.js"));
            bundles.Add(new ScriptBundle("~/bundles/tab-PlayerImages-FrontEnd-js").Include("~/Scripts/views_js/tab_PlayerImages_FrontEnd.js"));
            #endregion Common Bundles

            #region FrontEnd Bundles

            bundles.Add(new StyleBundle("~/Content/css-custom-frontend")
                .Include("~/Content/FrontEnd/css/style.css", cssFixer)
                .Include("~/Content/FrontEnd/assets/css/mvffootball.css", cssFixer)
                .Include("~/Content/FrontEnd/assets/css/kendo_ui_override.css", cssFixer)
                .Include("~/Content/FrontEnd/css/jquery-ui.css", cssFixer));

            bundles.Add(new ScriptBundle("~/bundles/js-main-frontend").Include(
                      "~/Content/FrontEnd/js/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js-frontend").Include(
                         "~/Content/FrontEnd/js/owl.carousel.min.js",
                         "~/Content/FrontEnd/js/jquery.ba-outside-events.min.js",
                         "~/Content/FrontEnd/js/tab.js",
                         "~/Content/FrontEnd/js/bootstrap-datepicker.js",
                         "~/Content/FrontEnd/twitter/jquery.tweet.min.js",
                         "~/Content/FrontEnd/js/jquery.timelinr-0.9.6.js",
                         "~/Content/FrontEnd/js/scripts.js",
                         "~/Scripts/jquery.maskedinput.min.js",
                         "~/Scripts/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/js-custom-frontend").Include(
                            "~/Scripts/GlobalValidations.js",
                            "~/Scripts/views_js/MVF_PayNow.js",
                             "~/Scripts/bootstrap-dialog-override.js"));

            bundles.Add(new StyleBundle("~/Content/css-kendo-frontend")
                .Include("~/Content/kendo/2017.3.1026/kendo.common.min.css", cssFixer)
                .Include("~/Content/kendo/2017.3.1026/kendo.default.min.css", cssFixer));

            bundles.Add(new ScriptBundle("~/bundles/js-kendo-frontend").Include(
                       "~/Scripts/kendo/2017.3.1026/kendo.all.min.js",
                        "~/Scripts/kendo/2017.3.1026/kendo.aspnetmvc.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Html5Gallery-js").Include("~/Content/FrontEnd/html5gallery/html5gallery.js"));


            bundles.Add(new ScriptBundle("~/bundles/EmergencyContactDetails-js").Include("~/Scripts/views_js/EmergencyContactDetails.js"));



            #endregion FrontEnd Bundles

            #region Progress Dashboard Bundles

            bundles.Add(new ScriptBundle("~/bundles/ProgressDashboard-js").Include(
                //"~/Scripts/date.js",
                "~/Scripts/moment.min.js",
                "~/Scripts/views_js/Dashboard/DailyActivities.js",
                "~/Scripts/views_js/Dashboard/WeeklyActivities.js",
                "~/Scripts/views_js/MobileDetection.js",
                "~/Scripts/views_js/Dashboard/Notifications.js"
                ));

            bundles.Add(new StyleBundle("~/Content/ProgressDashboard-css")
                    .Include("~/Content/FrontEnd/charts/circle.css", cssFixer)
                  //  .Include("~/Content/FrontEnd/css/PremiumDashboard.css", cssFixer)
                    );

            bundles.Add(new ScriptBundle("~/bundles/ProgressDashboard-Settings-js").Include(
               "~/Content/password_strength_meter/password.js",
               "~/Scripts/views_js/Dashboard/DashboardSettings.js",
               "~/Scripts/views_js/Dashboard/Dashboard_Common.js",
                 "~/Content/bootstrapSwitch/js/bootstrap-switch.min.js"

               ));

            bundles.Add(new StyleBundle("~/Content/ProgressDashboard-Settings-css")
                    .Include("~/Content/password_strength_meter/password.css", cssFixer)
                        .Include("~/Content/bootstrapSwitch/css/bootstrap-switch.min.css", cssFixer)
                    );
            #endregion


        }
    }
}