using Akka.Persistence.Fsm;

namespace DDD.Aggregate.Stereotypes.Aggregate;

public interface IAggregateState : PersistentFSM.IFsmState
{
}