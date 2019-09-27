using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace CSVORM_Magnitude.Models
{
    public class DynamicEntity : DynamicObject
    {

        private readonly IDictionary<string, object> _values;

        public DynamicEntity(IDictionary<string, object> values)
        {
            _values = values;
        }
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _values.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_values.ContainsKey(binder.Name))
            {
                result = _values[binder.Name];
                return true;
            }
            result = null;
            return false;
        }

    }
}