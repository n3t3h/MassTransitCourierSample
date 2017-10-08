using System;
using System.Threading.Tasks;
using MassTransit.Courier;
using Host.Contracts;

namespace Host
{
    public class CloseAccount : ExecuteActivity<CloseAccountArguments>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<CloseAccountArguments> context)
        {
            var args = context.Arguments;
            await Console.Out.WriteLineAsync($"Closing the account for {args.Username}.");
            return context.Completed();
        }
    }
}