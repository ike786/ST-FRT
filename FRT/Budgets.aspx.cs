using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace FRT
{
    public partial class Budgets : System.Web.UI.Page
    {
        protected void RefreshData()
        {
            FRT.DAL.frtEntities entities = new FRT.DAL.frtEntities();
            DateTime thisMonth = Convert.ToDateTime(dropdownMonth.SelectedValue);

            var thisMonthKPIs = from kpi in entities.FinancialKPIsMonths
                                where kpi.KPIMonth == thisMonth
                                select kpi;

            var budgetData = from person in entities.People
                             join kpi in thisMonthKPIs on person.PersonCode equals kpi.PersonCode into gj
                             from subkpi in gj.DefaultIfEmpty()
                             orderby person.Name
                             select new
                             {
                                 person.PersonCode,
                                 person.Name,
                                 subkpi.InvoiceDollarsBudget,
                                 subkpi.TimesheetDollarsBudget,
                                 subkpi.WriteOffDollarsBudget,
                                 subkpi.WIPDollarsBudget
                             };

            rptBudget.DataSource = budgetData;
            rptBudget.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FRT.DAL.frtEntities entities = new FRT.DAL.frtEntities();

            if (!IsPostBack)
            {
                // First load - refresh 
                var months = from m in entities.Months
                             orderby m.MonthCommencing
                             select m;

                dropdownMonth.DataSource = months;
                dropdownMonth.DataTextField = "MonthCommencing";
                dropdownMonth.DataValueField = "MonthCommencing";
                dropdownMonth.DataTextFormatString = "{0:MMMM yyyy}";
                dropdownMonth.DataBind();

                RefreshData();
            }
        }

        protected void dropdownMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        protected void SaveChanges()
        {
            FRT.DAL.frtEntities entities = new FRT.DAL.frtEntities();

            // Go through the each repeater item for this month
            foreach (RepeaterItem item in rptBudget.Items)
            {
                Label lblPersonCode = ((Label)item.FindControl("lblPersonCode"));
                TextBox txtTimesheet = ((TextBox)item.FindControl("txtTimesheet"));
                TextBox txtInvoice = ((TextBox)item.FindControl("txtInvoice"));
                TextBox txtWOF = ((TextBox)item.FindControl("txtWOF"));
                TextBox txtWIP = ((TextBox)item.FindControl("txtWIP"));

                // Try to convert all the textboxes to decimal values
                // If the conversion fails, it will convert with 0
                decimal dTimesheet, dInvoice, dWOF, dWIP;
                Decimal.TryParse(txtTimesheet.Text, out dTimesheet);
                Decimal.TryParse(txtInvoice.Text, out dInvoice);
                Decimal.TryParse(txtWOF.Text, out dWOF);
                Decimal.TryParse(txtWIP.Text, out dWIP);

                DateTime thisMonth = Convert.ToDateTime(dropdownMonth.SelectedValue);

                // See if the kpi exists in the database
                var kpis = from kpi in entities.FinancialKPIsMonths
                           where kpi.KPIMonth == thisMonth
                           && kpi.PersonCode.Equals(lblPersonCode.Text)
                           select kpi;

                if (kpis.Count() != 0)
                {
                    // It does exist
                    FRT.DAL.FinancialKPIsMonth kpi = kpis.First();
                    kpi.TimesheetDollarsBudget = dTimesheet;
                    kpi.InvoiceDollarsBudget = dInvoice;
                    kpi.WriteOffDollarsBudget = dWOF;
                    kpi.WIPDollarsBudget = dWIP;
                }
                else
                {
                    // It doesn't exist, make a new row
                    FRT.DAL.FinancialKPIsMonth kpi = new FRT.DAL.FinancialKPIsMonth();
                    kpi.PersonCode = lblPersonCode.Text;
                    kpi.KPIMonth = thisMonth;
                    kpi.TimesheetDollarsBudget = dTimesheet;
                    kpi.InvoiceDollarsBudget = dInvoice;
                    kpi.WriteOffDollarsBudget = dWOF;
                    kpi.WIPDollarsBudget = dWIP;
                    kpi.TimesheetDollars = 0;
                    kpi.InvoiceDollars = 0;
                    kpi.WriteOffDollars = 0;
                    kpi.WIPDollars = 0;

                    entities.AddToFinancialKPIsMonths(kpi);
                }
            }

            // Save changes and refresh data
            entities.SaveChanges();
            RefreshData();
        }

        protected void rptBudget_TextChanged(object sender, EventArgs e)
        {
            SaveChanges();
        }

        protected void Save1_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        protected void Save2_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }
    }
}