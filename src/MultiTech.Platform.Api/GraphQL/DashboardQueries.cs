using HotChocolate;
using MultiTech.Platform.Application.Abstractions.Messaging;
using MultiTech.Platform.Application.Features.Dashboard.GetOverview;
using DashboardOverviewApplicationResult = MultiTech.Platform.Application.Common.Results.Result<MultiTech.Platform.Application.Features.Dashboard.GetOverview.DashboardOverviewDto>;

namespace MultiTech.Platform.Api.GraphQL;

/// <summary>
/// Provides dashboard GraphQL query resolvers.
/// </summary>
public sealed class DashboardQueries
{
    /// <summary>
    /// Retrieves the dashboard overview summary metrics.
    /// </summary>
    /// <param name="handler">The dashboard overview query handler.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>The dashboard overview metrics.</returns>
    public async Task<DashboardOverviewDto> GetDashboardOverviewAsync(
        [Service] IQueryHandler<GetDashboardOverviewQuery, DashboardOverviewDto> handler,
        CancellationToken cancellationToken)
    {
        DashboardOverviewApplicationResult result = await handler.Handle(
            new GetDashboardOverviewQuery(),
            cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return result.Value;
        }

        throw new GraphQLException(
            ErrorBuilder.New()
                .SetMessage("Dashboard overview could not be loaded.")
                .SetCode(result.Error?.Code ?? "Dashboard.OverviewUnavailable")
                .Build());
    }
}
