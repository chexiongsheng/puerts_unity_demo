namespace PuertsDeclareTest {
    
    public class Plants {
        public interface Shootable {
            void shoot();
        }

        public class Pumpkin<T> {
            public interface Protectable {
                void protect();
            }
        }

        public class pumkinPeaShooter: Pumpkin<pumkinPeaShooter>.Protectable, Shootable {
            public void shoot() {

            }

            public void protect() {

            }
        }
    }

    public class Zombies {
        public interface Flyable {
            void action();
        }
        public interface Walkable {
            void action();
        }

        public class BalloonZombie : Flyable, Walkable {
            void Flyable.action() {

            }
            void Walkable.action() {

            }
        }
    }

}