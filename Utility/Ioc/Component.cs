using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Ifeng.Utility.Ioc
{
    internal class Component
    {
        private string _Name;
        private string _Type;
        private bool _Reusable;
        private Type _Service;
        private object _Instance;

        public Component(XmlElement elem)
        {
            if (elem == null) throw new ArgumentNullException("elem");

            _Name = elem.GetAttribute("name");
            string reusable = elem.GetAttribute("reusable");
            _Reusable = reusable.Length == 0 ? true : bool.Parse(reusable);

            string service = elem.GetAttribute("service");
            _Type = elem.GetAttribute("type");
            _Service = Type.GetType(service);
            if (_Service == null)
            {
                string error = string.Format("服务初始化失败，服务名称为:{0}", _Name);
                throw new ServiceInitException(_Name, error);
            }
        }

        public Component(string name, bool reusable, Type service, object instance)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (instance == null) throw new ArgumentNullException("instance");

            _Name = name;
            _Reusable = reusable;
            _Service = service;
            _Instance = instance;
        }

        public bool Reusable
        {
            get { return _Reusable; }
            set { _Reusable = value; }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public Type Service
        {
            get
            {
                return _Service;
            }
            set
            {
                _Service = value;
            }
        }

        public object Instance
        {
            get
            {
                if (_Instance == null || !_Reusable) _Instance = Activator.CreateInstance(Type.GetType(_Type));
                return _Instance;
            }
            //set
            //{
            //    _Instance = value;
            //}
        }

    }
}
