﻿using DotNetCommon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UIowaBuildingsConsoleApp
{
    public class CommandLineInputHelpers
    {
        public static T BindArgumentsToType<T>(string[] args) where T : new()
        {
            T obj = new T();
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                string[] matchingArgs = args.Where(arg => arg.StartsWith("-") && arg.CapsAndTrim().TrimStart('-') == prop.Name.CapsAndTrim()).ToArray();
                if (matchingArgs.Count() > 1) throw new ArgumentOutOfRangeException("Duplicate argument in the input");
                if (matchingArgs.Count() == 1)
                {
                    string matchingArg = matchingArgs.First();
                    int argIndex = Array.IndexOf(args, matchingArg);
                    if (argIndex + 1 <= args.LastIndex<string>() && !args[argIndex + 1].StartsWith("-"))
                    {
                        string argValue = args[argIndex + 1];
                        prop.SetValueWithTypeRespect(obj, argValue);
                    }
                }
            }

            return obj;
        }
    }
}
