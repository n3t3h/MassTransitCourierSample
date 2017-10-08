using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier;
using Host.Contracts;

namespace Host
{
    public class GenerateFinalProratedInvoice : ExecuteActivity<GenerateFinalProratedInvoiceArguments>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<GenerateFinalProratedInvoiceArguments> context)
        {
            var args = context.Arguments;
            await Console.Out.WriteLineAsync($"Generated final prorated invoice for {args.Username}.");
            return context.Completed();
        }
    }
}