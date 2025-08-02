using System.Threading.Tasks;
using SandboxModelContextProtocol.Server.Services.Models;

namespace SandboxModelContextProtocol.Server.Services.Interfaces;

public interface IToolService
{
	Task<CallToolResponse> CallTool( CallToolRequest request );
	void HandleResponse( string response );
}
