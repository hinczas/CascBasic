using CascBasic.Models.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CascBasic.Classes.API.Lookup
{
    public class InstitutionLookup
    {
        private LookupService _ls;

        public InstitutionLookup()
        {
            _ls = new LookupService();
        }

        public List<IbisInstitution> AllInsts(bool includeCancelled, string fetch)
        {
            string[] pathParams = { };
            object[] queryParams = { "includeCancelled", includeCancelled, "fetch", fetch };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/inst/all-insts",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.ToString());
            return result.institutions;
        }

        public IbisInstitution GetInst(string instid, string fetch)
        {
            string[] pathParams = { instid };
            object[] queryParams = { "fetch", fetch };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/inst/{0}",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.ToString());
            return result.institution;
        }
        
        public List<IbisGroup> GetGroups(string instid)
        {
            string[] pathParams = { instid };
            object[] queryParams = { "fetch", "inst_groups" };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/inst/{0}",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.ToString());
            return result.institution.groups;
        }

        public List<IbisPerson> GetMembers(string instid, string fetch)
        {
            string[] pathParams = { instid };
            object[] queryParams = { "fetch", fetch };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/inst/{0}/members",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.ToString());
            return result.people;
        }

        public IbisAttribute GetAttribute(string instid, long attrid)
        {
            string[] pathParams = { instid, "" + attrid };
            object[] queryParams = { };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/inst/{0}/{1}",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.ToString());
            return result.attribute;
        }

        public byte[] GetPhotoBytes(string instid)
        {
            string[] pathParams = { instid };
            object[] queryParams = { "attrs", "jpegPhoto" };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/inst/{0}/get-attributes",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.ToString());

            return result.attribute.binaryData;
        }

        public List<IbisInstitution> Search(string query)
        {
            return Search(query, false, false, null, 0, 100, null, null);
        }

        public List<IbisInstitution> Search(string query, bool approxMatches, bool includeCancelled, string attributes, 
                                            int offset, int limit, string orderBy, string fetch)
        {
            string[] pathParams = { };
            object[] queryParams = { "query", query,
                                     "approxMatches", approxMatches,
                                     "includeCancelled", includeCancelled,
                                     "attributes", attributes,
                                     "offset", offset,
                                     "limit", limit,
                                     "orderBy", orderBy,
                                     "fetch", fetch };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/inst/search",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.ToString());
            return result.institutions;
        }
    }
}