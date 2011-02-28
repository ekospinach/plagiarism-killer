<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
  <% using (Html.BeginForm("Index", "Home", FormMethod.Post, new { enctype = "multipart/form-data" }))
     {%>
            <p>
                <label>上传论文：</label>               
            </p>
            <input type="file" name="FileUpload1" /><br />
            <input type="submit" name="Submit" id="Submit" value="上传" />
    <% } %>
</asp:Content>
