using BenchmarkDotNet.Running;

namespace IdentityKeyBuilderBenchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmarks>();
        }
    }
}