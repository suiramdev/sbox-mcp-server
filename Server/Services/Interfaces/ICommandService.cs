using System.Threading.Tasks;
using SandboxModelContextProtocol.Server.Services.Models;

namespace SandboxModelContextProtocol.Server.Services.Interfaces;

public interface ICommandService
{
	Task<CommandResponse> ExecuteCommandAsync( CommandRequest request );
	void HandleResponse( string response );
}
