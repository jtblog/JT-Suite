using System;
using System.Windows.Forms;
using Facebook;
using System.Configuration;
using System.Dynamic;
using System.Collections.Generic;

namespace JT_Suite
{
    public partial class Form6 : Form
    {
        public static Form6 form6;
        public bool activated;
        public FacebookClient _fb;

        public Form6()
        {
            InitializeComponent();
            form6 = this;
            activated = true;
        }

        public void Form6_Load(object sender, EventArgs e)
        {
            _fb = new FacebookClient();
            webBrowser1.Navigated += new WebBrowserNavigatedEventHandler(webBrowser1_Navigated);
            webBrowser1.Navigate(GenerateLoginUrl(this.ApplicationId, this.ExtendedPermissions).AbsoluteUri);
        }

        public void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {

            FacebookOAuthResult oauthResult;
            if (_fb.TryParseOAuthCallbackUrl(e.Url, out oauthResult))
            {
                // The url is the result of OAuth 2.0 authentication
                FacebookOAuthResult = oauthResult;
            }
            else
            {
                // The url is NOT the result of OAuth 2.0 authentication.
                FacebookOAuthResult = null;
            }
            webBrowser1.DocumentTitleChanged += webBrowser1_DocumentTitleChanged;
        }

        public bool Form6_Opened()
        {
            return activated;
        }

        public static Form6 getInstance()
        {
            if (form6 == null)
            {
                form6 = new Form6();
            }
            return form6;
        }

        public void form6_buttons_MouseEnter(object sender, System.EventArgs e)
        {
            (sender as Control).BackColor = System.Drawing.Color.Pink;
        }

        public void form6_buttons_MouseLeave(object sender, System.EventArgs e)
        {
            (sender as Control).BackColor = System.Drawing.Color.White;
        }

        public void Form6_FormClosing(object sender, FormClosingEventArgs fceargs)
        {
            //var fb = new FacebookClient();
            //var logoutUrl = fb.GetLogoutUrl(new { access_token = AccessToken, next = "https://www.facebook.com/connect/login_success.html" });
            //webBrowser1.Navigate(logoutUrl);

            fceargs.Cancel = true;
            this.Hide();
        }

        public void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
        public void webBrowser1_DocumentTitleChanged(object sender, EventArgs e) {
            
            if (FacebookOAuthResult != null)
            {
                if (FacebookOAuthResult.IsSuccess)
                {
                    AccessToken = FacebookOAuthResult.AccessToken;
                    var _fb = new FacebookClient(FacebookOAuthResult.AccessToken);
            
                    dynamic result = _fb.Get("/me");
                    var name = result.name;

                    // for .net 3.5
                    //var result = (IDictionary<string, object>)_fb.Get("/me");
                    //var name = (string)result["name"];

                    MessageBox.Show("Database Connection " + JTDBConnection.getInstance().Connect());
                    if (JTDBConnection.getInstance().Insert_Facebook_User(name, AccessToken, AccessToken, AccessToken) > 0)
                    {
                        MessageBox.Show("Record saved in database");
                    }
                    else {
                        MessageBox.Show("Couldn't save data in database");
                    }
                    
                    this.Close();
                }
                else
                {
                    MessageBox.Show(FacebookOAuthResult.ErrorDescription);
                }
            }
        }

        public Uri GenerateLoginUrl(string appId, string extendedPermissions)
        {
            // for .net 3.5
            // var parameters = new Dictionary<string,object>
            // parameters["client_id"] = appId;
            dynamic parameters = new ExpandoObject();
            parameters.client_id = appId;
            parameters.redirect_uri = "https://www.facebook.com/connect/login_success.html";

            // The requested response: an access token (token), an authorization code (code), or both (code token).
            parameters.response_type = "token";

            // list of additional display modes can be found at http://developers.facebook.com/docs/reference/dialogs/#display
            parameters.display = "popup";

            // add the 'scope' parameter only if we have extendedPermissions.
            if (!string.IsNullOrWhiteSpace(extendedPermissions))
                parameters.scope = extendedPermissions;

            // when the Form is loaded navigate to the login url.
            return _fb.GetLoginUrl(parameters);
        }


        public FacebookOAuthResult FacebookOAuthResult { get; private set; }
        public string ApplicationId
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicationId"];
            }
        }
        public string ExtendedPermissions
        {
            get
            {
                return ConfigurationManager.AppSettings["ExtendedPermissions"];
            }
        }
        public string AppSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicationSecret"];
            }
        }
        public string AccessToken
        {
            get;
            set;
        }

        public void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Form6_Load(sender, e);
        }

        public void facebookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JTDBConnection.getInstance().Display();
        }

    }
}
