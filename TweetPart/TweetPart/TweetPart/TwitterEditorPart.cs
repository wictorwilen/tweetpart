/*
 * 
 * TweetPart for SharePoint
 * ------------------------------------------
 * Copyright (c) 2009, Wictor Wilén
 * http://tweetpart.codeplex.com/
 * http://www.wictorwilen.se/
 * ------------------------------------------
 * Licensed under the Microsoft Public License (Ms-PL) 
 * http://www.opensource.org/licenses/ms-pl.html
 * 
 */
using System;

using System.Web.UI.WebControls;
using System.Web.UI;
using System.Globalization;
using System.Web.UI.WebControls.WebParts;

namespace TweetPart {
    public class TwitterEditorPart : EditorPart {

        protected ListBox m_lbModes;
        protected TextBox m_tbUsername;
        protected TextBox m_tbSearchString;
        protected Panel m_editorPanel;
        protected Panel m_modesPanel;
        protected Panel m_usernamePanel;
        protected Panel m_searchPanel;


        protected override void CreateChildControls() {

            this.Title = "TweetPart properties";

            // Add controls here
            m_editorPanel = new Panel();
            m_editorPanel.CssClass = "ms-ToolPartSpacing";
            this.Controls.Add(m_editorPanel);

            m_modesPanel = new Panel();
            m_modesPanel.Controls.Add(new LiteralControl("<div class='UserSectionHead'>Mode</div>"));
            m_modesPanel.Controls.Add(new LiteralControl("<div class='UserSectionBody'><div class='UserControlGroup'><nobr>"));
            m_lbModes = new ListBox();
            m_lbModes.CssClass = "UserInput";
            m_lbModes.Width = new Unit("176px", CultureInfo.InvariantCulture);
            Array.ForEach(Enum.GetNames(typeof(TwitterTimeLineMode)), m_lbModes.Items.Add);

            m_lbModes.AutoPostBack = true;
            m_lbModes.SelectedIndexChanged += new EventHandler(m_lbModes_SelectedIndexChanged);
            m_modesPanel.Controls.Add(m_lbModes);
            m_modesPanel.Controls.Add(new LiteralControl("</nobr></div></div>"));
            m_editorPanel.Controls.Add(m_modesPanel);

            m_searchPanel = new Panel();
            m_searchPanel.Controls.Add(new LiteralControl("<div class='UserSectionHead'>Search string</div>"));
            m_searchPanel.Controls.Add(new LiteralControl("<div class='UserSectionBody'><div class='UserControlGroup'><nobr>"));
            m_tbSearchString = new TextBox();
            m_tbSearchString.CssClass = "UserInput";
            m_tbSearchString.Width = new Unit("176px", CultureInfo.InvariantCulture);
            m_searchPanel.Controls.Add(m_tbSearchString);
            m_searchPanel.Controls.Add(new LiteralControl("</nobr></div></div>"));
            m_editorPanel.Controls.Add(m_searchPanel);


            m_usernamePanel = new Panel();
            m_usernamePanel.Controls.Add(new LiteralControl("<div class='UserSectionHead'>Username</div>"));
            m_usernamePanel.Controls.Add(new LiteralControl("<div class='UserSectionBody'><div class='UserControlGroup'><nobr>"));
            m_tbUsername = new TextBox();
            m_tbUsername.CssClass = "UserInput";
            m_tbUsername.Width = new Unit("176px", CultureInfo.InvariantCulture);

            m_usernamePanel.Controls.Add(m_tbUsername);
            m_usernamePanel.Controls.Add(new LiteralControl("</nobr></div></div>"));
            m_editorPanel.Controls.Add(m_usernamePanel);



            base.CreateChildControls();
            this.ChildControlsCreated = true;
        }

        void m_lbModes_SelectedIndexChanged(object sender, EventArgs e) {
            showHideInputs();
        }

        private void showHideInputs() {
            switch ((TwitterTimeLineMode)Enum.Parse(typeof(TwitterTimeLineMode), m_lbModes.SelectedValue)) {
                case TwitterTimeLineMode.Search:
                    m_searchPanel.Visible = true;
                    m_usernamePanel.Visible = false;
                    break;
                case TwitterTimeLineMode.User:
                    m_searchPanel.Visible = false;
                    m_usernamePanel.Visible = true;
                    break;
                case TwitterTimeLineMode.Public:
                default:
                    m_searchPanel.Visible = false;
                    m_usernamePanel.Visible = false;
                    break;
            }
        }

        public override void SyncChanges() {
            EnsureChildControls();
            TwitterWebPart twitterWebPart = (TwitterWebPart)this.WebPartToEdit;
            if (twitterWebPart != null) {
                // get values from WebPart
                m_lbModes.SelectedValue = twitterWebPart.Mode.ToString();
                m_tbUsername.Text = twitterWebPart.Username;
                m_tbSearchString.Text = twitterWebPart.SearchText;
            }
            showHideInputs();
            return;
        }
        public override bool ApplyChanges() {
            EnsureChildControls();
            TwitterWebPart twitterWebPart = (TwitterWebPart)this.WebPartToEdit;
            if (twitterWebPart != null) {
                twitterWebPart.Mode = (TwitterTimeLineMode)Enum.Parse(typeof(TwitterTimeLineMode), m_lbModes.SelectedValue);
                twitterWebPart.SearchText = m_tbSearchString.Text;
                twitterWebPart.Username = m_tbUsername.Text;
            }
            return true;
        }
    }


}
