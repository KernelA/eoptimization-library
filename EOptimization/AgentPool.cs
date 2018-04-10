// This is an open source non-commercial project.Dear PVS-Studio, please check it.PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt
{
    using System;

    using EOpt.Math.Optimization;
    using System.Collections;
    using System.Collections.Generic;

    public class AgentPool
    {
        private int _maxSize;

        private Stack<Agent> _pool;

        private IAgentCreator _creator;

        public IAgentCreator Creator
        {
            get => _creator;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                else
                {
                    _creator = value;
                }
            }
        }

        public int MaxSize => _maxSize;

        public AgentPool(int MaxSize, IAgentCreator Creator)
        {
            _creator = Creator;
            _maxSize = MaxSize;
            _pool = new Stack<Agent>(MaxSize);
        }

        public Agent GetAgent()
        {
            if(_pool.Count != 0)
            {
                return _pool.Pop();
            }
            else
            {
                return _creator.Create();
            }
        }

        public void AddAgent(Agent Agent)
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                if(Agent == _pool.Peek())
                {
                    throw new InvalidOperationException("Pool object ref equals.");
                }
            }
            if(_pool.Count < _maxSize)
            {
                _pool.Push(Agent);
            }
        }

        public void Clear()
        {
            _pool.Clear();
        }
    }
}
