<%@ Page Title="Staff Budgets" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Budgets.aspx.cs" Inherits="FRT.Budgets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Button ID="Save1" runat="server" Text="Save Changes" 
        onclick="Save1_Click" />
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:DropDownList ID="dropdownMonth" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropdownMonth_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:Repeater ID="rptBudget" runat="server">
                <HeaderTemplate>
                    <table>
                        <tr>
                            <th>
                                Staff Member
                            </th>
                            <th>
                                Timesheet
                            </th>
                            <th>
                                Invoice
                            </th>
                            <th>
                                Write-off
                            </th>
                            <th>
                                WIP
                            </th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Label ID="lblPersonName" runat="server" Text='<%# Bind("Name") %>' />
                            <asp:Label ID="lblPersonCode" runat="server" Text='<%# Bind("PersonCode") %>' Visible="false" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtTimesheet" runat="server" Text='<%# Bind("TimesheetDollarsBudget","{0:F2}") %>'
                               OnTextChanged="rptBudget_TextChanged" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtInvoice" runat="server" Text='<%# Bind("InvoiceDollarsBudget","{0:F2}") %>'
                                OnTextChanged="rptBudget_TextChanged" TabIndex="10" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtWOF" runat="server" Text='<%# Bind("WriteOffDollarsBudget","{0:F2}") %>'
                                OnTextChanged="rptBudget_TextChanged" TabIndex="20" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtWIP" runat="server" Text='<%# Bind("WIPDollarsBudget","{0:F2}") %>'
                                OnTextChanged="rptBudget_TextChanged" TabIndex="30" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table></FooterTemplate>
            </asp:Repeater>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="Save2" runat="server" Text="Save Changes" 
        onclick="Save2_Click" />
</asp:Content>
