# MassTransitCourierSample
Demonstration of courier usage in a project

## Requirements
1. We can build and execute routing slips to represent longer running processes.
2. We can audit the execution outcome of a routing slip. In fact, we get a complete view of the activities or compensations that ran as well as the ultimate outcome (completed, faulted, compensated, compensation failed).
3. The activities are composable rather than a string of consumers that are tightly coupled to a single business process.

## Strengths
1. Activities are not that difficult to configure on the bus. We just call `ExecuteActivityHost` on the `IReceiveEndpointConfigurator` object.
2. We can use the MongoDB integration for free.
3. The MassTransit testing framework has harnesses we can use to test activities.

## Weaknesses
Routing slip events and activity events are asynchronous with respect to the activities and compensations themselves. This poses a challenge when persisting the routing slip events to MongoDB because the events could arrive out of order. I have seen some really funky non-deterministic behavior with this, including the creation of multiple documents for the same routing slip, with random events in one document and random events in another.

In my testing, I was able to overcome this by calling `UsePartitioner(16)` in the constructor that wires the bus. I am not exactly sure what a partitioner is, but it seems to be some sort of pattern in GreenPipes that serializes threads to prevent races. Again, in manual testing, I found that this causes exactly one Mongo document to be created for the routing slip, and the events appear in the array in the actual order of execution. However, while that achieved the desired result, I haven't yet tested whether this will work under load, how the parameter `16` is related to the thread synchronization, or how scalable this is.

## Overall
For infrequently run processes, using the partitioner might work, and not create performance issues. Courier and this kind of routing slip persistence pattern might be useful if the process is critical enough that it requires tracking.
