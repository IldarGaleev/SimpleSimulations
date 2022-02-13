using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmIntelligence.Primitives
{
    public class Ant:Insect
    {
        //public class MealInfo
        //{
        //    public double Distance { get; set; }
        //    public bool Available { get; set; }
        //}
        
        private Random random;
        public int Target { get; set; }
        public List<double> Distances { get; set; }
        public bool Finder { get; set; }

        public double Speed { get; set; }

        public Ant(Vector2d position,double listenRadius=0.08):
            base(position,listenRadius)
        {
            Distances = new List<double>();
            random = new Random();
        }

        public override void Step(double timeDelta)
        {
            Rotate((0.5 - random.NextDouble())*10);
            base.Step(timeDelta);

            for(int i=0;i<Distances.Count;i++)
            {
                Distances[i]+=timeDelta;                
            }
            //if (Distances[Target] > (3/AccelerationMagnitude))
            //{
            //    Distances[Target] /= 2;
            //    Rotate(90);
            //}
        }
        int totalStep = 0;
        public bool Meet(Ant other)
        {
            if (other==this)
            {
                return false;
            }
            double distance = Vector2d.Distance(Position, other.Position);
            bool result=(distance<=_listenRadius);
            if (result)
            {
                //if (distance < 0.1 && (Acceleration + other.Acceleration).Length < Vector2d.MagnitudeMax(Acceleration, other.Acceleration).Length)
                //{
                //    //double r = (0.5-random.NextDouble())*2;
                //    double r = 0.2;
                //    other.Rotate(r);
                //    Rotate(r);
                //}
                for (int i= 0; i < Distances.Count; i++)
                {
                    
                    double newDistance = other.Distances[i] + ListenRadius*2;
                    if (Distances[i]>newDistance)
                    {
                        Distances[i] = newDistance;

                        if ((i==Target) && !Finder)
                        {
                            if (newDistance>40)
                            {
                                totalStep++;
                            }
                            if (totalStep>25)
                            {
                                DirectTo(Vector2d.Zero, _accelerationMagnitude);
                                totalStep = 0;
                                return false;
                            }
                            //if (!Distances[item].Available)
                            //{
                            //    Target = other.Target;
                            //}
                            //if (totalStep > 150)
                            //{
                            //    totalStep = -150;
                            //    Position -= Acceleration;
                            //    Rotate(100);
                            //    return false;
                            //}
                            //else
                            {
                                DirectTo(other.Position, _accelerationMagnitude);
                            }
                            
                        }
                    }
                }
            }
            return result;
        }


        public void NextTarget(int[] list=null)
        {
            
            if (Target == 0 || (list?.Length??0)>0)
            {
                if (list.Length==0)
                {
                    Target = 0; 
                    return;
                }
                totalStep = 0;
                Target = list[random.Next(0, list.Length)];
            }
            else
            {
                Target = 0;
            }
            
        }
    }
}
