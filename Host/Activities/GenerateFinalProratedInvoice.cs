using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier;

namespace Host
{
    public class GenerateFinalProratedInvoiceArguments
    {

    }

    public class GenerateFinalProratedInvoiceLog
    {

    }

    public class GenerateFinalProratedInvoice : Activity<GenerateFinalProratedInvoiceArguments, GenerateFinalProratedInvoiceLog>
    {
        public Task<CompensationResult> Compensate(CompensateContext<GenerateFinalProratedInvoiceLog> context)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExecutionResult> Execute(ExecuteContext<GenerateFinalProratedInvoiceArguments> context)
        {
            throw new System.NotImplementedException();
        }
    }
}