let conf = ConfigurationFactory.ParseString(
        @"akka {
            actor {
                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                deployment {
                    /remoteActor {
                        remote = ""akka.tcp://Target@localhost:8080""
                    }
                }
            }
            remote {
                helios.tcp {
                    port = 0
                    hostname = localhost
                }
            }
        }")
let system = ActorSystem.Create("RemoteActor", configuration)
//Would need to build a new actor hear to communicate with the target machine and recieve data from it
