<%@ Page Title="FRT - Home" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="FRT._Default" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Src="PromiseControl.ascx" TagName="PromiseControl" TagPrefix="FRT" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    <script language="javascript">
        function ValidateLink(textControl) {
        /* see if the link is enclosed in an <a> tag and strip */
            if (textControl.value.substr(0, 9) == "<a href=\"") {
                working = textControl.value.substr(9, textControl.length);
                working = working.substr(0, working.search("\""));
                textControl.value = working;
            }
        }                                                   
    </script>

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlTrainingGoals" runat="server" Visible="false">
                <div class="mask">
                </div>
                <div class="trainingPopup">
                    <h2>
                        Your Training Goals</h2>
                    <p>
                        Please fill in details of the training goals discussed during your latest staff
                        review.</p>
                    <asp:Repeater runat="server" ID="rptTrainingGoals">
                        <HeaderTemplate>
                            <table>
                                <tr>
                                    <th>
                                        Goal
                                    </th>
                                    <th>
                                        Link URL (optional)
                                    </th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:HiddenField runat="server" ID="lblGoalID" Value='<%# Bind("GoalID") %>' />
                                    <asp:TextBox runat="server" ID="txtGoal" Text='<%# Bind("Goal") %>' CssClass="goal" />
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtGoalURL" Text='<%# Bind("URL") %>' CssClass="linkURL" onchange="javascript: ValidateLink(this);"/>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            <tr>
                                <td>
                                    <asp:TextBox runat="server" ID="newGoal" CssClass="goal" AutoPostBack="true" OnTextChanged="newGoal" />
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="newGoalURL" CssClass="linkURL" AutoPostBack="true"
                                        OnTextChanged="newGoal" onchange="javascript: ValidateLink(this);"/>
                            </tr>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <p />
                    <asp:Button ID="SaveGoals" runat="server" Text="Save and Close" OnClick="SaveGoals_Click" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="column first">
        <h1>Home</h1>
        <br />
        Staff Member:
        <asp:DropDownList CssClass="staffdropdown" ID="dropdownPerson" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropdownPerson_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        <br />
        Week Starting:
        <asp:DropDownList CssClass="weekdropdown" ID="dropdownWeek" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropdownWeek_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        <br />
        <asp:Panel ID="pnlFinancialKPIs" runat="server" CssClass="pagesection">
            <h2>Financial KPIs</h2>
            <p>How are you travelling this month versus your budget?</p>
            <asp:FormView ID="fvFinancialKPIsMonth" runat="server">
                <ItemTemplate>
                    <table class="financialKPIs">
                        <tr>
                            <th>
                                KPI
                            </th>
                            <th>
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
                        <tr class="shaded">
                            <td class="kpi">
                                Time timesheeted
                            </td>
                            <td class="infoicon">
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/info.gif" ToolTip="'Time timesheeted' counts all time put on the clock, before write-ups and write-offs are applied."
                                    CssClass="infoicon" />
                            </td>
                            <td align="right">
                                <asp:Label ID="TimesheetDollarsLabel" runat="server" Text='<%# String.Format("{0:C0}", Eval("TimesheetDollars")) %>' />
                            </td>
                            <td align="right">
                                <asp:Label ID="TimesheetDollarsBudgetLabel" runat="server" Text='<%# String.Format("{0:C0}", Eval("TimesheetDollarsBudget")) %>' />
                            </td>
                            <td class="graph">
                                <asp:Panel ID="TimesheetGraph" CssClass="graph" runat="server">
                                    <asp:Label runat="server" ID="lblTimesheetGraph" CssClass="lightText"></asp:Label></asp:Panel>
                                <asp:Label runat="server" ID="lblTimesheetGraphOutside" CssClass="darkText"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="kpi">
                                Time invoiced
                            </td>
                            <td class="infoicon">
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/info.gif" ToolTip="'Time invoiced' counts time that has been invoiced but does not take into account firm discounts. The figure includes non-finalised invoices."
                                    CssClass="infoicon" />
                            </td>
                            <td align="right">
                                <asp:Label ID="InvoiceDollarsLabel" runat="server" Text='<%# String.Format("{0:C0}", Eval("InvoiceDollars")) %>' />
                            </td>
                            <td align="right">
                                <asp:Label ID="InvoiceDollarsBudgetLabel" runat="server" Text='<%# String.Format("{0:C0}", Eval("InvoiceDollarsBudget")) %>' />
                            </td>
                            <td class="graph">
                                <asp:Panel ID="InvoiceGraph" CssClass="graph" runat="server">
                                    <asp:Label runat="server" ID="lblInvoiceGraph" CssClass="lightText"></asp:Label></asp:Panel>
                                <asp:Label runat="server" ID="lblInvoiceGraphOutside" CssClass="darkText"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="kpi">
                                Time written up/off
                            </td>
                            <td class="infoicon">
                                <asp:Image ID="Image3" runat="server" ImageUrl="~/Images/info.gif" ToolTip="'Time written up/off' is time that has been written up or written off, including on unfinalised invoices. Negative figures indicate write-offs and positive figures are write-ups."
                                    CssClass="infoicon" />
                            </td>
                            <td align="right">
                                <asp:Label ID="WriteOffDollarsLabel" runat="server" Text='<%# String.Format("{0:C0}", Eval("WriteOffDollars")) %>' />
                            </td>
                            <td align="right">
                                <asp:Label ID="WriteOffDollarsBudgetLabel" runat="server" Text='<%# String.Format("{0:C0}", Eval("WriteOffDollarsBudget")) %>' />
                            </td>
                            <td class="graph">
                                <asp:Panel ID="WriteOffGraph" CssClass="graph" runat="server">
                                    <asp:Label runat="server" ID="lblWriteOffGraph" CssClass="lightText"></asp:Label></asp:Panel>
                                <asp:Label runat="server" ID="lblWriteOffGraphOutside" CssClass="darkText"></asp:Label>
                            </td>
                        </tr>
                        <tr class="shaded">
                            <td class="kpi">
                                Time in WIP
                            </td>
                            <td class="infoicon">
                                <asp:Image ID="Image4" runat="server" ImageUrl="~/Images/info.gif" ToolTip="'Time in WIP' is counts all unbilled time. Time on unfinalised invoices is not included in the WIP figure."
                                    CssClass="infoicon" />
                            </td>
                            <td align="right">
                                <asp:Label ID="WIPDollarsLabel" runat="server" Text='<%# String.Format("{0:C0}", Eval("WIPDollars")) %>' />
                            </td>
                            <td align="right">
                                <asp:Label ID="WIPDollarsBudgetLabel" runat="server" Text='<%# String.Format("{0:C0}", Eval("WIPDollarsBudget")) %>' />
                            </td>
                            <td class="graph">
                                <asp:Panel ID="WIPGraph" CssClass="graph" runat="server">
                                    <asp:Label runat="server" ID="lblWIPGraph" CssClass="lightText"></asp:Label></asp:Panel>
                                <asp:Label runat="server" ID="lblWIPGraphOutside" CssClass="darkText"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:FormView>
        </asp:Panel>

        <asp:Panel ID="Panel4" runat="server" CssClass="pagesection">
            <h2>Training</h2>
            <p>What are your training goals?</p>
            <div style="float: right">
                <asp:LinkButton ID="btnCustomiseGoals" runat="server" CssClass="customiselink" OnClick="btnCustomiseGoals_Click">customise...</asp:LinkButton></div>
            <h3>Goals From Staff Review</h3>
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <asp:ListView ID="lvTrainingGoals" runat="server">
                        <EmptyDataTemplate>
                            <ul>
                                <li>You have not set any training goals.</li></ul>
                        </EmptyDataTemplate>
                        <ItemTemplate>
                            <li style="">
                                <asp:HyperLink ID="GoalHyperlink" runat="server" Text='<%# Eval("Goal") %>' NavigateUrl='<%# Eval("URL") %>'
                                    Visible='<%# Eval("DisplayLink") %>' />
                                <asp:Label ID="GoalLabel" runat="server" Text='<%# Eval("Goal") %>' Visible='<%# Eval("HideLink") %>' />
                            </li>
                        </ItemTemplate>
                        <LayoutTemplate>
                            <ul id="itemPlaceholderContainer" runat="server" style="">
                                <li runat="server" id="itemPlaceholder" />
                            </ul>
                        </LayoutTemplate>
                    </asp:ListView>
                </ContentTemplate>
            </asp:UpdatePanel>
            <%--<FRT:PromiseControl ID="TrainingPromise" runat="server" PromiseCategoryID="training" />--%>
        </asp:Panel>
        <asp:Panel ID="leavePanel" runat="server" CssClass="pagesection">
            <h2>Upcoming Leave</h2>
            <p>Leave you have planned</p>
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <asp:ListView ID="Leave" runat="server">
                        <EmptyDataTemplate>
                            <ul>
                                <li>You have no leave planned.</li></ul>
                        </EmptyDataTemplate>
                        <ItemTemplate>
                            <li>
                                <asp:Label ID="CaptionLabel" runat="server" Text='<%# Eval("Caption") %>' />
                            </li>
                        </ItemTemplate>
                        <LayoutTemplate>
                            <ul id="itemPlaceholderContainer" runat="server" style="">
                                <li runat="server" id="itemPlaceholder" />
                            </ul>
                        </LayoutTemplate>
                    </asp:ListView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
    </div>
    <div class="column">
        <asp:Panel ID="Panel7" runat="server" CssClass="pagesection">
            <FRT:PromiseControl ID="jobsOnPromise" runat="server" PromiseCategoryID="jobs_on" />
        </asp:Panel>
    </div>

    <div class="column-right">
      <asp:Panel ID="Panel2" runat="server" CssClass="pagesection">
            <h2>XPM Jobs</h2>
            <p>XPM Jobs assigned to you and your WIP</p>
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <asp:ListView ID="XPMJobs" runat="server">
                        <EmptyDataTemplate>
                            <ul>
                                <li>You have no jobs.</li></ul>
                        </EmptyDataTemplate>
                        <ItemTemplate>
                            <li><a href="<%# Eval("URL") %>">
                                <asp:Label ID="CaptionLabel" runat="server" Text='<%# Eval("Caption") %>' />
                            </a></li>
                        </ItemTemplate>
                        <LayoutTemplate>
                            <ul id="itemPlaceholderContainer" runat="server" style="">
                                <li runat="server" id="itemPlaceholder" />
                            </ul>
                        </LayoutTemplate>
                    </asp:ListView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
    </div>
</asp:Content>
