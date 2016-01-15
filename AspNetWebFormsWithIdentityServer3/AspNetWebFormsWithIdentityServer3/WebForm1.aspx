<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="AspNetWebFormsWithIdentityServer3.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
            <%foreach (var claim in (User as System.Security.Claims.ClaimsPrincipal).Claims)
              {
                  Response.Write(claim + "<br>");
              } %>
    </div>
    </form>
</body>
</html>
