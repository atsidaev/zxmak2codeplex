﻿using System;
using System.Collections.Generic;
using ZXMAK2.Dependency;
using ZXMAK2.Presentation.Interfaces;


namespace ZXMAK2.Presentation
{
    public class ViewResolver : IViewResolver
    {
        private readonly IEnumerable<string> m_viewTypes;
        private readonly IResolver m_resolver;
        
        public ViewResolver(IResolver resolver, string viewTypes)
        {
            m_resolver = resolver;
            var list = new List<string>();
            foreach (var type in viewTypes.Split(','))
            {
                list.Add(type.Trim());
            }
            m_viewTypes = list.ToArray();
        }

        public T Resolve<T>(params Argument[] arguments)
        {
            foreach (var viewType in m_viewTypes)
            {
                var view = m_resolver.TryResolve<T>(viewType);
                if (view != null)
                {
                    return view;
                }
            }
            return m_resolver.TryResolve<T>("WinForms");
        }
    }
}