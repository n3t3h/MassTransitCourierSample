using System.Threading.Tasks;
using MassTransit.Courier;

namespace Host
{
    public class CloseAccountArguments
    {

    }

    public class CloseAccountLog
    {

    }

    public class CloseAccount : Activity<CloseAccountArguments, CloseAccountLog>
    {
        public Task<CompensationResult> Compensate(CompensateContext<CloseAccountLog> context)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExecutionResult> Execute(ExecuteContext<CloseAccountArguments> context)
        {
            throw new System.NotImplementedException();
        }
    }
}