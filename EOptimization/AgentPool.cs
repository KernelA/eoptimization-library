// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt
{
    using System;
    using System.Collections.Generic;

    using EOpt.Math.Optimization;

    public class AgentPool
    {
        private readonly int _maxSize;

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

        /// <summary>
        /// maximum size of pool
        /// </summary>
        public int MaxSize => _maxSize;

        /// <summary>
        /// Create pool
        /// </summary>
        /// <param name="MaxSize"></param>
        /// <param name="Creator"></param>
        public AgentPool(int MaxSize, IAgentCreator Creator)
        {
            _creator = Creator;
            _maxSize = MaxSize;
            _pool = new Stack<Agent>(MaxSize);
        }

        /// <summary>
        /// Get free agent from pool.
        /// </summary>
        /// <returns></returns>
        public Agent GetAgent()
        {
            if (_pool.Count != 0)
            {
                return _pool.Pop();
            }
            else
            {
                return _creator.Create();
            }
        }

        /// <summary>
        /// Add agent to pool.
        /// </summary>
        /// <param name="Agent"></param>
        public void AddAgent(Agent Agent)
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                if (Agent == _pool.Peek())
                {
                    throw new InvalidOperationException("Pool object ref equals.");
                }
            }
            if (_pool.Count < _maxSize)
            {
                _pool.Push(Agent);
            }
        }

        /// <summary>
        /// Clear pool.
        /// </summary>
        public void Clear()
        {
            _pool.Clear();
        }
    }
}