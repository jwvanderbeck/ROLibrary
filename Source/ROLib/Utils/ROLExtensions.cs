using System;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic;

namespace ROLib
{
    public static class ROLExtensions
    {
        #region ConfigNode extension methods

        public static String[] ROLGetStringValues(this ConfigNode node, String name, bool reverse = false)
        {
            string[] values = node.GetValues(name);
            int l = values.Length;
            if (reverse)
            {
                int len = values.Length;
                string[] returnValues = new string[len];
                for (int i = 0, k = len - 1; i < len; i++, k--)
                {
                    returnValues[i] = values[k];
                }
                return returnValues;
            }
            return values;
        }

        public static string[] ROLGetStringValues(this ConfigNode node, string name, string[] defaults, bool reverse = false)
        {
            if (node.HasValue(name)) { return node.ROLGetStringValues(name, reverse); }
            return defaults;
        }

        public static string ROLGetStringValue(this ConfigNode node, String name, String defaultValue)
        {
            String value = node.GetValue(name);
            return value == null ? defaultValue : value;
        }

        public static string ROLGetStringValue(this ConfigNode node, String name)
        {
            return ROLGetStringValue(node, name, "");
        }

        public static bool[] ROLGetBoolValues(this ConfigNode node, String name)
        {
            String[] values = node.GetValues(name);
            int len = values.Length;
            bool[] vals = new bool[len];
            for (int i = 0; i < len; i++)
            {
                vals[i] = ROLUtils.safeParseBool(values[i]);
            }
            return vals;
        }

        public static bool ROLGetBoolValue(this ConfigNode node, String name, bool defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null) { return defaultValue; }
            try
            {
                return bool.Parse(value);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e.Message);
            }
            return defaultValue;
        }

        public static bool ROLGetBoolValue(this ConfigNode node, String name)
        {
            return ROLGetBoolValue(node, name, false);
        }

        public static float[] ROLGetFloatValues(this ConfigNode node, String name, float[] defaults)
        {
            String baseVal = node.ROLGetStringValue(name);
            if (!String.IsNullOrEmpty(baseVal))
            {
                String[] split = baseVal.Split(new char[] { ',' });
                float[] vals = new float[split.Length];
                for (int i = 0; i < split.Length; i++) { vals[i] = ROLUtils.safeParseFloat(split[i]); }
                return vals;
            }
            return defaults;
        }

        public static float[] ROLGetFloatValues(this ConfigNode node, String name)
        {
            return ROLGetFloatValues(node, name, new float[] { });
        }

        public static float[] ROLGetFloatValuesCSV(this ConfigNode node, String name)
        {
            return ROLGetFloatValuesCSV(node, name, new float[] { });
        }

        public static float[] ROLGetFloatValuesCSV(this ConfigNode node, String name, float[] defaults)
        {
            float[] values = defaults;
            if (node.HasValue(name))
            {
                string strVal = node.ROLGetStringValue(name);
                string[] splits = strVal.Split(',');
                values = new float[splits.Length];
                for (int i = 0; i < splits.Length; i++)
                {
                    values[i] = float.Parse(splits[i]);
                }
            }
            return values;
        }

        public static float ROLGetFloatValue(this ConfigNode node, String name, float defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null) { return defaultValue; }
            try
            {
                return float.Parse(value);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e.Message);
            }
            return defaultValue;
        }

        public static float ROLGetFloatValue(this ConfigNode node, String name)
        {
            return ROLGetFloatValue(node, name, 0);
        }

        public static double ROLGetDoubleValue(this ConfigNode node, String name, double defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null) { return defaultValue; }
            try
            {
                return double.Parse(value);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e.Message);
            }
            return defaultValue;
        }

        public static double ROLGetDoubleValue(this ConfigNode node, String name)
        {
            return ROLGetDoubleValue(node, name, 0);
        }

        public static int ROLGetIntValue(this ConfigNode node, String name, int defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null) { return defaultValue; }
            try
            {
                return int.Parse(value);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e.Message);
            }
            return defaultValue;
        }

        public static int ROLGetIntValue(this ConfigNode node, String name)
        {
            return ROLGetIntValue(node, name, 0);
        }

        public static int[] ROLGetIntValues(this ConfigNode node, string name, int[] defaultValues = null)
        {
            int[] values = defaultValues;
            string[] stringValues = node.GetValues(name);
            if (stringValues == null || stringValues.Length == 0) { return values; }
            int len = stringValues.Length;
            values = new int[len];
            for (int i = 0; i < len; i++)
            {
                values[i] = ROLUtils.safeParseInt(stringValues[i]);
            }
            return values;
        }

        public static Vector3 ROLGetVector3(this ConfigNode node, String name, Vector3 defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null)
            {
                return defaultValue;
            }
            String[] vals = value.Split(',');
            if (vals.Length < 3)
            {
                ROLLog.error("ERROR parsing values for Vector3 from input: " + value + ". found less than 3 values, cannot create Vector3");
                return defaultValue;
            }
            return new Vector3((float)ROLUtils.safeParseDouble(vals[0]), (float)ROLUtils.safeParseDouble(vals[1]), (float)ROLUtils.safeParseDouble(vals[2]));
        }

        public static Vector3 ROLGetVector3(this ConfigNode node, String name)
        {
            String value = node.GetValue(name);
            if (value == null)
            {
                ROLLog.error("ERROR: No value for name: " + name + " found in config node: " + node);
                return Vector3.zero;
            }
            String[] vals = value.Split(',');
            if (vals.Length < 3)
            {
                ROLLog.error("ERROR parsing values for Vector3 from input: " + value + ". found less than 3 values, cannot create Vector3");
                return Vector3.zero;
            }
            return new Vector3((float)ROLUtils.safeParseDouble(vals[0]), (float)ROLUtils.safeParseDouble(vals[1]), (float)ROLUtils.safeParseDouble(vals[2]));
        }

        public static FloatCurve ROLGetFloatCurve(this ConfigNode node, String name, FloatCurve defaultValue = null)
        {
            FloatCurve curve = new FloatCurve();
            if (node.HasNode(name))
            {
                ConfigNode curveNode = node.GetNode(name);
                String[] values = curveNode.GetValues("key");
                int len = values.Length;
                String[] splitValue;
                float a, b, c, d;
                for (int i = 0; i < len; i++)
                {
                    splitValue = Regex.Replace(values[i], @"\s+", " ").Split(' ');
                    if (splitValue.Length > 2)
                    {
                        a = ROLUtils.safeParseFloat(splitValue[0]);
                        b = ROLUtils.safeParseFloat(splitValue[1]);
                        c = ROLUtils.safeParseFloat(splitValue[2]);
                        d = ROLUtils.safeParseFloat(splitValue[3]);
                        curve.Add(a, b, c, d);
                    }
                    else
                    {
                        a = ROLUtils.safeParseFloat(splitValue[0]);
                        b = ROLUtils.safeParseFloat(splitValue[1]);
                        curve.Add(a, b);
                    }
                }
            }
            else if (defaultValue != null)
            {
                foreach (Keyframe f in defaultValue.Curve.keys)
                {
                    curve.Add(f.time, f.value, f.inTangent, f.outTangent);
                }
            }
            else
            {
                curve.Add(0, 0);
                curve.Add(1, 1);
            }
            return curve;
        }

        public static ConfigNode getNode(this FloatCurve curve, string name)
        {
            ConfigNode node = new ConfigNode(name);
            foreach (Keyframe key in curve.Curve.keys)
            {
                node.AddValue("key", $"{key.time} {key.value} {key.inTangent} {key.outTangent}");
            }
            return node;
        }

        public static Color getColor(this ConfigNode node, String name)
        {
            Color color = new Color();
            float[] vals = node.ROLGetFloatValuesCSV(name);
            color.r = vals[0];
            color.g = vals[1];
            color.b = vals[2];
            color.a = vals[3];
            return color;
        }

        public static Color ROLgetColorFromByteValues(this ConfigNode node, String name)
        {
            Color color = new Color();
            float[] vals = node.ROLGetFloatValuesCSV(name);
            color.r = vals[0]/255f;
            color.g = vals[1]/255f;
            color.b = vals[2]/255f;
            color.a = vals[3]/255f;
            return color;
        }

        public static Axis getAxis(this ConfigNode node, string name, Axis def = Axis.ZPlus)
        {
            string val = node.ROLGetStringValue(name, def.ToString());
            Axis axis = (Axis)Enum.Parse(typeof(Axis), val, true);
            return axis;
        }

        #endregion

        #region Transform extensionMethods

        /// <summary>
        /// Same as transform.FindChildren() but also searches for children with the (Clone) tag on the name.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static Transform[] ROLFindModels(this Transform transform, String modelName)
        {
            Transform[] trs = transform.ROLFindChildren(modelName);
            Transform[] trs2 = transform.ROLFindChildren(modelName + "(Clone)");
            Transform[] trs3 = new Transform[trs.Length + trs2.Length];
            trs3.AddUniqueRange(trs);
            trs3.AddUniqueRange(trs2);
            return trs3;
        }

        /// <summary>
        /// Same as transform.FindRecursive() but also searches for models with "(Clone)" added to the end of the transform name
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static Transform ROLFindModel(this Transform transform, String modelName)
        {
            if (transform.ROLFindRecursive(modelName) is Transform tr) return tr;
            return transform.ROLFindRecursive(modelName + "(Clone)");
        }

        /// <summary>
        /// Same as transform.FindRecursive() but returns an array of all children with that name under the entire heirarchy of the model
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform[] ROLFindChildren(this Transform transform, String name)
        {
            List<Transform> trs = new List<Transform>();
            if (transform.name == name) trs.Add(transform);
            ROLlocateTransformsRecursive(transform, name, trs);
            return trs.ToArray();
        }

        private static void ROLlocateTransformsRecursive(Transform tr, String name, List<Transform> output)
        {
            foreach (Transform child in tr)
            {
                if (child.name == name) { output.Add(child); }
                ROLlocateTransformsRecursive(child, name, output);
            }
        }

        /// <summary>
        /// Searches entire model heirarchy from the input transform to end of branches for transforms with the input transform name and returns the first match found, or null if none.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform ROLFindRecursive(this Transform transform, String name)
        {
            if (transform.name == name) { return transform; }//was the original input transform
            if (transform.Find(name) is Transform tr) return tr;    //found as a direct child
            foreach(Transform child in transform)
            {
                if (child.ROLFindRecursive(name) is Transform t) return t;
            }
            return null;
        }

        /// <summary>
        /// Uses transform.FindRecursive to search for the given transform as a child of the input transform; if it does not exist, it creates a new transform and nests it to the input transform (0,0,0 local position and scale).
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform ROLFindOrCreate(this Transform transform, String name)
        {
            if (transform.ROLFindRecursive(name) is Transform t) return t;
            GameObject newGO = new GameObject(name);
            newGO.SetActive(true);
            newGO.name = newGO.transform.name = name;
            newGO.transform.NestToParent(transform);
            return newGO.transform;
        }

        /// <summary>
        /// Returns -ALL- children/grand-children/etc transforms of the input; everything in the heirarchy.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Transform[] ROLGetAllChildren(this Transform transform)
        {
            List<Transform> trs = new List<Transform>();
            ROLrecurseAddChildren(transform, trs);
            return trs.ToArray();
        }

        private static void ROLrecurseAddChildren(Transform transform, List<Transform> trs)
        {
            foreach (Transform child in transform)
            {
                trs.Add(child);
                ROLrecurseAddChildren(child, trs);
            }
        }

        /// <summary>
        /// Returns true if the input 'isParent' transform exists anywhere upwards of the input transform in the heirarchy.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="isParent"></param>
        /// <param name="checkUpwards"></param>
        /// <returns></returns>
        public static bool ROLisParent(this Transform transform, Transform isParent, bool checkUpwards = true)
        {
            if (isParent == null) { return false; }
            if (isParent == transform.parent) { return true; }
            if (checkUpwards)
            {
                Transform p = transform.parent;
                if (p == null) { return false; }
                else { p = p.parent; }
                while (p != null)
                {
                    if (p == isParent) { return true; }
                    p = p.parent;
                }
            }
            return false;
        }

        public static Vector3 ROLgetTransformAxis(this Transform transform, Axis axis)
        {
            switch (axis)
            {
                case Axis.XPlus:
                    return transform.right;
                case Axis.XNeg:
                    return -transform.right;
                case Axis.YPlus:
                    return transform.up;
                case Axis.YNeg:
                    return -transform.up;
                case Axis.ZPlus:
                    return transform.forward;
                case Axis.ZNeg:
                    return -transform.forward;
                default:
                    return transform.forward;
            }
        }

        public static Vector3 ROLgetLocalAxis(this Transform transform, Axis axis)
        {
            switch (axis)
            {
                case Axis.XPlus:
                    return Vector3.right;
                case Axis.XNeg:
                    return Vector3.left;
                case Axis.YPlus:
                    return Vector3.up;
                case Axis.YNeg:
                    return Vector3.down;
                case Axis.ZPlus:
                    return Vector3.forward;
                case Axis.ZNeg:
                    return Vector3.back;
                default:
                    return Vector3.forward;
            }
        }

        #endregion

        #region PartModule extensionMethods

        public static void ROLsetFieldEnabledEditor(this PartModule module, string fieldName, bool active)
        {
            if (module.Fields[fieldName] is BaseField f) f.guiActiveEditor = active;
        }

        public static void ROLsetFieldEnabledFlight(this PartModule module, string fieldName, bool active)
        {
            if (module.Fields[fieldName] is BaseField f) f.guiActive = active;
        }

        private static UI_Control GetWidget(PartModule module, string fieldName)
        {
            if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor) return null;
            BaseField bf = module.Fields[fieldName];
            return HighLogic.LoadedSceneIsEditor ? bf.uiControlEditor : bf.uiControlFlight;
        }

        public static void ROLupdateUIFloatEditControl(this PartModule module, string fieldName, float min, float max, float incLarge, float incSmall, float incSlide, bool forceUpdate, float forceVal)
        {
            if (GetWidget(module, fieldName) is UI_FloatEdit widget)
            {
                widget.minValue = min;
                widget.maxValue = max;
                widget.incrementLarge = incLarge;
                widget.incrementSmall = incSmall;
                widget.incrementSlide = incSlide;
                if (forceUpdate && widget.partActionItem is UIPartActionFloatEdit  ctr)
                {
                    var t = widget.onFieldChanged;//temporarily remove the callback
                    widget.onFieldChanged = null;
                    ctr.incSmall.onToggle.RemoveAllListeners();
                    ctr.incLarge.onToggle.RemoveAllListeners();
                    ctr.decSmall.onToggle.RemoveAllListeners();
                    ctr.decLarge.onToggle.RemoveAllListeners();
                    ctr.slider.onValueChanged.RemoveAllListeners();
                    ctr.Setup(ctr.Window, module.part, module, HighLogic.LoadedSceneIsEditor ? UI_Scene.Editor : UI_Scene.Flight, widget, module.Fields[fieldName]);
                    widget.onFieldChanged = t;//re-seat callback
                }
            }
        }

        public static void ROLupdateUIFloatEditControl(this PartModule module, string fieldName, float newValue)
        {
            if (GetWidget(module, fieldName) is UI_FloatEdit widget)
            {
                BaseField field = module.Fields[fieldName];
                field.SetValue(newValue, field.host);
                //force widget re-setup for changed values; this will update the GUI value and slider positions/internal cached data
                if (widget.partActionItem is UIPartActionFloatEdit ctr)
                {
                    var t = widget.onFieldChanged;//temporarily remove the callback; we don't need an event fired when -we- are the ones editing the value...
                    widget.onFieldChanged = null;
                    ctr.incSmall.onToggle.RemoveAllListeners();
                    ctr.incLarge.onToggle.RemoveAllListeners();
                    ctr.decSmall.onToggle.RemoveAllListeners();
                    ctr.decLarge.onToggle.RemoveAllListeners();
                    ctr.slider.onValueChanged.RemoveAllListeners();
                    ctr.Setup(ctr.Window, module.part, module, HighLogic.LoadedSceneIsEditor ? UI_Scene.Editor : UI_Scene.Flight, widget, module.Fields[fieldName]);
                    widget.onFieldChanged = t;//re-seat callback
                }
            }
        }

        /// <summary>
        /// FOR EDITOR USE ONLY - will not update or activate UI fields in flight scene
        /// </summary>
        /// <param name="module"></param>
        /// <param name="fieldName"></param>
        /// <param name="options"></param>
        /// <param name="display"></param>
        /// <param name="forceUpdate"></param>
        /// <param name="forceVal"></param>
        public static void ROLupdateUIChooseOptionControl(this PartModule module, string fieldName, string[] options, string[] display, bool forceUpdate, string forceVal="")
        {
            if (display.Length == 0 && options.Length > 0) { display = new string[] { "NONE" }; }
            if (options.Length == 0) { options = new string[] { "NONE" }; }
            module.Fields[fieldName].guiActiveEditor = options.Length > 1;
            if (HighLogic.LoadedSceneIsEditor && GetWidget(module, fieldName) is UI_ChooseOption widget)
            {
                widget.display = display;
                widget.options = options;
                if (forceUpdate && widget.partActionItem is UIPartActionChooseOption ctr)
                {
                    var t = widget.onFieldChanged;
                    widget.onFieldChanged = null;
                    int index = Array.IndexOf(options, forceVal);
                    ctr.slider.minValue = 0;
                    ctr.slider.maxValue = options.Length - 1;
                    ctr.slider.value = index;
                    ctr.OnValueChanged(0);
                    widget.onFieldChanged = t;
                }
            }
        }

        public static void ROLupdateUIScaleEditControl(this PartModule module, string fieldName, float[] intervals, float[] increments, bool forceUpdate, float forceValue=0)
        {
            if (GetWidget(module, fieldName) is UI_ScaleEdit widget)
            {
                widget.intervals = intervals;
                widget.incrementSlide = increments;
                if (forceUpdate && widget.partActionItem is UIPartActionScaleEdit ctr)
                {
                    var t = widget.onFieldChanged;
                    widget.onFieldChanged = null;
                    ctr.inc.onToggle.RemoveAllListeners();
                    ctr.dec.onToggle.RemoveAllListeners();
                    ctr.slider.onValueChanged.RemoveAllListeners();
                    ctr.Setup(ctr.Window, module.part, module, HighLogic.LoadedSceneIsEditor ? UI_Scene.Editor : UI_Scene.Flight, widget, module.Fields[fieldName]);
                    widget.onFieldChanged = t;
                }
            }
        }

        public static void ROLupdateUIScaleEditControl(this PartModule module, string fieldName, float min, float max, float increment, bool flight, bool editor, bool forceUpdate, float forceValue = 0)
        {
            float seg = (max - min) / increment;
            int numOfIntervals = Mathf.RoundToInt(seg) + 1;
            BaseField field = module.Fields[fieldName];
            if (increment <= 0 || numOfIntervals <= 1)
            {
                field.guiActive = false;
                field.guiActiveEditor = false;
                return;
            }
            float sliderInterval = increment * 0.05f;
            field.guiActive = flight;
            field.guiActiveEditor = editor;
            float[] intervals = new float[numOfIntervals];
            float[] increments = new float[numOfIntervals];
            for (int i = 0; i < numOfIntervals; i++)
            {
                intervals[i] = min + (increment * i);
                increments[i] = sliderInterval;
            }
            module.ROLupdateUIScaleEditControl(fieldName, intervals, increments, forceUpdate, forceValue);
        }

        public static void ROLupdateUIScaleEditControl(this PartModule module, string fieldName, float value)
        {
            if (GetWidget(module, fieldName) is UI_ScaleEdit widget && widget.partActionItem is UIPartActionScaleEdit ctr)
            {
                var t = widget.onFieldChanged;
                widget.onFieldChanged = null;
                ctr.inc.onToggle.RemoveAllListeners();
                ctr.dec.onToggle.RemoveAllListeners();
                ctr.slider.onValueChanged.RemoveAllListeners();
                ctr.Setup(ctr.Window, module.part, module, HighLogic.LoadedSceneIsEditor ? UI_Scene.Editor : UI_Scene.Flight, widget, module.Fields[fieldName]);
                widget.onFieldChanged = t;
            }
        }

        public static void ROLupdateUIFloatRangeControl(this PartModule module, string fieldName, float min, float max, float inc, bool forceUpdate)
        {
            if (GetWidget(module, fieldName) is UI_FloatRange widget)
            {
                widget.minValue = min;
                widget.maxValue = max;
                widget.stepIncrement = inc;
                if (forceUpdate && widget.partActionItem is UIPartActionFloatRange ctr)
                {
                    var t = widget.onFieldChanged;  // temporarily remove the callback
                    widget.onFieldChanged = null;
                    ctr.slider.onValueChanged.RemoveAllListeners();
                    ctr.inputField.onValueChanged.RemoveAllListeners();
                    ctr.Setup(ctr.Window, module.part, module, HighLogic.LoadedSceneIsEditor ? UI_Scene.Editor : UI_Scene.Flight, widget, module.Fields[fieldName]);
                    widget.onFieldChanged = t; // re-seat callback
                }
            }
        }

        /// <summary>
        /// Performs the input delegate onto the input part module and any modules found in symmetry counerparts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module"></param>
        /// <param name="action"></param>
        public static void ROLactionWithSymmetry<T>(this T module, Action<T> action) where T : PartModule
        {
            action(module);
            ROLforEachSymmetryCounterpart(module, action);
        }

        /// <summary>
        /// Performs the input delegate onto any modules found in symmetry counerparts. (does not effect this.module)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module"></param>
        /// <param name="action"></param>
        public static void ROLforEachSymmetryCounterpart<T>(this T module, Action<T> action) where T : PartModule
        {
            int index = module.part.Modules.IndexOf(module);
            foreach (Part p in module.part.symmetryCounterparts)
            {
                action((T)p.Modules[index]);
            }
        }

        #endregion

        #region Generic extension and Utiltiy methods

        /// <summary>
        /// Return true/false if the input array contains at least one element that satsifies the input predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool ROLExists<T>(this T[] array, Func<T,bool> predicate)
        {
            int len = array.Length;
            for (int i = 0; i < len; i++)
            {
                if (predicate(array[i])) { return true; }
            }
            return false;
        }

        public static T ROLFind<T>(this T[] array, Func<T, bool> predicate)
        {
            int len = array.Length;
            for (int i = 0; i < len; i++)
            {
                if (array[i] == null)
                {
                    ROLLog.error("ERROR: Null value in array in Find method, at index: " + i);
                }
                if (predicate(array[i]))
                {
                    return array[i];
                }
            }
            //return default in order to properly handle value types (structs)
            //should return either null for reference types or default value for structs
            return default(T);
        }


        #endregion

        #region FloatCurve extensions

        public static string ROLPrint(this FloatCurve curve)
        {
            string output = "";
            foreach (Keyframe f in curve.Curve.keys)
            {
                output += $"\n{f.time} {f.value} {f.inTangent} {f.outTangent}";
            }
            return output;
        }

        public static string ROLToStringSingleLine(this FloatCurve curve)
        {
            string data = "";
            for (int i = 0; i < curve.Curve.length; i++)
            {
                Keyframe key = curve.Curve.keys[i];
                if (i > 0) data += ":";
                data += $"{key.time},{key.value},{key.inTangent},{key.outTangent}";
            }
            return data;
        }

        public static void ROLloadSingleLine(this FloatCurve curve, string input)
        {
            foreach (string keySplit in input.Split(':'))
            {
                string[] valSplits = keySplit.Split(',');
                float key = float.Parse(valSplits[0]);
                float value = float.Parse(valSplits[1]);
                float inTan = float.Parse(valSplits[2]);
                float outTan = float.Parse(valSplits[3]);
                curve.Add(key, value, inTan, outTan);
            }
        }

        #endregion

    }
}

