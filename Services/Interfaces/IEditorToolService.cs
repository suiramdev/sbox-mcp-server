using System.Threading.Tasks;
using SandboxModelContextProtocol.Server.Services.Models;

namespace SandboxModelContextProtocol.Server.Services.Interfaces;

public interface IEditorToolService
{
	Task<CallEditorToolResponse> CallTool( CallEditorToolRequest request );
	void HandleResponse( string response );
}
