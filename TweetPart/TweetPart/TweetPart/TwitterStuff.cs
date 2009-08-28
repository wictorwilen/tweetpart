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
using System.Web.UI;
using System.Net;
using System.Xml.XPath;
using System.Web.UI.HtmlControls;
using System.Xml;
using Microsoft.SharePoint.Utilities;

namespace TweetPart {
    class TwitterStuff {
        internal static void GenerateTweets(string username, ControlCollection collection, TwitterTimeLineMode mode, int count, string searchtext) {
            HtmlTable table = new HtmlTable();

            string url = null;
            switch (mode) {
                case TwitterTimeLineMode.Public:
                    url = "http://twitter.com/statuses/public_timeline.xml";
                    break;
                case TwitterTimeLineMode.User:
                    url = string.Format("http://twitter.com/statuses/user_timeline/{0}.xml?count={1}", username, count);
                    break;
                case TwitterTimeLineMode.Search:
                    url = string.Format("http://search.twitter.com/search.atom?q={0}&rpp={1}", SPEncode.UrlEncode( searchtext), count);                    
                    break;
            }
            
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.UserAgent = "TweetPart";
            request.Timeout = 60000;
            request.Method = "GET";
            request.KeepAlive = false;
            try {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                XPathDocument doc = new XPathDocument(response.GetResponseStream());
                if (mode == TwitterTimeLineMode.Search) {
                    XPathNavigator nav = doc.CreateNavigator();
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(nav.NameTable);
                    nsmgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                    XPathExpression expr = nav.Compile("/atom:feed/atom:entry");
                    expr.SetContext(nsmgr);
                    XPathNodeIterator iterator = nav.Select(expr);
                    while (iterator.MoveNext()) {


                        string text, img, name;
                        XPathNavigator nav2 = iterator.Current.Clone();
                        nav2.MoveToChild("title", "http://www.w3.org/2005/Atom");
                        text = nav2.Value;
                        nav2.MoveToParent();
                        
                        XPathNavigator nav3 = nav2.SelectSingleNode("atom:link[@rel='image']", nsmgr);
                        
                        img = nav3.GetAttribute("href", "");
                        
                        nav2.MoveToChild("author", "http://www.w3.org/2005/Atom");
                        nav2.MoveToChild("uri", "http://www.w3.org/2005/Atom");

                        name = nav2.Value; // username
                        
                        

                        HtmlTableRow row = new HtmlTableRow();
                        HtmlTableCell cell1 = new HtmlTableCell();
                        HtmlTableCell cell2 = new HtmlTableCell();
                        cell1.VAlign = "Top";
                        row.Cells.Add(cell1);
                        row.Cells.Add(cell2);
                        cell1.Controls.Add(new LiteralControl(string.Format("<a href='{0}'><img src='{1}' style='border:0px' align='left'/></a>", name, img)));
                        cell2.InnerText = text;
                        table.Rows.Add(row);
                    }
                }
                else {
                    XPathNavigator nav = doc.CreateNavigator();
                    XPathExpression expr = nav.Compile("/statuses/status");
                    XPathNodeIterator iterator = nav.Select(expr);
                    while (iterator.MoveNext()) {


                        string text, img, name;
                        XPathNavigator nav2 = iterator.Current.Clone();
                        nav2.MoveToChild("text", "");
                        text = nav2.Value;
                        nav2.MoveToParent();
                        nav2.MoveToChild("user", "");
                        nav2.MoveToChild("profile_image_url", "");
                        img = nav2.Value; // image
                        nav2.MoveToParent();
                        nav2.MoveToChild("screen_name", "");
                        name = nav2.Value; // username

                        HtmlTableRow row = new HtmlTableRow();
                        HtmlTableCell cell1 = new HtmlTableCell();
                        HtmlTableCell cell2 = new HtmlTableCell();
                        cell1.VAlign = "Top";
                        row.Cells.Add(cell1);
                        row.Cells.Add(cell2);
                        cell1.Controls.Add(new LiteralControl(string.Format("<a href='http://twitter.com/{0}/'><img src='{1}' style='border:0px' align='left'/></a>", name, img)));
                        cell2.InnerText = text;
                        table.Rows.Add(row);
                    }

                }
            }
            catch (WebException e) {
                table.Rows.Add(new HtmlTableRow());
                table.Rows[0].Cells.Add(new HtmlTableCell());
                table.Rows[0].Cells[0].InnerText = e.ToString();
            }
            collection.Add(table);
        }
    }
}