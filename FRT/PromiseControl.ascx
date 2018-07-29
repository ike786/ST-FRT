<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PromiseControl.ascx.cs"
    Inherits="FRT.Promise" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <h2><asp:label ID="lblPromiseTitle" runat="server" /></h2>
    <p><asp:Label ID="lblPromiseDescription" runat="server" /></p>
        <div class="innerColumn">
            <h3>
                <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
                </asp:ScriptManagerProxy>
                This Week</h3>
            <asp:Repeater ID="rptThisWeek" runat="server" EnableTheming="True">
                <ItemTemplate>
                    <asp:Label ID="twPromiseID" runat="server" Text='<%# Bind("PromiseID") %>' Visible="False" />
                    <asp:TextBox ID="twPromiseDescription" runat="server" Text='<%# Bind("PromiseDescription") %>'
                        AutoPostBack="True" OnTextChanged="twPromiseDescription_TextChanged"  Rows="1" CssClass="promiseDescription" Font-Strikeout='<%# Eval("PromiseComplete") %>' 
                        style="text-decoration-color:gray; -moz-text-decoration-color:gray" />
                    <asp:CheckBox ID="twPromiseComplete" runat="server" AutoPostBack="true" OnCheckedChanged="twPromiseComplete_CheckedChanged" Checked='<%# Eval("PromiseComplete") %>' />
                </ItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="newPromiseDescription" runat="server" AutoPostBack="true" OnTextChanged="newPromiseDescription_TextChanged" Wrap="True" Rows="1" CssClass="promiseDescription" /></li>                    
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div class="innerColumn">
            <h3>
                Last Week</h3>
            <asp:Repeater ID="rptLastWeek" runat="server">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <asp:Label ID="lwPromiseDescription" runat="server" Text='<%# Eval("PromiseDescription") %>' Width="100%" Font-Strikeout='<%# Eval("PromiseComplete") %>' /></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul></FooterTemplate>
            </asp:Repeater>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
