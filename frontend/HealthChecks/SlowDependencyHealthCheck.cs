using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GloboTicket.Frontend.HealthChecks
{
    public class SlowDependencyHealthCheck : IHealthCheck
    {
        public static readonly string HealthCheckName = "slow_dependency";

        private static int invocationCount = 0;

        public SlowDependencyHealthCheck()
        {
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocationCount++>3)
            {
                return Task.FromResult(HealthCheckResult.Healthy($"Dependency is ready. Invocation count:{invocationCount}"));
            }
            
            return Task.FromResult(new HealthCheckResult(
                status: context.Registration.FailureStatus,
                description: $"Dependency is still initializing. Invocation Count:{invocationCount}"));
        }
    }
}
