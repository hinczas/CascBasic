using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CascBasic.Models.API
{
    public class LookupResult
    {
        public IbisResult result { get; set; }
    }

    public class IbisResult
    {
        public string version  { get; set; }

        public string value  { get; set; }

        public IbisPerson person  { get; set; }

        public IbisInstitution institution  { get; set; }

        public IbisGroup group  { get; set; }

        public IbisIdentifier identifier  { get; set; }

        public IbisAttribute attribute  { get; set; }

        public IbisError error  { get; set; }

        public List<IbisPerson> people { get; set; }

        public List<IbisInstitution> institutions { get; set; }

        public List<IbisGroup> groups { get; set; }

        public Entities entities { get; set; }

        public List<IbisAttribute> attributes  { get; set; }

        public List<IbisAttributeScheme> attributeSchemes  { get; set; }

    }

    public class Entities
    {

        public List<IbisPerson> people { get; set; }

        public List<IbisInstitution> institutions { get; set; }

        public List<IbisGroup> groups { get; set; }

    }

    public class IbisAttributeScheme
    {
        public string schemeid  { get; set; }

        public int precedence  { get; set; }

        public string ldapName  { get; set; }

        public string displayName  { get; set; }

        public string dataType  { get; set; }

        public bool multiValued  { get; set; }

        public bool multiLined  { get; set; }

        public bool searchable  { get; set; }

        public string regexp  { get; set; }
    }

    public class IbisError
    {
        public int status  { get; set; }

        public string code  { get; set; }

        public string message  { get; set; }

        public string details  { get; set; }

        public override string ToString()
        {
            return code + " " + message;
        }
    }

    public class IbisIdentifier
    {
        public string scheme { get; set; }
        public string value { get; set; }
    }

    public class IbisContactWebPage
    {
        public string url { get; set; }

        public string label { get; set; }
    }

    public class IbisContactPhoneNumber
    {
        public string phoneType { get; set; }

        public string number { get; set; }

        public string comment { get; set; }
    }

    public class IbisContactRow
    {
        public string description { get; set; }

        public bool bold { get; set; }

        public bool italic { get; set; }

        public List<string> addresses { get; set; }

        public List<string> emails { get; set; }

        public List<IbisPerson> people { get; set; }

        public List<IbisContactPhoneNumber> phoneNumbers { get; set; }

        public List<IbisContactWebPage> webPages { get; set; }

    }

    public class IbisAttribute
    {
        public long attrid { get; set; }

        public string scheme { get; set; }

        public string value { get; set; }

        public byte[] binaryData { get; set; }

        public string comment { get; set; }

        public string instid { get; set; }

        public string visibility { get; set; }

        public DateTime effectiveFrom { get; set; }

        public DateTime effectiveTo { get; set; }

        public string owningGroupid { get; set; }

        public string EncodedString()
        {
            if (scheme == null)
                throw new Exception("Attribute scheme must be set");

            StringBuilder sb = new StringBuilder("scheme:");
            sb.Append(Base64Encode(scheme));

            if (attrid != null)
                sb.Append(",attrid:").Append(attrid);
            if (value != null)
                sb.Append(",value:").Append(Base64Encode(value));
            if (binaryData != null)
                sb.Append(",binaryData:").Append(Encoding.UTF8.GetString(binaryData));
            if (comment != null)
                sb.Append(",comment:").Append(Base64Encode(comment));
            if (instid != null)
                sb.Append(",instid:").Append(Base64Encode(instid));
            if (visibility != null)
                sb.Append(",visibility:").Append(Base64Encode(visibility));
            if (effectiveFrom != null)
                sb.Append(",effectiveFrom:").Append(effectiveFrom.ToShortDateString());
            if (effectiveTo != null)
                sb.Append(",effectiveTo:").Append(effectiveTo.ToShortDateString());
            if (owningGroupid != null)
                sb.Append(",owningGroupid:").Append(Base64Encode(owningGroupid));

            return sb.ToString();
        }

        private static string Base64Encode(string val)
        {
            try
            {
                byte[] byt = System.Text.Encoding.UTF8.GetBytes(val);

                // convert the byte array to a Base64 string

                return Convert.ToBase64String(byt);
            }
            catch (Exception e)
            {
                // Should never happen - all sensible JVMs support UTF-8
                throw e;
            }
        }

        private static string Base64Decode(string val)
        {
            try
            {
                byte[] b = Convert.FromBase64String(val);

                return System.Text.Encoding.UTF8.GetString(b);
            }
            catch (Exception e)
            {
                // Should never happen - all sensible JVMs support UTF-8
                throw e;
            }
        }

    }

    public class IbisInstitution
    {
        public bool cancelled { get; set; }

        public string instid { get; set; }

        public string name { get; set; }

        public string acronym { get; set; }

        public List<IbisAttribute> attributes { get; set; }

        public List<IbisContactRow> contactRows { get; set; }

        public List<IbisPerson> members { get; set; }

        public List<IbisInstitution> parentInsts { get; set; }

        public List<IbisInstitution> childInsts { get; set; }

        public List<IbisGroup> groups { get; set; }

        public List<IbisGroup> membersGroups { get; set; }

        public List<IbisGroup> managedByGroups { get; set; }

        public string id { get; set; }

        public string @ref { get; set; }

    }

    public class IbisGroup
    {
        public bool cancelled { get; set; }

        public string groupid { get; set; }

        public string name { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string email { get; set; }

        public IbisInstitution membersOfInst { get; set; }

        public List<IbisPerson> members { get; set; }

        public List<IbisPerson> directMembers { get; set; }

        public List<IbisInstitution> owningInsts { get; set; }

        public List<IbisInstitution> managesInsts { get; set; }

        public List<IbisGroup> managesGroups { get; set; }
        
        public List<IbisGroup> managedByGroups { get; set; }

        public List<IbisGroup> readsGroups { get; set; }

        public List<IbisGroup> readByGroups { get; set; }

        public List<IbisGroup> includesGroups { get; set; }

        public List<IbisGroup> includedByGroups { get; set; }

        public string id { get; set; }

        public string @ref { get; set; }
    }

    public class IbisPerson
    {
        public bool cancelled { get; set; }

        public IbisIdentifier identifier { get; set; }

        public string displayName { get; set; }

        public string registeredName { get; set; }

        public string surname { get; set; }

        public string visibleName { get; set; }

        public string misAffiliation { get; set; }

        public List<IbisIdentifier> identifiers { get; set; }

        public List<IbisAttribute> attributes { get; set; }

        public List<IbisInstitution> institutions { get; set; }

        public List<IbisGroup> groups { get; set; }

        public List<IbisGroup> directGroups { get; set; }
               
        public string id { get; set; }

        public bool staff { get; set; }

        public bool student { get; set; }

        public string @ref { get; set; }

    }

}