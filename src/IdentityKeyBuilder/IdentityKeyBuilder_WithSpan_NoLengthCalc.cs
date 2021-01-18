using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using IdentityKeyBuilder.Application;

namespace IdentityKeyBuilder
{
    public static class IdentityKeyBuilder_WithSpan_NoLengthCalc
    {
        private const char _joinChar = '_';
        private static int _maxIdentityKeyLength = 1000;

        public static (string IdentityKey, string ErrorMessage) BuildIdentityKey(IdentityKeyProperties props, List<IdentityKeyProperty> identityKeyRequirements)
        {
            if (identityKeyRequirements == null  || !identityKeyRequirements.Any())
            {
                return (null, $"{identityKeyRequirements} must have at least one {nameof(IdentityKeyProperty)} defined.");
            }

            var currentPosition = 0;
            var propertyLength = 0;

            Span<char> identityKeySpan = stackalloc char[_maxIdentityKeyLength];

            foreach (var property in identityKeyRequirements)
            {

                if (currentPosition > 0) { identityKeySpan[currentPosition++] = _joinChar; }

                switch (property)
                {
                    case IdentityKeyProperty.AccountNumber:
                        propertyLength = props.AccountNumber?.Length ?? 0;
                        if (currentPosition + propertyLength > 1000) { return (null, $"IdentityKey cannot be more than 1000 characters."); }
                        if (propertyLength == 0) { return (null, $"Property {nameof(IdentityKeyProperty.AccountNumber)} cannot be null or empty."); }
                        props.AccountNumber.AsSpan().CopyTo(identityKeySpan.Slice(currentPosition));
                        break;

                    case IdentityKeyProperty.SystemCode:
                        propertyLength = props.SystemCode?.Length ?? 0;
                        if (currentPosition + propertyLength > 1000) { return (null, $"IdentityKey cannot be more than 1000 characters."); }
                        if (propertyLength == 0) { return (null, $"Property {nameof(IdentityKeyProperty.SystemCode)} cannot be null or empty."); }
                        props.SystemCode.AsSpan().CopyTo(identityKeySpan.Slice(currentPosition));
                        break;

                    case IdentityKeyProperty.ExternalId:
                        propertyLength = props.ExternalId?.Length ?? 0;
                        if (currentPosition + propertyLength > 1000) { return (null, $"IdentityKey cannot be more than 1000 characters."); }
                        if (propertyLength == 0) { return (null, $"Property {nameof(IdentityKeyProperty.ExternalId)} cannot be null or empty."); }
                        props.ExternalId.AsSpan().CopyTo(identityKeySpan.Slice(currentPosition));
                        break;

                    case IdentityKeyProperty.ServiceDate:
                        if (props.ServiceDate == null) { return (null, $"Property {nameof(IdentityKeyProperty.ServiceDate)} cannot be null."); }
                        propertyLength = 8;
                        if (currentPosition + propertyLength > 1000) { return (null, $"IdentityKey cannot be more than 1000 characters."); }
                        props.ServiceDate.Value.TryFormat(identityKeySpan.Slice(currentPosition), out int charsWritten, "yyyyMMdd");
                        break;

                    default:
                        throw new Exception($"{nameof(IdentityKeyProperty)} '{property}' is not implemented.");
                }

                currentPosition += propertyLength;
            }

            return (identityKeySpan.Slice(0, currentPosition).ToString(), null); // Allocate final string
        }

    }
}
