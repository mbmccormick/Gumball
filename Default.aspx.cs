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
            
            var jsonResult = client.DownloadString("http://bubblegum.me/api/v1/timelines/" + identifier);

            foreach (var p in Json.Decode(jsonResult))
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

                DateTime createdDate = DateTime.ParseExact(p.photo.created_at, "yyyy-MM-ddTHH:mm:ssZ", new System.Globalization.CultureInfo("en-US"));
                
                if (DateTime.UtcNow.Subtract(createdDate).Days == 0)
                    if (DateTime.UtcNow.Subtract(createdDate).Hours == 0) 
                        data.AppendLine("This photo was taken by <a href=\"http://gumball.apphub.com/?id=" + p.photo.user.id + "\">" + p.photo.user.user_name + "</a> just now at " + p.photo.lat + ", " + p.photo["long"] + ".");
                    else if (DateTime.UtcNow.Subtract(createdDate).Hours == 1)
                        data.AppendLine("This photo was taken by <a href=\"http://gumball.apphub.com/?id=" + p.photo.user.id + "\">" + p.photo.user.user_name + "</a> about " + DateTime.UtcNow.Subtract(createdDate).Hours + " hour ago at " + p.photo.lat + ", " + p.photo["long"] + ".");
                    else
                        data.AppendLine("This photo was taken by <a href=\"http://gumball.apphub.com/?id=" + p.photo.user.id + "\">" + p.photo.user.user_name + "</a> about " + DateTime.UtcNow.Subtract(createdDate).Hours + " hours ago at " + p.photo.lat + ", " + p.photo["long"] + ".");
                else if (DateTime.UtcNow.Subtract(createdDate).Days == 1)
                    data.AppendLine("This photo was taken by <a href=\"http://gumball.apphub.com/?id=" + p.photo.user.id + "\">" + p.photo.user.user_name + "</a> about " + DateTime.UtcNow.Subtract(createdDate).Days + " day ago at " + p.photo.lat + ", " + p.photo["long"] + ".");
                else
                    data.AppendLine("This photo was taken by <a href=\"http://gumball.apphub.com/?id=" + p.photo.user.id + "\">" + p.photo.user.user_name + "</a> about " + DateTime.UtcNow.Subtract(createdDate).Days + " days ago at " + p.photo.lat + ", " + p.photo["long"] + ".");
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
