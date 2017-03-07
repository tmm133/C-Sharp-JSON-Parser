using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JSON_Parser
{
    class JSON_Parser
    {
        static List<Null> nulls = new List<Null>();
        static List<Bool> bools = new List<Bool>();
        static List<Float> nums = new List<Float>();
        static List<Str> strings = new List<Str>();
        static List<Array> arrays = new List<Array>();
        static List<Object> objects = new List<Object>();
        static int objectCount = 0;
        static string jsonString;
        static int stringIndex;

        static void Main(string[] args)
        {
            try
            {
                using (StreamReader sr = new StreamReader(args[0].ToString()))
                {
                    jsonString = sr.ReadToEnd();
                }
                jsonString = jsonString.Replace(System.Environment.NewLine, string.Empty);

                parseJSON(jsonString, 1, string.Empty, false);
                prettyPrint();
                printJSONInformation();

                Console.ReadKey();
            }
            catch
            {
                Console.Write("Please make sure to type in full path of file.");
                Console.ReadKey();
            }
        }

        static Value parseJSON(string s, int startIndex, string objectName, bool forArray)
        {
            stringIndex = startIndex;
            int objCount = objects.Count();
            bool isKey = true;
            string tempKey = string.Empty;
            Dictionary<string, List<Value>> dictTemp = new Dictionary<string, List<Value>>();
            List<Value> lstVal = new List<Value>();
            Str tempString;

            while (stringIndex < s.Length - 1)
            {
                dictTemp.Clear();
                lstVal.Clear();

                switch (s[stringIndex])
                {
                    case ':':
                        isKey = false;
                        break;
                    case ',':
                    case ']':
                        isKey = true;
                        if (forArray)
                            return null;
                        break;
                    case '"': //Creates Objects and handles strings
                        stringIndex++;
                        if (forArray)
                        {
                            tempString = new Str(parse_String(ref stringIndex));
                            strings.Add(tempString);
                            return new Str(tempString.val);
                        }
                        if (isKey == false) //The string is a value
                        {
                            tempString = new Str(parse_String(ref stringIndex));
                            lstVal.Add(tempString);
                            dictTemp.Add(tempKey, lstVal);
                            objects.Add(new Object(new Dictionary<string, List<Value>>(dictTemp)));
                            strings.Add(tempString);
                        }
                        else //It is a key
                        {
                            tempKey = parse_String(ref stringIndex);
                        }
                        break;
                    case 'n': //NULL
                    case 'N':
                        lstVal.Add(new Null());
                        dictTemp.Add(tempKey, lstVal);
                        if (!forArray)
                            objects.Add(new Object(new Dictionary<string, List<Value>>(dictTemp)));
                        nulls.Add(new Null());
                        stringIndex += 3;
                        if (forArray)
                            return new Null();
                        break;
                    case 't': //True
                    case 'T':
                        lstVal.Add(new Bool(true));
                        dictTemp.Add(tempKey, lstVal);
                        if (!forArray)
                            objects.Add(new Object(new Dictionary<string, List<Value>>(dictTemp)));
                        bools.Add(new Bool(true));
                        stringIndex += 3;
                        if (forArray)
                            return new Bool(true);
                        break;
                    case 'f': //False
                    case 'F':
                        lstVal.Add(new Bool(false));
                        dictTemp.Add(tempKey, lstVal);
                        if (!forArray)
                            objects.Add(new Object(new Dictionary<string, List<Value>>(dictTemp)));
                        bools.Add(new Bool(false));
                        stringIndex += 4;
                        if (forArray)
                            return new Bool(false);
                        break;
                    case '[':
                        List<Value> arrayVals = parse_Array(ref stringIndex);
                        lstVal.Add(new Array(arrayVals));
                        dictTemp.Add("arr" + stringIndex.ToString(), lstVal);
                        objects.Add(new Object(new Dictionary<string, List<Value>>(dictTemp)));
                        arrays.Add(new Array(arrayVals));
                        break;
                    case '{':
                        objectCount++;
                        isKey = true;
                        parseJSON(s, stringIndex += 1, tempKey, false);
                        break;
                    case '}':
                        int currentObjCount = objects.Count() - 1;
                        List<Value> vals = new List<Value>();
                        while (objCount < currentObjCount)
                        {
                            vals.Add(objects[objCount]);
                            objCount++;
                        }
                        dictTemp.Add(objectName, vals);
                        objects.Add(new Object(new Dictionary<string, List<Value>>(dictTemp)));
                        if (forArray)
                            return new Object(new Dictionary<string, List<Value>>(dictTemp));
                        return null;
                    default: //This is where numbers will be checked
                        int o;
                        if (s[stringIndex] == '-' || int.TryParse(s[stringIndex].ToString(), out o))
                        {
                            float tempFloat = parse_Float(ref stringIndex);
                            lstVal.Add(new Float(tempFloat));
                            dictTemp.Add(tempKey, lstVal);
                            if (!forArray)
                                objects.Add(new Object(new Dictionary<string, List<Value>>(dictTemp)));
                            nums.Add(new Float(tempFloat));
                            if (forArray)
                                return new Float(tempFloat);
                        }
                        break;
                }
                stringIndex++;
            }

            return null;
        }

        static string parse_String(ref int stringIndex)
        {
            StringBuilder sb = new StringBuilder();

            while (jsonString[stringIndex] != '"')
            {
                if (jsonString[stringIndex] == '\\')
                    stringIndex += 2;
                sb.Append(jsonString[stringIndex]);
                stringIndex++;
            }

            return sb.ToString();
        }

        static float parse_Float(ref int stringIndex)
        {
            StringBuilder sbFloat = new StringBuilder();
            while (jsonString[stringIndex] != ',' && jsonString[stringIndex] != ']' && jsonString[stringIndex] != '}' && jsonString[stringIndex] != ' ')
            {
                sbFloat.Append(jsonString[stringIndex]);
                stringIndex++;
            }

            stringIndex--; //Goes back one, main loop will increment to a ','
            return float.Parse(sbFloat.ToString());
        }

        static List<Value> parse_Array(ref int stringIndex)
        {
            List<Value> arrayValues = new List<Value>();
            Value v;
            while (jsonString[stringIndex] != ']')
            {
                if (jsonString[stringIndex] == ']')
                    break;
                stringIndex++;
                if (jsonString[stringIndex] == ']')
                    break;
                v = parseJSON(jsonString, stringIndex, string.Empty, true);
                if (v != null)
                    arrayValues.Add(v);
            }

            return arrayValues;
        }

        static void printJSONInformation()
        {
            Console.Write("\n\n" + new string('-', 100));
            Console.Write("\nTotal Weight: " + (nulls.Count() + bools.Count() + nums.Count() + strings.Count() + arrays.Count() + objectCount + 1).ToString() + "\n");
            Console.Write("Nulls: " + nulls.Count().ToString() + "\n");
            Console.Write("Bools: " + bools.Count().ToString() + "\n");
            Console.Write("Numbers: " + nums.Count().ToString() + "\n");
            Console.Write("Strings: " + strings.Count().ToString() + "\n");
            Console.Write("Arrays: " + arrays.Count().ToString() + "\n");
            Console.Write("Objects: " + (objectCount + 1).ToString() + "\n"); //+1 for the outermost {}
        }

        static void prettyPrint()
        {
            StringBuilder sbFormattedJSON = new StringBuilder();
            int indent = 0;
            bool inArray = false;

            foreach (char ch in jsonString)
            {
                switch (ch)
                {
                    case ',':
                        if (inArray)
                        {
                            sbFormattedJSON.Append(ch);
                        }
                        else
                        {
                            sbFormattedJSON.Append(ch + "\n");
                            sbFormattedJSON.Append(new string(' ', indent * 4));
                        }
                        break;
                    case '[':
                        inArray = true;
                        sbFormattedJSON.Append(ch);
                        break;
                    case ']':
                        inArray = false;
                        sbFormattedJSON.Append(ch);
                        break;
                    case '{':
                        sbFormattedJSON.Append("\n" + new string(' ', indent * 4) + ch + "\n");
                        sbFormattedJSON.Append(new string(' ', indent * 4));
                        indent++;
                        break;
                    case '}':
                        indent--;
                        sbFormattedJSON.Append(new string(' ', indent * 4));
                        sbFormattedJSON.Append("\n" + new string(' ', indent * 4) + ch);
                        break;
                    default:
                        sbFormattedJSON.Append(ch);
                        break;
                }
            }

            Console.Write(sbFormattedJSON.ToString());
        }
    }
}
