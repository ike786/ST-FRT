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
    public partial class _Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            FRT.DAL.frtEntities entities = new FRT.DAL.frtEntities();

            // is this the first page load?
            if (!IsPostBack)
            {
                // Grab the weeks from the database and populate the dropdown box
                var weeks = from w in entities.Weeks
                            select w;

                dropdownWeek.DataSource = weeks;
                dropdownWeek.DataTextField = "WeekCommencing";
                dropdownWeek.DataValueField = "WeekCommencing";
                dropdownWeek.DataTextFormatString = "{0:ddd dd/MM/yy}";

                // Look up the date of last Friday

                DateTime dateLastFriday = DateTime.Now;


                // if it's friday
                if (dateLastFriday.DayOfWeek == DayOfWeek.Friday)
                {
                    // set the drop down date as today
                    dateLastFriday = dateLastFriday.Date;

                }
                else { // if not, get the last Friday

                    while ((dateLastFriday.DayOfWeek != DayOfWeek.Friday))
                    {

                        dateLastFriday = dateLastFriday.AddDays(-1);
                        dateLastFriday = dateLastFriday.Date;
                        System.Diagnostics.Trace.WriteLine(dateLastFriday);

                    }


                }

                var week = from w in weeks
                           where w.WeekCommencing == dateLastFriday
                           select w;

                var descendingWeeks = from w in weeks
                                      orderby w.WeekCommencing descending
                                      select w;

                // If it doesn't exist, just select the last week we can. Otherwise grab last Monday.
                if (week.Count() == 0) { dropdownWeek.SelectedValue = descendingWeeks.First().WeekCommencing.ToString(); }
                else { dropdownWeek.SelectedValue = week.First().WeekCommencing.ToString(); }

                dropdownWeek.DataBind();


                var people = (from p in entities.People
                              where p.PersonCode != "FIRM" && p.Hidden == false
                              orderby p.Name ascending
                              select p).ToList();

                // Grab the people from the database and populate the dropdown box
                dropdownPerson.DataSource = people;
                dropdownPerson.DataTextField = "Name";
                dropdownPerson.DataValueField = "PersonCode";
                dropdownPerson.DataBind();

                dropdownPerson.Items.Insert(0, new ListItem("- Please select a name -", ""));
                dropdownPerson.SelectedIndex = 0;

                // Refresh the 'Timesheets' section
                SetPanelVisibility();
                //RefreshTimesheetPromise();
                RefreshTrainingGoals();
                RefreshXPMJobs();
                RefreshLeave();
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

        protected void SetPanelVisibility()
        {
            bool isPersonSelected = !(dropdownPerson.SelectedValue == "");

            List<Panel> allPanels = new List<Panel>();
            GetControlList<Panel>(Page.Controls, allPanels);

            foreach (Panel panel in allPanels)
            {
                if (panel.CssClass == "pagesection") { panel.Visible = isPersonSelected; }
            }
        }

        protected void dropdownPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the 'Timesheets' section
            SetPanelVisibility();
            //RefreshTimesheetPromise();
            RefreshTrainingGoals();
            RefreshFinancialKPIs();
            RefreshXPMJobs();
            RefreshLeave();

            ScriptManager.GetCurrent(Page).SetFocus(dropdownPerson);
        }

        protected void dropdownWeek_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the 'Timesheets' section
            //RefreshTimesheetPromise();
            RefreshFinancialKPIs();

            ScriptManager.GetCurrent(Page).SetFocus(dropdownWeek);
        }

        protected void RefreshTrainingGoals()
        {
            FRT.DAL.frtEntities entities = new FRT.DAL.frtEntities();
            var trainingGoals = from t in entities.TrainingGoals
                                where t.PersonCode == dropdownPerson.SelectedValue
                                select new
                                {
                                    t.PersonCode,
                                    t.GoalID,
                                    t.Goal,
                                    t.URL,
                                    DisplayLink = t.URL == null ? false : true,
                                    HideLink = t.URL == null ? true: false
                                };

            rptTrainingGoals.DataSource = trainingGoals;
            rptTrainingGoals.DataBind();
            lvTrainingGoals.DataSource = trainingGoals;
            lvTrainingGoals.DataBind();
        }

        protected void RefreshXPMJobs()
        {
            FRT.DAL.frtEntities entities = new FRT.DAL.frtEntities();

            var jobs = from j in entities.XPMJobs
                                where j.PersonCode == dropdownPerson.SelectedValue
                                select j;


            XPMJobs.DataSource = jobs;
            XPMJobs.DataBind();
        }

        protected void RefreshLeave()
        {
            FRT.DAL.frtEntities entities = new FRT.DAL.frtEntities();

            var leave = from l in entities.Leaves
                       where l.PersonCode == dropdownPerson.SelectedValue
                       select l;

            Leave.DataSource = leave;
            Leave.DataBind();
        }

        protected void RefreshFinancialKPIs()
        {
            FRT.DAL.frtEntities entities = new FRT.DAL.frtEntities();

            DateTime selectedWeek = Convert.ToDateTime(dropdownWeek.SelectedValue);
            // Subtract the day of the month to get the first
            DateTime thisMonth = selectedWeek.AddDays((-selectedWeek.Day) + 1);

            var KPIMs = from k in entities.FinancialKPIsMonths
                        where k.PersonCode == dropdownPerson.SelectedValue
                        && k.KPIMonth == thisMonth
                        select new
                        {
                            k.TimesheetDollars,
                            k.TimesheetDollarsBudget,
                            k.InvoiceDollars,
                            k.InvoiceDollarsBudget,
                            k.WriteOffDollars,
                            k.WriteOffDollarsBudget,
                            k.WIPDollars,
                            k.WIPDollarsBudget,
                            TimesheetPercent = k.TimesheetDollarsBudget == 0 ? 0 : k.TimesheetDollars / k.TimesheetDollarsBudget,
                            InvoicePercent = k.InvoiceDollarsBudget == 0 ? 0 : k.InvoiceDollars / k.InvoiceDollarsBudget,
                            WriteOffPercent = k.WriteOffDollarsBudget == 0 ? 0 : k.WriteOffDollars / k.WriteOffDollarsBudget,
                            WIPPercent = k.WIPDollarsBudget == 0 ? 0 : k.WIPDollars / k.WIPDollarsBudget
                        };

            fvFinancialKPIsMonth.DataSource = KPIMs;
            fvFinancialKPIsMonth.DataBind();

            Panel myPanel;
            decimal myPercentage;
            Label insideLabel, outsideLabel;

            if (KPIMs.Count() == 1)
            {
                string[] graphList = { "Timesheet", "Invoice", "WriteOff", "WIP" };

                foreach (string item in graphList)
                {
                    switch (item)
                    {
                        case "Timesheet":
                            myPercentage = Convert.ToInt32(KPIMs.First().TimesheetPercent * 100);
                            break;
                        case "Invoice":
                            myPercentage = Convert.ToInt32(KPIMs.First().InvoicePercent * 100);
                            break;
                        case "WriteOff":
                            myPercentage = Convert.ToInt32(KPIMs.First().WriteOffPercent * 100);
                            break;
                        case "WIP":
                            myPercentage = Convert.ToInt32(KPIMs.First().WIPPercent * 100);
                            break;
                        default:
                            myPercentage = 0;
                            break;
                    }

                    myPanel = (Panel)fvFinancialKPIsMonth.FindControl(item + "Graph");
                    if (myPercentage > 100) { myPanel.Width = Unit.Percentage(100); }
                    else if (myPercentage < 0) { myPanel.Width = Unit.Percentage(0); }
                    else myPanel.Width = Unit.Percentage((Convert.ToDouble(myPercentage)));
                    insideLabel = (Label)myPanel.FindControl("lbl" + item + "Graph");
                    outsideLabel = (Label)fvFinancialKPIsMonth.FindControl("lbl" + item + "GraphOutside");
                    if (myPercentage > 65)
                    {
                        outsideLabel.Text = "&nbsp;";
                        insideLabel.Text = myPercentage.ToString("F0") + "%";
                        outsideLabel.Visible = false;
                    }
                    else
                    {
                        insideLabel.Text = "&nbsp;";
                        outsideLabel.Text = myPercentage.ToString("F0") + "%";
                        outsideLabel.Visible = true;
                    }

                    myPanel.CssClass = "graph";
                    if (item == "WIP" && myPercentage > 100) { myPanel.CssClass += " red"; }
                    if (item == "WriteOff" && myPercentage > 100) { myPanel.CssClass += " red"; }
                    if (item == "Timesheet" && myPercentage > 100) { myPanel.CssClass += " green"; }
                    if (item == "Invoice" && myPercentage > 100) { myPanel.CssClass += " green"; }
                }

            }
        }

        protected void btnCustomiseGoals_Click(object sender, EventArgs e)
        {
            pnlTrainingGoals.Visible = true;
        }

        protected void newGoal(object sender, EventArgs e)
        {
            FRT.DAL.frtEntities entities = new FRT.DAL.frtEntities();

            TextBox newGoal = (TextBox)rptTrainingGoals.Controls[rptTrainingGoals.Controls.Count - 1].Controls[0].FindControl("newGoal");
            TextBox newGoalURL = (TextBox)rptTrainingGoals.Controls[rptTrainingGoals.Controls.Count - 1].Controls[0].FindControl("newGoalURL");

            // User has entered a new goal

            // Create the promise with relevant attributes
            FRT.DAL.TrainingGoal goal = new FRT.DAL.TrainingGoal();
            goal.PersonCode = dropdownPerson.SelectedValue;
            goal.Date = DateTime.Today;
            goal.Goal = newGoal.Text;
            goal.URL = newGoalURL.Text;

            // Add to DB and commit
            entities.AddToTrainingGoals(goal);
            entities.SaveChanges();

            // Refresh data
            RefreshTrainingGoals();

            // Set focus to the new caption
            ScriptManager.GetCurrent(Page).SetFocus(rptTrainingGoals.Controls[rptTrainingGoals.Controls.Count - 1].Controls[0].FindControl("newGoal"));
        }

        protected void SaveGoals_Click(object sender, EventArgs e)
        {
            FRT.DAL.frtEntities entities = new FRT.DAL.frtEntities();

            // Go through each of the items in the repeater and update
            foreach (RepeaterItem item in rptTrainingGoals.Items)
            {
                HiddenField lblGoalID = (HiddenField)item.FindControl("lblGoalID");
                TextBox txtGoal = (TextBox)item.FindControl("txtGoal");
                TextBox txtGoalURL = (TextBox)item.FindControl("txtGoalURL");

                int goalID = Convert.ToInt32(lblGoalID.Value);

                // See if the goal exists in the database
                var goals = from g in entities.TrainingGoals
                            where g.GoalID == goalID
                            select g;

                if (goals.Count() != 0)
                {
                    // It does exist
                    FRT.DAL.TrainingGoal goal = goals.First();

                    // If the textbox is empty, delete the row
                    if (txtGoal.Text.Equals(""))
                    {
                        entities.DeleteObject(goal);
                    }
                    else
                    {
                        // Otherwise update the goal
                        goal.Goal = txtGoal.Text;
                        goal.URL = txtGoalURL.Text;
                    }
                }
                else
                {
                    // it doesn't exist. wtf??
                }
            }
            // Save changes and refresh data
            entities.SaveChanges();
            RefreshTrainingGoals();
            pnlTrainingGoals.Visible = false;
        }
    }
}
