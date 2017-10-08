using System.Threading.Tasks;
using System;
using System.IO;
using MassTransit.Courier;
using Host.Contracts;

namespace Host
{
    public class ChargeAllOpenInvoices : ExecuteActivity<ChargeAllOpenInvoicesArguments>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<ChargeAllOpenInvoicesArguments> context)
        {
            var args = context.Arguments;
            await Console.Out.WriteLineAsync($"Charging all open invoices for {args.Username}.");
            return context.Completed();
        }
    }
}