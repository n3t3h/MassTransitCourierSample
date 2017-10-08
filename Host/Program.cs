using GreenPipes;
using GreenPipes.Partitioning;
using Host.Contracts;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using MassTransit.MongoDbIntegration.Courier;
using MassTransit.MongoDbIntegration.Courier.Documents;
using MassTransit.RabbitMqTransport;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Host
{
    class Program
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<RoutingSlipDocument> _routingSlips;
        private readonly IBusControl _busControl;
        private readonly IRoutingSlipEventPersister _persister;

        public Program()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("masstransitcouriersample");
            _routingSlips = _database.GetCollection<RoutingSlipDocument>("routing_slips");
            _persister = new RoutingSlipEventPersister(_routingSlips);

            _busControl = Bus.Factory.CreateUsingRabbitMq(cfg => {
                var host = cfg.Host(new Uri("rabbitmq://localhost"), hc => {
                    hc.Username("guest");
                    hc.Password("guest");
                });

                cfg.ReceiveEndpoint(host, "generatefinalinvoice", ep => {
                    ep.ExecuteActivityHost<GenerateFinalProratedInvoice, GenerateFinalProratedInvoiceArguments>();
                });

                cfg.ReceiveEndpoint(host, "chargeallopeninvoices", ep => {
                    ep.ExecuteActivityHost<ChargeAllOpenInvoices, ChargeAllOpenInvoicesArguments>();
                });

                cfg.ReceiveEndpoint(host, "closeaccount", ep => {
                    ep.ExecuteActivityHost<CloseAccount, CloseAccountArguments>();
                });

                cfg.ReceiveEndpoint(host, "RoutingSlipEvents", ep => {
                    var partitioner = cfg.CreatePartitioner(16);
                    ep.RoutingSlipEventConsumers(_persister, partitioner);
                    ep.RoutingSlipActivityEventConsumers(_persister, partitioner);
                });
            });

            _busControl.Start();
        }

        private RoutingSlip VoluntarilyCloseAccount(string username)
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            
            builder.AddActivity(
                nameof(GenerateFinalProratedInvoice), 
                new Uri("rabbitmq://localhost/generatefinalinvoice"), 
                new { Username = username });
            
            builder.AddActivity(
                nameof(ChargeAllOpenInvoices),
                new Uri("rabbitmq://localhost/chargeallopeninvoices"),
                new { Username = username });

            builder.AddActivity(
                nameof(CloseAccount),
                new Uri("rabbitmq://localhost/closeaccount"),
                new { Username = username });

            builder.AddSubscription(new Uri("rabbitmq://localhost/RoutingSlipEvents"), RoutingSlipEvents.All);
            return builder.Build();
        }

        public async Task Run()
        {
            await Console.Out.WriteLineAsync("Sending the routing slip...");
            await _busControl.Execute(VoluntarilyCloseAccount("username_bob"));
        }

        static void Main(string[] args)
        {
            new Program()
                .Run()
                .Wait();
        }
    }
}
