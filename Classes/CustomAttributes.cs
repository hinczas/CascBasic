﻿using CascBasic.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace CascBasic.Classes
{
    public class AuthorizeRoles : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            IPrincipal user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            if (SplitString(Users).Length > 0 && !(SplitString(Users).Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase)))
            {
                return false;
            }

            // Role preparation
            var allowedRolesRaw = SplitString(Roles);

            using (ApplicationDbContext _db = new ApplicationDbContext())
            {
                var dbUser = _db.Users.Where(a => a.Email.Equals(user.Identity.Name) || a.UserName.Equals(user.Identity.Name)).FirstOrDefault();

                if (dbUser == null)
                    return false;

                var userRoles = dbUser.Roles.Select(a => a.Role.Name).ToArray();
                var groupRoles = dbUser.Groups.SelectMany(a => a.Roles).ToList();
                var userGroupRoles = groupRoles.Select(a => a.Name).ToArray();
                userGroupRoles = userGroupRoles.Union(userRoles).ToArray();

               return userGroupRoles.Intersect(allowedRolesRaw).Any();

            }
        }

        internal static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }

    public class SessionIsRoleAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Session["Role"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Account/RoleSelect");
        }
    }
        

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizePermission : AuthorizeAttribute
    {
        private string Name;
        private string _ReturnUrl;

        public AuthorizePermission(string name)
        {
            Name = name;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Request.UrlReferrer==null || httpContext.Request.Url.Equals(httpContext.Request.UrlReferrer))
            {
                _ReturnUrl = httpContext.Server.UrlEncode("/");
            } else
            {
                _ReturnUrl = httpContext.Server.UrlEncode(httpContext.Request.UrlReferrer.PathAndQuery);
            }
            if (httpContext.Session["Role"] == null)
                return false;

            string actionName = ((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["action"].ToString();
            string controllerName = ((MvcHandler)httpContext.Handler).RequestContext.RouteData.Values["controller"].ToString();

            if (actionName.Equals("LogOff"))
                return true;

            ApplicationDbContext _db = new ApplicationDbContext();

            var role = _db.Roles.Find(httpContext.Session["roleId"]);

            if (role == null)
                return false;

            if (role.Permissions != null)
            {
                var perms = role.Permissions
                    .Where(a => a.Name.Equals(this.Name))
                    .Count();

                return perms > 0;
            }
            else
            {
                return false;
            }

        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult(string.Format("/Forbidden?code={0}&url={1}", StatusCode.PermissionUnauthorized, _ReturnUrl));
        }
    }
}