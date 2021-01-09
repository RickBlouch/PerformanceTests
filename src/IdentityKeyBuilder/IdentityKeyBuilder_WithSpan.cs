using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using IdentityKeyBuilder.Application;

namespace IdentityKeyBuilder
{
    public static class IdentityKeyBuilder_WithSpan
    {

        private const int _maxStackAllocationSize = 500;
        private const char _joinChar = '_';

        public static (string IdentityKey, string ErrorMessage) BuildIdentityKey(IdentityKeyProperties props, List<IdentityKeyProperty> identityKeyRequirements)
        {
            if (identityKeyRequirements == null  || !identityKeyRequirements.Any())
            {
                return (null, $"{identityKeyRequirements} must have at least one {nameof(IdentityKeyProperty)} defined.");
            }

            var identityKeyLength = CalculateIdentityKeyLength(props, identityKeyRequirements);
            var currentPosition = 0;
            var propertyLength = 0;

            if (identityKeyLength > 250) { return (null, $"IdentityKey cannot be more than 250 characters."); }

            Span<char> identityKeySpan = stackalloc char[identityKeyLength];

            foreach (var property in identityKeyRequirements)
            {
                switch (property)
                {
                    case IdentityKeyProperty.AccountNumber:
                        propertyLength = props.AccountNumber?.Length ?? 0;
                        if (propertyLength == 0) { return (null, $"Property {nameof(IdentityKeyProperty.AccountNumber)} cannot be null or empty."); }
                        props.AccountNumber.AsSpan().CopyTo(identityKeySpan.Slice(currentPosition));
                        break;

                    case IdentityKeyProperty.SystemCode:
                        propertyLength = props.SystemCode?.Length ?? 0;
                        if (propertyLength == 0) { return (null, $"Property {nameof(IdentityKeyProperty.SystemCode)} cannot be null or empty."); }
                        props.SystemCode.AsSpan().CopyTo(identityKeySpan.Slice(currentPosition));
                        break;

                    case IdentityKeyProperty.ExternalId:
                        propertyLength = props.ExternalId?.Length ?? 0;
                        if (propertyLength == 0) { return (null, $"Property {nameof(IdentityKeyProperty.ExternalId)} cannot be null or empty."); }
                        props.ExternalId.AsSpan().CopyTo(identityKeySpan.Slice(currentPosition));
                        break;

                    case IdentityKeyProperty.ServiceDate:
                        if (props.ServiceDate == null) { return (null, $"Property {nameof(IdentityKeyProperty.ServiceDate)} cannot be null."); }
                        propertyLength = 8;
                        props.ServiceDate.Value.TryFormat(identityKeySpan.Slice(currentPosition), out int charsWritten, "yyyyMMdd");
                        break;

                    default:
                        throw new Exception($"{nameof(IdentityKeyProperty)} '{property}' is not implemented.");
                }

                currentPosition += propertyLength;

                // Append join character unless we've reached the end.  There should not be a trailing '_'.
                if (currentPosition < identityKeyLength) { identityKeySpan[currentPosition++] = _joinChar; }

            }

            return (identityKeySpan.ToString(), null); // Allocate final string
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CalculateIdentityKeyLength(IdentityKeyProperties props, List<IdentityKeyProperty> identityKeyRequirements)
        {
            int length = 0;

            // We may have values for all properties, but we only need the lengths for the properties that are part of the requirements.
            foreach (var requirement in identityKeyRequirements)
            {
                switch (requirement)
                {
                    case IdentityKeyProperty.AccountNumber:
                        length += !string.IsNullOrEmpty(props.AccountNumber) ? props.AccountNumber.Length + 1 : 0;
                        break;
                    case IdentityKeyProperty.SystemCode:
                        length += !string.IsNullOrEmpty(props.SystemCode) ? props.SystemCode.Length + 1 : 0;
                        break;
                    case IdentityKeyProperty.ExternalId:
                        length += !string.IsNullOrEmpty(props.ExternalId) ? props.ExternalId.Length + 1 : 0;
                        break;
                    case IdentityKeyProperty.ServiceDate:
                        length += props.ServiceDate != null ? 9 : 0; // 8 for 'yyyyMMdd', + 1 for '_'
                        break;
                    default:
                        throw new Exception($"{nameof(IdentityKeyProperty)} '{requirement}' is not implemented.");
                }
            }

            // Each property length adds +1 above to account for the '_' character that we include between properties.  We subtract one here
            // because we don't have a trailing '_' character on the final IdentityKey.
            return length - 1;
        }
    }
}
