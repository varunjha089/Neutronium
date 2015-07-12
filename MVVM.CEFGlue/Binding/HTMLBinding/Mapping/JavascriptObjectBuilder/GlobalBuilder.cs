﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xilium.CefGlue;

using MVVM.CEFGlue.CefGlueHelper;

namespace MVVM.CEFGlue.HTMLBinding
{
    public class GlobalBuilder : IGlobalBuilder
    {
        private static uint _Count = 0;
        private string _NameScape;
        private CefV8Context _CefV8Context;

        public GlobalBuilder(CefV8Context iWebView, string iNameScape)
        {
            _CefV8Context = iWebView;
            _NameScape = iNameScape;
        }

        public CefV8Value CreateJSO()
        {
            //string Name = string.Format("{0}_{1}", _NameScape, ++_Count);
            return _CefV8Context.EvaluateAsync(() =>
                {
                    _CefV8Context.Enter();
                    var res = CefV8Value.CreateObject(null);
                    res.SetValue("_globalId_", CefV8Value.CreateUInt(++_Count),CefV8PropertyAttribute.DontDelete);
                        //_CefV8Context.CreateGlobalJavascriptObject(Name);
                    _CefV8Context.Exit();
                    return res;
                }).Result;
        }


        public uint GetID(CefV8Value iJSObject)
        {
            return _CefV8Context.EvaluateInCreateContextAsync(() =>
            {
                return iJSObject.GetValue("_globalId_").GetUIntValue();
            }).Result;
        }

        public uint CreateAndGetID(CefV8Value iJSObject)
        {
            return _CefV8Context.EvaluateInCreateContextAsync(() =>
            {
                var value = iJSObject.GetValue("_globalId_");
                if (value.IsUInt)
                    return value.GetUIntValue();

                iJSObject.SetValue("_globalId_", CefV8Value.CreateUInt(++_Count), CefV8PropertyAttribute.DontDelete);
                return _Count;
            }).Result;
        }


        public bool HasRelevantId(CefV8Value iJSObject)
        {    
            if (iJSObject.IsUserCreated)
                    return false;

            return _CefV8Context.EvaluateAsync(() =>
            {
                return iJSObject.HasValue("_globalId_");
            }).Result;
        }

    }
}
