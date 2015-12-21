using Automatonymous;

namespace aXon.RX02.ControlServer
{
    public class RobotStateMachine : AutomatonymousStateMachine<RobotState>
    {
        public RobotStateMachine()
        {
            State(() => Charging);
            State(() => WaitingForJob);
            State(() => Offline);
            State(() => Traveling);
            State(() => Pickup);
            State(() => Putaway);
            State(() => ObstructionInPath);

            Event(() => AuthenticateEvent);
            Event(() => ReadyForWorkEvent);
            Event(() => ArrivedAtDestinationEvent);
            Event(() => PickUpEvent);
            Event(() => PutAwayEvent);
            Event(() => JobArrivesEvent);
            Event(() => ChargeCompleteEvent);
            Event(() => BatteryDeadEvent);
            Event(() => ObstructionEvent);


            Initially(
                When(AuthenticateEvent)
                    .TransitionTo(Authenticating),
                When(ReadyForWorkEvent)
                    .TransitionTo(WaitingForJob),
                When(ArrivedAtDestinationEvent)
                    .TransitionTo(Traveling),
                When(PickUpEvent)
                    .TransitionTo(Pickup),
                When(PutAwayEvent)
                    .TransitionTo(Putaway),
                When(JobArrivesEvent)
                    .TransitionTo(Traveling),
                When(ChargeCompleteEvent)
                    .TransitionTo(WaitingForJob),
                When(BatteryDeadEvent)
                    .TransitionTo(Charging),
                When(ObstructionEvent)
                    .TransitionTo(ObstructionInPath)
                //When(PissOff)
                //    .TransitionTo(Enemy),
                //When(Introduce)
                //    .Then((instance, data) => instance.SerialNumber = data.SerialNumber)
                //    .TransitionTo(Friend)
                );
        }

        public State Authenticating { get; private set; }
        public State Charging { get; private set; }
        public State WaitingForJob { get; private set; }
        public State Offline { get; private set; }
        public State Traveling { get; private set; }
        public State Pickup { get; private set; }
        public State Putaway { get; private set; }
        public State ObstructionInPath { get; private set; }


        public Event AuthenticateEvent { get; private set; }
        public Event ReadyForWorkEvent { get; private set; }
        public Event ArrivedAtDestinationEvent { get; private set; }
        public Event PickUpEvent { get; private set; }
        public Event PutAwayEvent { get; private set; }
        public Event JobArrivesEvent { get; private set; }
        public Event ChargeCompleteEvent { get; private set; }
        public Event BatteryDeadEvent { get; private set; }
        public Event ObstructionEvent { get; private set; }
    }
}