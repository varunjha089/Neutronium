﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM.CEFGlue.Infra
{
    public static class DictionaryExtension
    {
        public static TValue FindOrCreateEntity<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, Func<TKey, TValue> Fac)
        {
            TValue res = default(TValue);
            if (dic.TryGetValue(key, out res))
                return res;

            res = Fac(key);
            dic.Add(key, res);
            return res;
        }
    }
}
