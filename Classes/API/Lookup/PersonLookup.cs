using CascBasic.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CascBasic.Classes.API.Lookup
{
    public class PersonLookup
    {
        private LookupService _ls;

        public PersonLookup()
        {
            _ls = new LookupService();
        }

        /// <summary>
        ///Get the person with the specified identifier.
        ///
        ///By default, only a few basic details about the person are returned,
        ///but the optional <code>fetch</code> parameter may be used to fetch
        ///additional attributes or references of the person.
        ///
        ///NOTE: The person returned may be a cancelled person. It is the
        ///caller's repsonsibility to check its cancelled flag.
        ///
        ///</summary>
        ///<param name="scheme"> [required] The person identifier scheme. Typically this
        ///should be {@code "crsid"}, but other identifier schemes may be
        ///available in the future, such as {@code "usn"} or {@code "staffNumber"}. </param>
        ///<param name="identifier"> [required] The identifier of the person to fetch
        ///(typically their CRSid). </param>
        ///<param name="fetch"> [optional] A comma-separated list of any additional
        ///attributes or references to fetch. </param>
        ///
        ///<returns>The requested person or {@code null} if they were not found.</returns> 
        ///
        public IbisPerson GetPerson(string identifier, string fetch, string scheme="crsid")
        {
            string[] pathParams = { scheme, identifier };
            object[] queryParams = { "fetch", fetch };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/person/{0}/{1}",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.message);
            return result.person;
        }

        public List<IbisAttribute> GetAttributes(string identifier,string attrs, string scheme = "crsid")
        {
            string[] pathParams = { scheme, identifier };
            object[] queryParams = { "attrs", attrs };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/person/{0}/{1}/get-attributes",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.message);
            return result.attributes;
        }

        public List<IbisGroup> GetGroups(string identifier, string fetch, string scheme = "crsid")
        {
            string[] pathParams = { scheme, identifier };
            object[] queryParams = { "fetch", fetch };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/person/{0}/{1}/groups",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.message);
            return result.groups;
        }

        public List<IbisInstitution> GetInsts(string identifier, string fetch, string scheme = "crsid")
        {
            string[] pathParams = { scheme, identifier };
            object[] queryParams = { "fetch", fetch };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/person/{0}/{1}/insts",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.message);
            return result.institutions;
        }

        public bool IsMemberOfGroup(string identifier, string groupid, string scheme = "crsid")
        {
            string[] pathParams = { scheme, identifier, groupid };
            object[] queryParams = { };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/person/{0}/{1}/is-member-of-group/{2}",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.message);
            return bool.Parse(result.value);
        }

        public bool IsMemberOfInst(string identifier, string instid, string scheme = "crsid")
        {
            string[] pathParams = { scheme, identifier, instid };
            object[] queryParams = { };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/person/{0}/{1}/is-member-of-inst/{2}",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.message);
            return bool.Parse(result.value);
        }

        public List<IbisPerson> Search(string query, bool approxMatches,  bool includeCancelled, string misStatus,
                                             string attributes,  int offset,  int limit,  string orderBy, string fetch)
        {
            string[] pathParams = { };
            object[] queryParams = { "query", query,
                                     "approxMatches", approxMatches,
                                     "includeCancelled", includeCancelled,
                                     "misStatus", misStatus,
                                     "attributes", attributes,
                                     "offset", offset,
                                     "limit", limit,
                                     "orderBy", orderBy,
                                     "fetch", fetch };
            object[] formParams = { };
            IbisResult result = _ls.InvokeMethod("api/v1/person/search",
                                                  pathParams,
                                                  queryParams,
                                                  formParams);
            if (result.error != null)
                throw new Exception(result.error.ToString());
            return result.people;
        }
}
}