using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using Ifeng.Utility.ConfigManager;
using Ifeng.Utility.Helper;

namespace Ifeng.Utility.Ioc
{
    // todo: ���߳�����(��������߳�ͬ��֧�ֵ�ԭ��һ����˵ע�����ֻ��������������ʱ���Ժ����ж�̬ע�������Ҫ�ٸ�)
    public class IocContainer
    {
		public static readonly IocContainer Default;

        private Dictionary<Type, Component> _ComponentList;

		static IocContainer()
		{
			Default = new IocContainer();
		}

        private IocContainer()
        {
            string filePath = ConfigBase.GetConfigFilePath("Service.config", false);
            if (File.Exists(filePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                Init(doc.DocumentElement.FirstChild as XmlElement);
            }
            else _ComponentList = new Dictionary<Type, Component>();
        }

        public IocContainer(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");
            if (!File.Exists(filePath)) throw new Exception("�Ҳ��������ļ�: " + filePath);

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            Init(doc.DocumentElement.FirstChild as XmlElement);
        }

        public IocContainer(XmlElement componentsElem)
        {
            Init(componentsElem);
        }

        private void Init(XmlElement componentsElem)
        {
            _ComponentList = new Dictionary<Type, Component>();
            foreach (XmlNode node in componentsElem.ChildNodes)
            {
                XmlElement elem = node as XmlElement;
                if (elem != null)
                {
                    Component component = new Component(elem);
                    _ComponentList.Add(component.Service, component);
                }
            }
        }

        public void Register(string name, Type serviceType, Type type, params object[] args)
        {
            object instance = Activator.CreateInstance(type, args);
            Component component = new Component(name, true, serviceType, instance);
            _ComponentList.Add(component.Service, component);
        }

        public void UnRegister(Type serviceType)
        {
            _ComponentList.Remove(serviceType);
        }

        /// <summary>
        /// ��ȡ���ʵ��
        /// </summary>
        /// <param name="absType">�ӿ�����</param>
        /// <returns>���ʵ��</returns>
        public object GetInstance(Type serviceType)
        {
            Component component;
            if (_ComponentList.TryGetValue(serviceType, out component))
            {
                return component.Instance;
            }

            return null;
        }

        /// <summary>
        /// ��ȡ���ʵ��
        /// </summary>
        /// <typeparam name="T">�ӿ�����</typeparam>
        /// <returns>���ʵ��</returns>
        public T GetInstance<T>()
        {
            object result = GetInstance(typeof(T));
            if (result == null) return default(T);
            return (T)result;
        }

    }
}
