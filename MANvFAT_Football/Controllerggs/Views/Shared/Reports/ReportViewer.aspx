<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="MANvFAT_Football.Reports" %>
 
 

<%@ Register Assembly="Telerik.ReportViewer.WebForms, Version=9.0.15.324, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>
<%@ Register Assembly="Telerik.Reporting, Version=9.0.15.324, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" Namespace="Telerik.Reporting" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body >

    <script runat="server">
 public override void VerifyRenderingInServerForm(Control control)
  {
 // to avoid the server form (<form runat="server"> requirement
  }
 protected override void OnLoad(EventArgs e)
  {
 base.OnLoad(e);
      var instanceReportSource = new Telerik.Reporting.InstanceReportSource();
     //TODO: uncomment the following line and replace the Report Name with your Created Report name
     // instanceReportSource.ReportDocument = new rptApplicationsReceived(); 
      rptMyReportName.ReportSource = instanceReportSource;
  }
</script>

   
   <form id="main" method="post" action="">
 <telerik:ReportViewer ID="rptMyReportName" Width="100%" Height="900px" runat="server" ViewMode="PrintPreview">
 </telerik:ReportViewer>
</form>
       

</body>
</html>
