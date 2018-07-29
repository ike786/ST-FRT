<%@ Page Title="WIP by Employee Report" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeBehind="WIPbyEmployee.aspx.cs" Inherits="FRT.WIP" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Src="PromiseControl.ascx" TagName="PromiseControl" TagPrefix="FRT" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:ScriptManager ID="ScriptManager2" runat="server">
    </asp:ScriptManager>
    <h1>
        WIP by Employee
    </h1>
    <p>
        Month starting
        <asp:DropDownList ID="dropdownMonthWIP" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropdownMonthWIP_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        Show hidden staff?
        <asp:CheckBox ID="showHiddenStaff" runat="server" AutoPostBack="true" OnCheckedChanged="showHiddenStaff_CheckedChanged" />
        <br />
        Graph shows
        <asp:DropDownList ID="graphStat" runat="server" AutoPostBack="true" OnTextChanged="graphStat_TextChanged">
        </asp:DropDownList>
    </p>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <div class="widecolumn first">
                <asp:Repeater ID="fvWIP" runat="server">
                    <HeaderTemplate>
                        <table class="financialKPIs">
                            <tr>
                                <th>
                                    Staff Member
                                </th>
                                <th>
                                    Actual
                                </th>
                                <th>
                                    Budget
                                </th>
                                <th>
                                    Graph
                                </th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="kpi">
                                <asp:Label runat="server" ID="PersonNameLabel" Text='<%# Eval("Name") %>' />
                            </td>
                            <td align="right">
                                <asp:Label ID="WIPDollarsLabel" runat="server" Text='<%# String.Format("{0:C0}", Eval("WIPDollars")) %>' />
                            </td>
                            <td align="right">
                                <asp:Label ID="WIPDollarsBudgetLabel" runat="server" Text='<%# String.Format("{0:C0}", Eval("WIPDollarsBudget")) %>' />
                            </td>
                            <td class="widegraph">
                                <asp:Panel ID="WIPGraph" CssClass="graph" runat="server">
                                    <asp:Label runat="server" ID="lblWIPGraph" CssClass="lightText"></asp:Label></asp:Panel>
                                <asp:Label runat="server" ID="lblWIPGraphOutside" CssClass="darkText"></asp:Label>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr class="shaded">
                            <td class="kpi">
                                <asp:Label runat="server" ID="PersonNameLabel" Text='<%# Eval("Name") %>' />
                            </td>
                            <td align="right">
                                <asp:Label ID="WIPDollarsLabel" runat="server" Text='<%# String.Format("{0:C0}", Eval("WIPDollars")) %>' />
                            </td>
                            <td align="right">
                                <asp:Label ID="WIPDollarsBudgetLabel" runat="server" Text='<%# String.Format("{0:C0}", Eval("WIPDollarsBudget")) %>' />
                            </td>
                            <td class="widegraph">
                                <asp:Panel ID="WIPGraph" CssClass="graph" runat="server">
                                    <asp:Label runat="server" ID="lblWIPGraph" CssClass="lightText"></asp:Label></asp:Panel>
                                <asp:Label runat="server" ID="lblWIPGraphOutside" CssClass="darkText"></asp:Label>
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
