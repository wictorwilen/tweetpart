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
using System.Web.UI.WebControls.WebParts;
using System.ComponentModel;
using Microsoft.SharePoint.WebPartPages;
using System.Collections.Generic;
using Microsoft.SharePoint.Security;
using System.Security.Permissions;
using System.Net;

namespace TweetPart {

    public class TwitterWebPart : System.Web.UI.WebControls.WebParts.WebPart, IWebEditable {
        /// <summary>
        /// Initializes a new instance of the TwitterWebPart class.
        /// </summary>
        public TwitterWebPart() {
        }

        
        [SharePointPermission(SecurityAction.Demand, ObjectModel = true)]
        [WebPermission(SecurityAction.Demand, AcceptPattern = "http://www.twitter.com/*", ConnectPattern = "http://www.twitter.com/*", Unrestricted = true)]
        [WebPermission(SecurityAction.Demand, AcceptPattern = "http://search.twitter.com/*", ConnectPattern = "http://search.twitter.com/*", Unrestricted = true)]    
        protected override void CreateChildControls() {


            if (string.IsNullOrEmpty(this.Username)) {
                this.Mode = TwitterTimeLineMode.Public;
            }
            TwitterStuff.GenerateTweets(this.Username, this.Controls, this.Mode, this.TweetCount, this.SearchText);

        }



        [WebBrowsable]
        [WebDisplayName("User name")]
        [WebDescription("The Twitter Username")]
        [Category("Twitter")]
        [WebPartStorage(Storage.Personal)]
        public string Username {
            get;
            set;
        }

        [WebBrowsable]
        [WebDisplayName("Search for")]
        [WebDescription("Text to search for")]
        [Category("Twitter")]
        [WebPartStorage(Storage.Personal)]
        public string SearchText {
            get;
            set;
        }

        [WebBrowsable]
        [WebDisplayName("Time line mode")]
        [WebDescription("Friends or user time line")]
        [Category("Twitter")]
        [WebPartStorage(Storage.Personal)]
        public TwitterTimeLineMode Mode {
            get;
            set;
        }

        [WebBrowsable]
        [WebDisplayName("Tweet count")]
        [WebDescription("Number of tweets to show")]
        [Category("Twitter")]
        [Personalizable(PersonalizationScope.Shared)]
        [WebPartStorage(Storage.Shared)]
        public int TweetCount {
            get;
            set;
        }




        #region IWebEditable Members

        EditorPartCollection IWebEditable.CreateEditorParts() {
            // control the editorparts
            List<EditorPart> editors = new List<EditorPart>();
            TwitterEditorPart editorPart = new TwitterEditorPart { ID = String.Format("{0}_editorPart", this.ID) };
            editors.Add(editorPart);
            return new EditorPartCollection(editors);

        }

        object IWebEditable.WebBrowsableObject {
            get { return this; }
        }

        #endregion
    }
}
