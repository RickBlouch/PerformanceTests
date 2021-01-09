using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityKeyBuilder.Application;

namespace IdentityKeyBuilder
{
    public static class IdentityKeyBuilder_WithStringBuilder
    {
        public static (string IdentityKey, string ErrorMessage) BuildIdentityKey(IdentityKeyProperties props, List<IdentityKeyProperty> identityKeyRequirements)
        {
            if (identityKeyRequirements == null || !identityKeyRequirements.Any())
            {
                return (null, $"{identityKeyRequirements} must have at least one {nameof(IdentityKeyProperty)} defined.");
            }

            var identityKeyBuilder = new StringBuilder();

            foreach (var property in identityKeyRequirements)
            {
                if (identityKeyBuilder.Length > 0) { identityKeyBuilder.Append("_"); }

                switch (property)
                {
                    case IdentityKeyProperty.AccountNumber:
                        if (string.IsNullOrEmpty(props.AccountNumber)) { return (null, $"Property {nameof(IdentityKeyProperty.AccountNumber)} cannot be null or empty."); }
                        identityKeyBuilder.Append(props.AccountNumber);
                        break;

                    case IdentityKeyProperty.SystemCode:
                        if (string.IsNullOrEmpty(props.SystemCode)) { return (null, $"Property {nameof(IdentityKeyProperty.SystemCode)} cannot be null or empty."); }
                        identityKeyBuilder.Append(props.SystemCode);
                        break;

                    case IdentityKeyProperty.ExternalId:
                        if (string.IsNullOrEmpty(props.ExternalId)) { return (null, $"Property {nameof(IdentityKeyProperty.ExternalId)} cannot be null or empty."); }
                        identityKeyBuilder.Append(props.ExternalId);
                        break;
                    case IdentityKeyProperty.ServiceDate:
                        if (props.ServiceDate == null) { return (null, $"Property {nameof(IdentityKeyProperty.ServiceDate)} cannot be null."); }
                        identityKeyBuilder.Append(props.ServiceDate?.ToString("yyyyMMdd"));
                        break;

                    default:
                        return (null, $"{nameof(IdentityKeyProperty)} '{property}' is not implemented.");
                }
            }

            var identityKey = identityKeyBuilder.ToString();

            if (identityKey.Length > 250) { return (null, $"IdentityKey cannot be more than 250 characters."); }

            return (identityKeyBuilder.ToString(), null);
        }

    }
}
