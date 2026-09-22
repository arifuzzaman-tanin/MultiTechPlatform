using System.Reflection;

namespace MultiTech.Platform.ArchitectureTests;

public sealed class ProjectDependencyTests
{
    private const string ApiAssemblyName = "MultiTech.Platform.Api";
    private const string ApplicationAssemblyName = "MultiTech.Platform.Application";
    private const string InfrastructureAssemblyName = "MultiTech.Platform.Infrastructure";

    [Theory]
    [InlineData("MultiTech.Platform.Domain", InfrastructureAssemblyName)]
    [InlineData("MultiTech.Platform.Domain", ApiAssemblyName)]
    [InlineData(ApplicationAssemblyName, InfrastructureAssemblyName)]
    [InlineData(ApplicationAssemblyName, ApiAssemblyName)]
    public void Project_ShouldNotReferenceForbiddenProject(
        string projectAssemblyName,
        string forbiddenAssemblyName)
    {
        Assembly assembly = Assembly.Load(projectAssemblyName);

        string[] referencedAssemblyNames = assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null)
            .Cast<string>()
            .ToArray();

        Assert.DoesNotContain(forbiddenAssemblyName, referencedAssemblyNames);
    }
}
