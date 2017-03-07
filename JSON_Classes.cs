using System.Collections.Generic;

namespace JSON_Parser
{
    public class Value { }

    public class Null : Value { }

    public class Bool : Value
    {
        public bool val { get; set; }

        public Bool(bool b)
        {
            val = b;
        }
    }

    public class Float : Value
    {
        public float val { get; set; }

        public Float(float f)
        {
            val = f;
        }
    }

    public class Str : Value
    {
        public string val { get; set; }

        public Str(string s)
        {
            val = s;
        }
    }

    public class Array : Value
    {
        public List<Value> arrVals { get; set; }

        public Array(List<Value> arr)
        {
            arrVals = arr;
        }
    }

    public class Object : Value
    {
        public Dictionary<string, List<Value>> objVals { get; set; }

        public Object(Dictionary<string, List<Value>> dict)
        {
            objVals = dict;
        }
    }
}
