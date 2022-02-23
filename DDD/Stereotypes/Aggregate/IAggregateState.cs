using Akka.Persistence.Fsm;

namespace DDD.Stereotypes.Aggregate;

public interface IAggregateState : PersistentFSM.IFsmState
{
}
