using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ink.Runtime;
using System.IO;
using RGiesecke.DllExport;
using System.Runtime.InteropServices;

namespace GMWolf.GMInk
{
    using GML;

    public class GMInk
    {
        private static Story story;

        [DllExport("Load", CallingConvention = CallingConvention.Cdecl)]
        public static double Load(string file)
        {
            string json = File.ReadAllText(file);
            story = new Story(json);

            return 1;
        }


        [DllExport("CanContinue", CallingConvention = CallingConvention.Cdecl)]
        public static double CanContinue()
        {
            return (story?.canContinue ?? false) ? 1 : 0 ;
        }


        [DllExport("Continue", CallingConvention = CallingConvention.Cdecl)]
        public static string Continue()
        {
            return  story?.Continue() ?? "";
        }


        [DllExport("CurrentChoicesCount", CallingConvention = CallingConvention.Cdecl)]
        public static double CurrentChoicesCount()
        {
            return story?.currentChoices.Count ?? 0;
        }


        [DllExport("CurrentChoices", CallingConvention = CallingConvention.Cdecl)]
        public static string CurrentChoice(double i)
        {
            return story?.currentChoices?[(int)i]?.text ?? "";
        }


        [DllExport("ChooseChoiceIndex", CallingConvention = CallingConvention.Cdecl)]
        public static void ChooseChoiceIndex(double i)
        {
            story?.ChooseChoiceIndex((int)i);
        }


        [DllExport("SaveState", CallingConvention = CallingConvention.Cdecl)]
        public static string SaveState()
        {
            return story?.state.ToJson() ?? "";
        }


        [DllExport("LoadState", CallingConvention = CallingConvention.Cdecl)]
        public static void LoadState(string json)
        {
            if (story)
            {
                story.state.LoadJson(json);
            }
        }

        [DllExport("TagCount", CallingConvention = CallingConvention.Cdecl)]
        public static double TagCount()
        {
            return TagCountInternal();
        }

        public static double TagCountInternal()
        {
            return story?.currentTags?.Count ?? 0;
        }

        [DllExport("GetTag", CallingConvention = CallingConvention.Cdecl)]
        public static string GetTag(double i)
        {
            if ((int)i < TagCountInternal() && i >= 0)
            {
                return story?.currentTags?[(int)i] ?? "";
            } else
            {
                return "";
            }
        }

        [DllExport("TagForContentAtPathCount", CallingConvention = CallingConvention.Cdecl)]
        public static double TagForContentAtPathCount(string path)
        {
            return story?.TagsForContentAtPath(path)?.Count ?? 0;
        }

        [DllExport("TagForContentAtPath", CallingConvention = CallingConvention.Cdecl)]
        public static string TagForContentAtPath(string path, double i)
        {
            return story?.TagsForContentAtPath(path)?[(int)i] ?? "";
        }

        [DllExport("GlobalTagCount", CallingConvention = CallingConvention.Cdecl)]
        public static double GlobalTagCount()
        {
            return story?.globalTags?.Count ?? 0;
        }

        [DllExport("GlobalTag", CallingConvention = CallingConvention.Cdecl)]
        public static string GlobalTag(double i)
        {
            return story?.globalTags?[(int)i] ?? "";
        }

        [DllExport("ChoosePathString", CallingConvention = CallingConvention.Cdecl)]
        public static void ChoosePathString(string path)
        {
            story?.ChoosePathString(path);
        }

        [DllExport("VariableGetReal", CallingConvention = CallingConvention.Cdecl)]
        public static double VariableGetReal(string var)
        {
            object o = story?.variablesState?[var] ?? 0;
            return double.Parse(o.ToString());
        }

        [DllExport("VariableGetString", CallingConvention = CallingConvention.Cdecl)]
        public static string VariableGetString(string var)
        {
            return (story?.variablesState?[var] ?? "").ToString();
        }

        [DllExport("VariableSetReal", CallingConvention = CallingConvention.Cdecl)]
        public static void VariableSetReal(string var, double value)
        {
            if (story)
            {
                story.variablesState[var] = value;
            }
        }

        [DllExport("VariableSetString", CallingConvention = CallingConvention.Cdecl)]
        public static void VariableSetString(string var, string value)
        {
            if (story)
            {
                story.variablesState[var] = value;
            }
        }

        [DllExport("VisitCountAtPathString", CallingConvention = CallingConvention.Cdecl)]
        public static double VisitCountAtPathString(string path)
        {
            return story?.state?.VisitCountAtPathString(path) ?? 0;
        }

       
        [DllExport("ObserveVariable", CallingConvention = CallingConvention.Cdecl)]
        public static void ObserveVariable(String name, double script)
        { 
            story?.ObserveVariable(name, (string varName, object value) =>
            {
                GML.CallScript(script, varName, value);
            });
        }

        [DllExport("BindExternal", CallingConvention = CallingConvention.Cdecl)]
        public static void BindExternal(string name, double script)
        {
            story?.BindExternalFunctionGeneral(name, (object[] args) =>
            {
                GML.CallScript(script, args);
                return 0;
            });
        }

    }
}
