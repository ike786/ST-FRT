using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace FRT
{
    public partial class WIP : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            FRT.DAL.frtEntities entities = new FRT.DAL.frtEntities();

            // is this the first page load?
            if (!IsPostBack)
            {
                // Grab the weeks from the database and populate the dropdown box
                var months = from m in entities.Months
                             select m;

                dropdownMonthWIP.DataSource = months;
                dropdownMonthWIP.DataTextField = "MonthCommencing";
                dropdownMonthWIP.DataValueField = "MonthCommencing";
                dropdownMonthWIP.DataTextFormatString = "{0:ddd dd/MM/yy}";

                // Look up the date of last Monday
                DateTime dateFirstofMonth = (DateTime.Today.AddDays(-((int)DateTime.Today.Day - 1)));

                var month = from m in months
                            where m.MonthCommencing == dateFirstofMonth
                            select m;

                var descendingMonths = from m in months
                                       orderby m.MonthCommencing descending
                                       select m;

                // If it doesn't exist, just select the last week we can. Otherwise grab last Monday.
                if (month.Count() == 0) { dropdownMonthWIP.SelectedValue = descendingMonths.First().MonthCommencing.ToString(); }
                else { dropdownMonthWIP.SelectedValue = month.First().MonthCommencing.ToString(); }

                dropdownMonthWIP.DataBind();

                // add options to stat selection drop-down
                Dictionary<string, string> graphStats = new Dictionary<string, string>();
                graphStats.Add("budgetpc", "WIP as percentage of budget");
                graphStats.Add("dollars", "WIP in dollars");
                graphStat.DataSource = graphStats;
                graphStat.DataTextField = "Value";
                graphStat.DataValueField = "Key";
                graphStat.DataBind();

                RefreshWIP();
            }

            // Not the first page load, nothing to do. Other events are handled by their
            // relevant events
        }

        private void GetControlList<T>(ControlCollection controlCollection, List<T> resultCollection) where T : Control
        {
            foreach (Control control in controlCollection)
            {
                //if (control.GetType() == typeof(T))
                if (control is T) // This is cleaner
                    resultCollection.Add((T)control);

                if (control.HasControls())
                    GetControlList(control.Controls, resultCollection);
            }
        }

        protected void dropdownMonthWIP_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the 'Timesheets' section
            RefreshWIP();

            ScriptManager.GetCurrent(Page).SetFocus(dropdownMonthWIP);
        }

        protected void showHiddenStaff_CheckedChanged(object sender, EventArgs e)
        {
            // Update the 'Timesheets' section
            RefreshWIP();

            ScriptManager.GetCurrent(Page).SetFocus(showHiddenStaff);
        }

        protected void graphStat_TextChanged(object sender, EventArgs e)
        {
            // Update the 'Timesheets' section
            RefreshWIP();

            ScriptManager.GetCurrent(Page).SetFocus(graphStat);
        }

        protected void RefreshWIP()
        {
            FRT.DAL.frtEntities entities = new FRT.DAL.frtEntities();

            DateTime selectedMonth = Convert.ToDateTime(dropdownMonthWIP.SelectedValue);
            // Subtract the day of the month to get the first
            DateTime thisMonth = selectedMonth; // selectedMonth.AddDays((-selectedWeek.Day) + 1);

            bool showHidden = showHiddenStaff.Checked;

            // include both non-hidden and optionally hidden staff
            var KPIMs = from k in entities.FinancialKPIsMonths
                        where k.KPIMonth == thisMonth
                        && (k.Person.Hidden == false
                        || k.Person.Hidden == showHidden)
                        select new
                        {
                            k.Person.Name,
                            k.WIPDollars,
                            k.WIPDollarsBudget,
                            WIPPercent = k.WIPDollarsBudget == 0 ? 0 : k.WIPDollars / k.WIPDollarsBudget
                        };

            var orderedKPIMs = from k in KPIMs
                               orderby k.Name
                               select k;

            fvWIP.DataSource = orderedKPIMs;
            fvWIP.DataBind();

            decimal maxGraphValue;
            decimal scaledPercentage, myGraphValue;

            string graphValueFormat;

            if (graphStat.SelectedItem.Value == "dollars")
            { 
                maxGraphValue = Convert.ToDecimal(orderedKPIMs.Max(s => s.WIPDollars));
                graphValueFormat = "C0";
            }
            else {
                maxGraphValue = Convert.ToDecimal(orderedKPIMs.Max(s => s.WIPPercent));
                graphValueFormat = "P0";
            }

            if (orderedKPIMs.Count() > 1)
            {
                for (int i = 0; i <= fvWIP.Items.Count - 1; i++)
                {
                    Panel myPanel = (Panel)fvWIP.Items[i].FindControl("WIPGraph");

                    if (graphStat.SelectedItem.Value == "dollars")
                    { myGraphValue = Convert.ToDecimal(orderedKPIMs.Skip(i).First().WIPDollars); }
                    else { myGraphValue = Convert.ToDecimal(orderedKPIMs.Skip(i).First().WIPPercent); }

                    Label insideLabel = (Label)fvWIP.Items[i].FindControl("lblWIPGraph");
                    Label outsideLabel = (Label)fvWIP.Items[i].FindControl("lblWIPGraphOutside");

                    if (myGraphValue <= 0) { scaledPercentage = 0; }
                    else { scaledPercentage = (myGraphValue / maxGraphValue) * 100; }

                    if (scaledPercentage >= 100) { myPanel.Width = Unit.Percentage(100); }
                    else if (scaledPercentage <= 0) { myPanel.Width = Unit.Percentage(0); }
                    else myPanel.Width = Unit.Percentage(Convert.ToDouble(scaledPercentage));

                    if (scaledPercentage > 75)
                    {
                        outsideLabel.Text = "&nbsp;";
                        insideLabel.Text = myGraphValue.ToString(graphValueFormat);
                        outsideLabel.Visible = false;
                    }
                    else
                    {
                        insideLabel.Text = "&nbsp;";
                        outsideLabel.Text = myGraphValue.ToString(graphValueFormat);
                        outsideLabel.Visible = true;
                    }

                    myPanel.CssClass = "graph";
                    if ((Convert.ToDecimal(orderedKPIMs.Skip(i).First().WIPPercent)) > 1)
                    { myPanel.CssClass += " red"; }
                }
            }
        }
    }
}
