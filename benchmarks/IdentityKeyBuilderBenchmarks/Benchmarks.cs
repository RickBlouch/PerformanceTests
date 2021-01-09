using System;
using System.Collections.Generic;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using IdentityKeyBuilder;
using IdentityKeyBuilder.Application;

namespace IdentityKeyBuilderBenchmarks
{
    [MemoryDiagnoser]
    public class Benchmarks
    {

        private readonly List<IdentityKeyProperties> _identityKeyProperties = new List<IdentityKeyProperties>();
        private readonly List<IdentityKeyProperty> _identityKeyRequirements;

        [Params(1, 5, 10)]
        public int Iterations;

        public Benchmarks()
        {
            // Customers can choose which properties to include and what order they should be in to make up their IdentityKey.  This benchmark will include all of them.
            _identityKeyRequirements = new List<IdentityKeyProperty>() { IdentityKeyProperty.SystemCode, IdentityKeyProperty.AccountNumber, IdentityKeyProperty.ExternalId, IdentityKeyProperty.ServiceDate };
            
        }

        [GlobalSetup]
        public void Setup()
        {
            for(var i = 0; i < Iterations; i++)
            {
                _identityKeyProperties.Add(new IdentityKeyProperties($"123456789{i}", $"ROOT-System{i}", $"ExternalId-Test-Longer-Value-{i}", DateTime.UtcNow));
            }
        }

        [Benchmark(Baseline = true)]
        public void IdentityKey_WithStringBuilder()
        {
            foreach (var identityKey in _identityKeyProperties)
            {
                IdentityKeyBuilder_WithStringBuilder.BuildIdentityKey(identityKey, _identityKeyRequirements);
            }
        }

        [Benchmark]
        public void IdentityKey_WithSpan()
        {
            foreach (var identityKey in _identityKeyProperties)
            {
                IdentityKeyBuilder_WithSpan.BuildIdentityKey(identityKey, _identityKeyRequirements);
            }
        }
    }
}
