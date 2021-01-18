using System;
using System.Collections.Generic;
using IdentityKeyBuilder;
using IdentityKeyBuilder.Application;
using Xunit;

namespace IdentityKeyBuilderTests
{

    // Test that update key property is built correctly without each property once
    // Test that update key is built with all properties

    public class IdentityKeyBuilder_WithSpan_NoLengthCalcTests_NoLengthCalc
    {
        [Fact]
        public void BuildIdentityKey_ReturnsError_WhennullRequirements()
        {
            var props = new IdentityKeyProperties("", "", "", null);

            var result = IdentityKeyBuilder_WithSpan_NoLengthCalc.BuildIdentityKey(props, null);

            Assert.NotNull(result.ErrorMessage);
            Assert.Null(result.IdentityKey);
        }

        [Fact]
        public void BuildIdentityKey_ReturnsError_WhenEmptyRequirements()
        {
            var requirements = new List<IdentityKeyProperty>();
            var props = new IdentityKeyProperties("", "", "", null);

            var result = IdentityKeyBuilder_WithSpan_NoLengthCalc.BuildIdentityKey(props, requirements);

            Assert.NotNull(result.ErrorMessage);
            Assert.Null(result.IdentityKey);
        }

        [Fact]
        public void BuildIdentityKey_ReturnsError_WhenIdentityKeyLength_GreaterThan1000()
        {
            var requirements = new List<IdentityKeyProperty>() { IdentityKeyProperty.AccountNumber, IdentityKeyProperty.SystemCode, IdentityKeyProperty.ExternalId };
            var props = new IdentityKeyProperties(new string('0', 400), new string('0', 400), new string('0', 201), null);

            var result = IdentityKeyBuilder_WithSpan_NoLengthCalc.BuildIdentityKey(props, requirements);

            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("cannot be more than 1000 characters", result.ErrorMessage);
            Assert.Null(result.IdentityKey);
        }

        [Fact]
        public void BuildIdentityKey_ReturnsError_WhenAccountNumberLength_Is1000()
        {
            var requirements = new List<IdentityKeyProperty>() { IdentityKeyProperty.AccountNumber };
            var props = new IdentityKeyProperties(new string('0', 1001), null, null, null);

            var result = IdentityKeyBuilder_WithSpan_NoLengthCalc.BuildIdentityKey(props, requirements);

            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("cannot be more than 1000 characters", result.ErrorMessage);
            Assert.Null(result.IdentityKey);
        }

        [Fact]
        public void BuildIdentityKey_ReturnsError_WhenSystemCodeLength_is1000()
        {
            var requirements = new List<IdentityKeyProperty>() { IdentityKeyProperty.SystemCode };
            var props = new IdentityKeyProperties(null, new string('0', 1001), null, null);

            var result = IdentityKeyBuilder_WithSpan_NoLengthCalc.BuildIdentityKey(props, requirements);

            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("cannot be more than 1000 characters", result.ErrorMessage);
            Assert.Null(result.IdentityKey);
        }

        [Fact]
        public void BuildIdentityKey_ReturnsError_WhenExternalIdLength_is1000()
        {
            var requirements = new List<IdentityKeyProperty>() { IdentityKeyProperty.ExternalId };
            var props = new IdentityKeyProperties(null, null, new string('0', 1001), null);

            var result = IdentityKeyBuilder_WithSpan_NoLengthCalc.BuildIdentityKey(props, requirements);

            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("cannot be more than 1000 characters", result.ErrorMessage);
            Assert.Null(result.IdentityKey);
        }

        [Fact]
        public void BuildIdentityKey_IsValid_WhenIdentityKeyLength_Is1000()
        {
            var requirements = new List<IdentityKeyProperty>() { IdentityKeyProperty.AccountNumber, IdentityKeyProperty.SystemCode, IdentityKeyProperty.ExternalId, IdentityKeyProperty.ServiceDate };
            var props = new IdentityKeyProperties(new string('0', 400), new string('0', 400), new string('0', 189), DateTime.Parse("10/19/2019"));

            var result = IdentityKeyBuilder_WithSpan_NoLengthCalc.BuildIdentityKey(props, requirements);

            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.IdentityKey);
            Assert.Equal(1000, result.IdentityKey.Length);
        }

        [Theory]
        [InlineData("", "SystemCode", "ExternalId", true, nameof(IdentityKeyProperty.AccountNumber))]
        [InlineData(null, "SystemCode", "ExternalId", true, nameof(IdentityKeyProperty.AccountNumber))]
        [InlineData("AccountNumber", "", "ExternalId", true, nameof(IdentityKeyProperty.SystemCode))]
        [InlineData("AccountNumber", null, "ExternalId", true, nameof(IdentityKeyProperty.SystemCode))]
        [InlineData("AccountNumber", "SystemCode", "", true, nameof(IdentityKeyProperty.ExternalId))]
        [InlineData("AccountNumber", "SystemCode", null, true, nameof(IdentityKeyProperty.ExternalId))]
        [InlineData("AccountNumber", "SystemCode", "ExternalId", false, nameof(IdentityKeyProperty.ServiceDate))]
        public void BuildIdentityKey_ReturnsError_WhenRequiredPropertyIsNullOrEmpty(string accountNumber, string systemCode, string externalId, bool hasServiceDate, string exceptionPropertyName)
        {
            DateTime? serviceDate = null;
            if (hasServiceDate) { serviceDate = new DateTime(2019, 11, 22); }

            var props = new IdentityKeyProperties(accountNumber, systemCode, externalId, serviceDate);
            var requirements = new List<IdentityKeyProperty>() { IdentityKeyProperty.AccountNumber, IdentityKeyProperty.SystemCode, IdentityKeyProperty.ExternalId, IdentityKeyProperty.ServiceDate };

            var result = IdentityKeyBuilder_WithSpan_NoLengthCalc.BuildIdentityKey(props, requirements);

            Assert.Null(result.IdentityKey);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains($"Property {exceptionPropertyName} cannot be null", result.ErrorMessage);
        }

        [Fact]
        public void BuildIdentityKey_IsValid_WhenAllRequirementsPresent()
        {
            var props = new IdentityKeyProperties("AccountNumber", "SystemCode", "ExternalId", DateTime.Parse("11/19/2019"));
            var requirements = new List<IdentityKeyProperty>() { IdentityKeyProperty.SystemCode, IdentityKeyProperty.AccountNumber, IdentityKeyProperty.ExternalId, IdentityKeyProperty.ServiceDate };

            var result = IdentityKeyBuilder_WithSpan_NoLengthCalc.BuildIdentityKey(props, requirements);

            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.IdentityKey);
            Assert.Equal("SystemCode_AccountNumber_ExternalId_20191119", result.IdentityKey);
        }

        [Fact]
        public void BuildIdentityKey_IsValid_WithoutAccountNumber()
        {
            var props = new IdentityKeyProperties("AccountNumber", "SystemCode", "ExternalId", DateTime.Parse("11/19/2019"));
            var requirements = new List<IdentityKeyProperty>() { IdentityKeyProperty.SystemCode, IdentityKeyProperty.ExternalId, IdentityKeyProperty.ServiceDate };

            var result = IdentityKeyBuilder_WithSpan_NoLengthCalc.BuildIdentityKey(props, requirements);

            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.IdentityKey);
            Assert.Equal("SystemCode_ExternalId_20191119", result.IdentityKey);
        }

        [Fact]
        public void BuildIdentityKey_IsValid_WithoutSystemCode()
        {
            var props = new IdentityKeyProperties("AccountNumber", "SystemCode", "ExternalId", DateTime.Parse("11/19/2019"));
            var requirements = new List<IdentityKeyProperty>() { IdentityKeyProperty.ServiceDate, IdentityKeyProperty.AccountNumber, IdentityKeyProperty.ExternalId };

            var result = IdentityKeyBuilder_WithSpan_NoLengthCalc.BuildIdentityKey(props, requirements);

            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.IdentityKey);
            Assert.Equal("20191119_AccountNumber_ExternalId", result.IdentityKey);
        }

        [Fact]
        public void BuildIdentityKey_IsValid_WithoutExternalId()
        {
            var props = new IdentityKeyProperties("AccountNumber", "SystemCode", "ExternalId", DateTime.Parse("11/19/2019"));
            var requirements = new List<IdentityKeyProperty>() { IdentityKeyProperty.SystemCode, IdentityKeyProperty.AccountNumber, IdentityKeyProperty.ServiceDate };

            var result = IdentityKeyBuilder_WithSpan_NoLengthCalc.BuildIdentityKey(props, requirements);

            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.IdentityKey);
            Assert.Equal("SystemCode_AccountNumber_20191119", result.IdentityKey);
        }

        [Fact]
        public void BuildIdentityKey_IsValid_WithoutServiceDate()
        {
            var props = new IdentityKeyProperties("AccountNumber", "SystemCode", "ExternalId", DateTime.Parse("11/19/2019"));
            var requirements = new List<IdentityKeyProperty>() { IdentityKeyProperty.SystemCode, IdentityKeyProperty.AccountNumber, IdentityKeyProperty.ExternalId };

            var result = IdentityKeyBuilder_WithSpan_NoLengthCalc.BuildIdentityKey(props, requirements);

            Assert.Null(result.ErrorMessage);
            Assert.NotNull(result.IdentityKey);
            Assert.Equal("SystemCode_AccountNumber_ExternalId", result.IdentityKey);
        }
    }
}
