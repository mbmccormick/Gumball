using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace Gumball
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            
            string identifier;
            if (Request.QueryString["id"] == null)
                identifier = "interesting";
            else
                identifier = Request.QueryString["id"];
            
            var jsonResult1 = client.DownloadString("http://bubblegum.me/api/v1/timelines/" + identifier);

            foreach (var p in Json.Decode(jsonResult1))
            {
                StringBuilder data = new StringBuilder();
                data.AppendLine("<div class=\"photo\">");
                data.AppendLine("<div class=\"photo-image\">");
                data.AppendLine("<img src=\"" + p.photo.url + "\" alt=\"" + p.photo.caption + "\" />");
                data.AppendLine("</div>");
                data.AppendLine("<div class=\"photo-data\">");
                data.AppendLine("<div class=\"photo-data-caption\">");
                if (p.photo.caption.ToString() == "")
                    data.AppendLine("\"(no caption)\"");
                else
                    data.AppendLine("\"" + p.photo.caption.ToString().Trim() + "\"");
                data.AppendLine("</div>");
                data.AppendLine("<div class=\"photo-data-meta\">");

                string location;
                try
                {
                    var jsonResult2 = client.DownloadString("http://maps.googleapis.com/maps/api/geocode/json?address=" + p.photo.lat + "," + p.photo["long"] + "&sensor=false");
                    var r = Json.Decode(jsonResult2).results[0];

                    string city = "";
                    string state = "";
                    foreach (var t in r.address_components)
                    {
                        if (t.types[0] == "locality")
                            city = t.long_name;
                        else if (t.types[0] == "administrative_area_level_1")
                            state = t.short_name;
                    }

                    location = "<a href=\"http://www.bing.com/maps/default.aspx?lvl=12&cp=" + p.photo.lat + "~" + p.photo["long"] + "\" target=\"blank\">" + city + ", " + state + "</a>";
                }
                catch
                {
                    location = "<a href=\"http://www.bing.com/maps/default.aspx?lvl=12&cp=" + p.photo.lat + "~" + p.photo["long"] + "\" target=\"blank\">(" + p.photo.lat + ", " + p.photo["long"] + ")</a>";
                }

                DateTime createdDate = DateTime.ParseExact(p.photo.created_at, "yyyy-MM-ddTHH:mm:ssZ", new System.Globalization.CultureInfo("en-US"));
                
                if (DateTime.UtcNow.Subtract(createdDate).Days == 0)
                    if (DateTime.UtcNow.Subtract(createdDate).Hours == 0)
                        data.AppendLine("This photo was taken by <a href=\"/?id=" + p.photo.user.id + "\">" + p.photo.user.user_name + "</a> just now in " + location + ".");
                    else if (DateTime.UtcNow.Subtract(createdDate).Hours == 1)
                        data.AppendLine("This photo was taken by <a href=\"/?id=" + p.photo.user.id + "\">" + p.photo.user.user_name + "</a> about " + DateTime.UtcNow.Subtract(createdDate).Hours + " hour ago in " + location + ".");
                    else
                        data.AppendLine("This photo was taken by <a href=\"/?id=" + p.photo.user.id + "\">" + p.photo.user.user_name + "</a> about " + DateTime.UtcNow.Subtract(createdDate).Hours + " hours ago in " + location + ".");
                else if (DateTime.UtcNow.Subtract(createdDate).Days == 1)
                    data.AppendLine("This photo was taken by <a href=\"/?id=" + p.photo.user.id + "\">" + p.photo.user.user_name + "</a> about " + DateTime.UtcNow.Subtract(createdDate).Days + " day ago in " + location + ".");
                else
                    data.AppendLine("This photo was taken by <a href=\"/?id=" + p.photo.user.id + "\">" + p.photo.user.user_name + "</a> about " + DateTime.UtcNow.Subtract(createdDate).Days + " days ago in " + location + ".");

                data.AppendLine("<br />");
                data.AppendLine("<br />");
                data.AppendLine("No comments yet.");
                
                data.AppendLine("</div>");
                data.AppendLine("</div>");
                data.AppendLine("</div>");

                Literal l = new Literal();
                l.Text = data.ToString();

                this.uxContentPane.Controls.Add(l);
            }
        }
    }
}
