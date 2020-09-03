using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Planiture_Website.Areas.Identity.Pages.Account.Manage
{
    public static class ManageNavPages
    {
        public static string Index => "Index";

        public static string Email => "Email";

        public static string ChangePassword => "ChangePassword";

        public static string DownloadPersonalData => "DownloadPersonalData";

        public static string DeletePersonalData => "DeletePersonalData";

        public static string ExternalLogins => "ExternalLogins";

        public static string PersonalData => "PersonalData";

        public static string TwoFactorAuthentication => "TwoFactorAuthentication";

        //just added
        public static string CreateRole => "CreateRole";
        public static string Dashboard => "Dashboard";
        public static string Accounts => "Accounts";
        public static string Transactions => "Transactions";
        public static string ConfigureChat => "Set Up Live Chat";
        public static string AgentLogin => "AgentLogin";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string EmailNavClass(ViewContext viewContext) => PageNavClass(viewContext, Email);

        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

        public static string DownloadPersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DownloadPersonalData);

        public static string DeletePersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DeletePersonalData);

        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);

        public static string PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalData);

        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);

        //just added
        public static string CreateRoleNavClass(ViewContext viewContext) => PageNavClass(viewContext, CreateRole);
        public static string DashboardNavClass(ViewContext viewContext) => PageNavClass(viewContext, Dashboard);
        public static string AccountsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Accounts);
        public static string TransactionsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Transactions);
        public static string ConfigureChatNavClass(ViewContext viewContext) => PageNavClass(viewContext, ConfigureChat);
        public static string AgentLoginNavClass(ViewContext viewContext) => PageNavClass(viewContext, AgentLogin);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
