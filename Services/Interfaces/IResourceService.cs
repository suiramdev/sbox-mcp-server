using System.Threading.Tasks;
using SandboxModelContextProtocol.Server.Services.Models;

namespace SandboxModelContextProtocol.Server.Services.Interfaces;

public interface IResourceService
{
	Task<CallResourceResponse> GetResource( CallResourceRequest request );
	void HandleResponse( string message );
}
