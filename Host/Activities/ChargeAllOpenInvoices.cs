using System.Threading.Tasks;
using MassTransit.Courier;

namespace Host
{
    public class ChargeAllOpenInvoicesArguments
    {

    }

    public class ChargeAllOpenInvoicesLog
    {

    }

    public class ChargeAllOpenInvoices : Activity<ChargeAllOpenInvoicesArguments, ChargeAllOpenInvoicesLog>
    {
        public Task<CompensationResult> Compensate(CompensateContext<ChargeAllOpenInvoicesLog> context)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExecutionResult> Execute(ExecuteContext<ChargeAllOpenInvoicesArguments> context)
        {
            throw new System.NotImplementedException();
        }
    }
}