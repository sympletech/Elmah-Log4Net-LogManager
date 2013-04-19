<%@ Page Title="Log Manager Example App" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="LogManager.ExampleMonitoredApplication._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Installed NuGet Packages

    </h2>
    <ul>
        <li>Elmah (http://code.google.com/p/elmah/)</li>
        <li>Log4Net (http://logging.apache.org/log4net/)</li>
        <li>ELMAH Appender for log4net (https://github.com/edwinf/log4net---ELMAH-Appender)</li>
    </ul>
    
    <p>See Web Config for configuration settings</p>
    
    <center>
        <button onclick="window.open('404.aspx')">Spawn A 404 Error</button>
        <asp:Button ID="Button1" runat="server" Text="Throw An Uncaught Exception" OnClick="Button1_OnClick" />
        <asp:Button ID="Button2" runat="server" Text="Throw An Exception Caught and Logged" OnClick="Button2_OnClick" />
    </center>

</asp:Content>
