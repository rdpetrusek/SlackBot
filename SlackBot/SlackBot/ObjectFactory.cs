using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBot
{
    public static class ObjectFactory
    {
        private static Container _container;

        public static TReturn Get<TReturn>()
        {
            if(_container == null)
            {
                _container = StructureMapConfiguration.Initialize();
            }
            return _container.GetInstance<TReturn>();
        }
    }
}
