using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace FRT
{


    public partial class Promise : System.Web.UI.UserControl
    {
        public string PromiseCategoryID { get; set; }
        public string ThisWeek { get; set; }
        public String strDateThisWeek
        {
            get { return (String)ViewState["strDateThisWeek"]; }
            set { ViewState["strDateThisWeek"] = value; }
        }
        public String strCurrentPerson
        {
            get { return (String)ViewState["strCurrentPerson"]; }
            set { ViewState["strCurrentPerson"] = value; }
        }

        private DateTime dateThisWeek;
        private DateTime dateLastWeek;

        protected FRT.DAL.frtEntities entities = new FRT.DAL.frtEntities();

        protected void Page_Load(object sender, EventArgs e)
        {
            // See if the week has changed
            DropDownList ddWeek = (DropDownList)this.Parent.FindControl("dropdownWeek");
            DropDownList ddPerson = (DropDownList)this.Parent.FindControl("dropdownPerson");

            bool weekChanged = (!ddWeek.SelectedValue.Equals(strDateThisWeek));
            bool personChanged = (!ddPerson.SelectedValue.Equals(strCurrentPerson));

            FRT.DAL.PromiseCategory pc = (from c in entities.PromiseCategories
                                          where c.PromiseCategoryID == PromiseCategoryID
                                          select c).First();

            if (!(pc == null))
            {
                lblPromiseTitle.Text = pc.Title;
                lblPromiseDescription.Text = pc.Description;
            }

            // don't show the promise title for training promise
            lblPromiseTitle.Visible = !(PromiseCategoryID == "training");

            // If it has, refresh the data
            if (!IsPostBack || weekChanged || personChanged) { BindRepeaters(); }

        }

        public bool Save_Changes()
        {
            // Go through the each repeater item for this week
            foreach (RepeaterItem item in rptThisWeek.Items)
            {
                TextBox txtPromiseDescription = ((TextBox)item.FindControl("twPromiseDescription"));
                CheckBox cbPromiseComplete = ((CheckBox)item.FindControl("twPromiseComplete"));
                Label lblPromiseID = ((Label)item.FindControl("twPromiseID"));
                int promiseID = Convert.ToInt32(lblPromiseID.Text);

                // See if the promise exists in the database
                var promises = from p in entities.Promises
                               where p.PromiseID == promiseID
                               select p;

                if (promises.Count() != 0)
                {
                    // It does exist
                    FRT.DAL.Promise promise = promises.First();

                    // If the new textbox is empty, delete the row
                    if (txtPromiseDescription.Text == "")
                    {
                        entities.DeleteObject(promise);
                    }
                    else {
                        // Otherwise update the promisedescription
                        if (!promise.PromiseDescription.Equals(txtPromiseDescription.Text)) {                            
                            promise.PromiseDescription = txtPromiseDescription.Text;
                        }

                        // If the promisecomplete has changed, update that too.
                        if (!promise.PromiseComplete.Equals(cbPromiseComplete.Checked)) {
                            promise.PromiseComplete = cbPromiseComplete.Checked;
                        }
                    }
                }
                else
                {
                    // it doesn't exist. wtf??
                }
            }
            // Save changes and refresh data
            entities.SaveChanges();
            BindRepeaters();

            // Always return true (we have the option to in the future return false for failure)
            return true;
        }

        protected void BindRepeaters()
        {
            // Refresh the data in the repeaters
            DropDownList ddWeek = (DropDownList)this.Parent.FindControl("dropdownWeek");
            DropDownList ddPerson = (DropDownList)this.Parent.FindControl("dropdownPerson");

            dateThisWeek = Convert.ToDateTime(ddWeek.SelectedValue);
            strDateThisWeek = dateThisWeek.ToString();
            strCurrentPerson = ddPerson.SelectedValue;

            // Initialise LastWeekDate to be 7 days ago
            this.dateLastWeek = this.dateThisWeek.AddDays(-7);

            // Grab the entries for this week from the db
            var twPromises = from p in entities.Promises
                             where p.PromiseWeek == this.dateThisWeek
                             && p.PromiseCategory == PromiseCategoryID
                             && p.PersonCode == ddPerson.SelectedValue
                             select p;

            rptThisWeek.DataSource = twPromises;
            rptThisWeek.DataBind();

            // Grab the entries for last week from the db
            var lwPromises = from p in entities.Promises
                             where p.PromiseWeek == this.dateLastWeek
                             && p.PromiseCategory == PromiseCategoryID
                             && p.PersonCode == ddPerson.SelectedValue
                             select p;

            rptLastWeek.DataSource = lwPromises;
            rptLastWeek.DataBind();
        }



        protected void twPromiseDescription_TextChanged(object sender, EventArgs e)
        {
            // Go through all items in the repeater and save the right one
            this.Save_Changes();
        }

        protected void twPromiseComplete_CheckedChanged(object sender, EventArgs e)
        {
            // Go through all items in the repeater and save the right one
            this.Save_Changes();
        }
        
        protected void newPromiseDescription_TextChanged(object sender, EventArgs e)
        {
            // User has entered a new promise
            DropDownList ddWeek = (DropDownList)this.Parent.FindControl("dropdownWeek");
            DropDownList ddPerson = (DropDownList)this.Parent.FindControl("dropdownPerson");
            TextBox newPromiseDescription = ((TextBox)sender);

            // Create the promise with relevant attributes
            FRT.DAL.Promise newPromise = new FRT.DAL.Promise();
            newPromise.PromiseCategory = this.PromiseCategoryID;
            newPromise.PromiseDescription = newPromiseDescription.Text;
            newPromise.PromiseWeek = Convert.ToDateTime(ddWeek.SelectedValue);
            newPromise.PersonCode = ddPerson.SelectedValue;
            newPromiseDescription.Text = "";

            // Add to DB and commit
            entities.AddToPromises(newPromise);
            entities.SaveChanges();

            // Refresh data
            BindRepeaters();

            // Set the control focus to the 'new promise' textbox
            ScriptManager.GetCurrent(Page).SetFocus(this.rptThisWeek.Controls[rptThisWeek.Controls.Count - 1].Controls[0].FindControl("newPromiseDescription"));
            //((ScriptManager)Parent.FindControl("ScriptManager1")).SetFocus(newPromiseDescription);
        }
    }
}