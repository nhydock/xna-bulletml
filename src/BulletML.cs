﻿using System;
using System.Collections.Generic;

namespace github.io.nhydock.BulletML
{
    public class GameManager
    {
        public static int GameDifficulty()
        {
            return 0;
        }
    }

    namespace Specification
    {
        using System.Xml.Serialization;
        
        public class TaskNode { }

        public interface BulletMLNode { }

        [Serializable()]
        public class Reference<T> : TaskNode where T : BulletMLNode
        {
            [XmlAttribute(AttributeName = "label")]
            [XmlText]
            public string Label;

            [XmlElement("param", typeof(Param))]
            public Param[] Parameters;
            
            public float[] GetParams(float[] param)
            {
                float[] eval = new float[Parameters.Length];
                for (int i = 0; i < eval.Length; i++)
                {
                    eval[i] = Parameters[i].GetValue(param);
                }
                return eval;
            }
        }
    }

    namespace Implementation
    {
        using Specification;

        public abstract class Step
        {
            public static float[] NO_PARAM = new float[0];

            protected IBullet _bullet;
            public IBullet Bullet {
                set
                {
                    _bullet = value;
                    SetBullet(value);
                }
                get
                {
                    return _bullet;
                }
            }
            public TaskNode Node;
            public float[] ParamList;

            public Step(TaskNode node, float[] Parameters)
            {
                Node = node;
                ParamList = Parameters;
            }

            abstract protected bool IsDone();
            public bool Done
            {
                get { return IsDone(); }
            }

            virtual public void Finish() { }
            virtual protected void SetBullet(IBullet bullet)
            {

            }
            virtual public void Reset() { }
            virtual public void UpdateParameters(float[] Parameters) {
                ParamList = Parameters;
            }
        }

        class StepFactory
        {
            public static Step make(TaskNode node, BulletMLSpecification spec, float[] parameters)
            {
                if (node is Action)
                {
                    return new Sequence((Action)node, spec, parameters);
                }
                else if (node is Reference<Action>)
                {
                    return new Sequence((Reference<Action>)node, spec, parameters);
                }
                else if (node is Repeat)
                {
                    return new RepeatSequence((Repeat)node, spec, parameters);
                }
                else if (node is Fire)
                {
                    return new FireBullet((Fire)node, spec, parameters);
                }
                else if (node is Reference<Fire>)
                {
                    return new FireBullet((Reference<Fire>)node, spec, parameters);
                }
                else if (node is Vanish)
                {
                    return new RemoveSelf();
                }
                else if (node is ChangeDirection)
                {
                    return new SetDirectionMutation((ChangeDirection)node, parameters);
                }
                else if (node is ChangeSpeed)
                {
                    return new SetSpeedMutation((ChangeSpeed)node, parameters);
                }
                else if (node is Delay)
                {
                    return new TimedStep((Delay)node, parameters);
                }
                return null;
            }
        }
    }
}
