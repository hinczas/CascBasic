using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CascBasic.Classes
{
    public enum StatusCode
    {
        Unknown,
        ObjectNotFound,
        ExceptionThrown,
        Success,
        CannotRemPerms,
        ParentNotFound,
        CreateSuccess,
        UpdateSuccess,
        ApiResultEmpty
    }
}