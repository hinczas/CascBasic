using CascBasic.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CascBasic.Classes.API.Lookup
{
    public class GroupLookup
    {
        private LookupService _ls;

        public GroupLookup()
        {
            _ls = new LookupService();
        }

        public List<IbisGroup> AllGroups(bool includeCancelled, string fetch)
        {
            string[] pathParams = { };
            object[] queryParams = { "includeCancelled", includeCancelled,
                                     "fetch", fetch };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/group/all-groups",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.ToString());
            return result.groups;
        }

        public IbisGroup GetGroup(string groupid, string fetch)
        {
            string[] pathParams = { groupid };
            object[] queryParams = { "fetch", fetch };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/group/{0}",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.ToString());
            return result.group;
        }

        public List<IbisPerson> GetMembers(string groupid, string fetch)
        {
            string[] pathParams = { groupid };
            object[] queryParams = { "fetch", fetch };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/group/{0}/members",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.ToString());
            return result.people;
        }
    }
}