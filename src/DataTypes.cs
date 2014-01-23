using System;
using System.Collections.Generic;
using System.Linq;

namespace SRTS
{
    public class DataTypes
    {
        static Dictionary<string, TypeScriptType> cache;

        public static Dictionary<string, TypeScriptType> Cache { get { return cache; } }

        public static List<int> TupleTypesByCount { get; set; }
        public static bool HasDictionary { get; set; }

        static DataTypes()
        {
            cache = new Dictionary<string, TypeScriptType>();
            TupleTypesByCount = new List<int>();
            HasDictionary = false;

            // Integral types
            cache.Add("System.Object", new TypeScriptType { Name = "any", Declaration = "" });
            cache.Add("System.Boolean", new TypeScriptType { Name = "boolean", Declaration = "" });

            cache.Add("System.Byte", new TypeScriptType { Name = "number", Declaration = "" });
            cache.Add("System.SByte", new TypeScriptType { Name = "number", Declaration = "" });
            cache.Add("System.Short", new TypeScriptType { Name = "number", Declaration = "" });
            cache.Add("System.UShort", new TypeScriptType { Name = "number", Declaration = "" });
            cache.Add("System.Int32", new TypeScriptType { Name = "number", Declaration = "" });
            cache.Add("System.UInt32", new TypeScriptType { Name = "number", Declaration = "" });
            cache.Add("System.Int64", new TypeScriptType { Name = "number", Declaration = "" });
            cache.Add("System.UInt64", new TypeScriptType { Name = "number", Declaration = "" });
            cache.Add("System.Single", new TypeScriptType { Name = "number", Declaration = "" });
            cache.Add("System.Double", new TypeScriptType { Name = "number", Declaration = "" });
            cache.Add("System.Decimal", new TypeScriptType { Name = "number", Declaration = "" });

            cache.Add("System.String", new TypeScriptType { Name = "string", Declaration = "" });
            cache.Add("System.Char", new TypeScriptType { Name = "string", Declaration = "" });
            cache.Add("System.DateTime", new TypeScriptType { Name = "string", Declaration = "" });
            cache.Add("System.DateTimeOffset", new TypeScriptType { Name = "string", Declaration = "" });
            cache.Add("System.Byte[]", new TypeScriptType { Name = "string", Declaration = "" });
            cache.Add("System.Type", new TypeScriptType { Name = "string", Declaration = "" });
            cache.Add("System.Guid", new TypeScriptType { Name = "string", Declaration = "" });

            cache.Add("System.Exception", new TypeScriptType { Name = "string", Declaration = "" });
            cache.Add("System.Collections.IDictionary", new TypeScriptType { Name = "Dictionary<string, any>", Declaration = "" });

        }

        // Types that have implicit declarations 
        public static TypeScriptType MakeFromNullable(Type type)
        {
            if (type.GenericTypeArguments.Count() == 1)
            {
                var name = GetTypeScriptType(type.GenericTypeArguments[0]).Name;
                return new TypeScriptType
                {
                    Name = name + "?",
                    Declaration = ""
                };
            }
            throw new ArgumentException("Can't make a Nullable type from: " + type);
        }

        public static TypeScriptType MakeFromTask(Type type)
        {
            var name = "void";

            if (type.GenericTypeArguments.Count() == 1)
            {
                name = GetTypeScriptType(type.GenericTypeArguments[0]).Name;
            }
            return new TypeScriptType
            {
                Name = name,
                Declaration = ""
            };
        }

        public static TypeScriptType MakeArray(Type type)
        {
            if (type.GenericTypeArguments.Count() != 1)
            {
                throw new ArgumentException("Can't convert " + type.FullName + " to an Array type. Invalid number of generic type arguments");
            }

            var elementType = type.GenericTypeArguments[0];
            var tst = GetTypeScriptType(elementType);

            return new TypeScriptType
            {
                Name = "Array<" + tst.Name + ">",
                Declaration = ""
            };
        }

        // TODO: create a generic Dictionary type interface..  such as IDictionary<T> (excepts number or string) in []  
        static TypeScriptType MakeDictionary(Type type)
        {
            if (type.GenericTypeArguments.Count() != 2)
            {
                throw new ArgumentException("Can't convert " + type.FullName + " to a Dictionary type. Invalid number of generic type arguments");
            }

            var keyType = type.GenericTypeArguments[0];
            var valueType = type.GenericTypeArguments[1];
            var keyTypeScriptType = GetTypeScriptType(keyType);
            if (keyTypeScriptType.Name != "string" && keyTypeScriptType.Name != "number")
            {
                throw new ArgumentException("Can't convert " + type.FullName
                    + " to a collection type. TypeScript type of the key is "
                    + keyTypeScriptType.Name + ". Should be number or string");
            }

            var valueTypeScriptType = GetTypeScriptType(valueType);
            HasDictionary = true;

            return new TypeScriptType
            {
                Name = "IDictionary<" + valueTypeScriptType.Name + ">",
                Declaration = ""
            };
        }

        // TODO: for each count of tuples, create a corresponding interface with number of Tuples..
        static TypeScriptType MakeTuple(Type type)
        {
            var count = type.GenericTypeArguments.Count();
            var name = "Tuple" + count + "<";

            TupleTypesByCount.Add(count);

            type.GenericTypeArguments.ToList().ForEach(t => name += GetTypeScriptType(t).Name + ", ");

            name = name.Substring(0, name.Length - ", ".Length);
            name += ">";

            return new TypeScriptType
            {
                Name = name,
                Declaration = ""
            };
        }

        // Types that have exlplicit declarations

        static TypeScriptType MakeEnum(Type type)
        {
            var name = type.Name;
            var declaration = "enum " + name + " {";
            var count = 0;

            type.GetEnumNames().ToList().ForEach(n =>
            {
                var sep = (count != 0 ? ", " : "") + "\n    ";
                count++;
                declaration += sep + n;
            });

            declaration += "\n}";

            return new TypeScriptType
            {
                Name = name,
                Declaration = declaration
            };
        }

        static TypeScriptType MakeClassOrInterface(Type type)
        {
            var declaration = "";
            var name = type.Name;

            var members = type.GetMembers().ToList()
                .Select((mi, x) => Tuple.Create(mi.MemberType.ToString(), mi))
                .Where(mt => (mt.Item1 == "Property" || mt.Item1 == "Field"))
                .ToList();

            // TODO: Consider differentiating between classes and interfaces...
            declaration += "interface " + name + " {\n";

            members.ForEach(mt =>
            {
                var mem = mt.Item2;

                var fieldName = mem.Name;
                var typeName = "";

                if (mt.Item1 == "Property")
                {
                    var prop = type.GetProperty(fieldName);
                    typeName = GetTypeScriptType(prop.PropertyType).Name;
                }
                else if (mt.Item1 == "Field")
                {
                    var prop = type.GetField(fieldName);
                    typeName = GetTypeScriptType(prop.FieldType).Name;
                }
                else
                {
                    // Shouldn't happen
                    throw new ArgumentException("Unknown kind of class or interface element: " + fieldName + " is not expected....");
                }

                declaration += "    " + mem.Name + ": " + typeName + ";\n";
            });

            declaration += "}";

            return new TypeScriptType
            {
                Name = name,
                Declaration = declaration
            };
        }

        public static TypeScriptType GetTypeScriptType(Type type)
        {
            TypeScriptType value;
            var typeName = type.FullName;

            if (cache.TryGetValue(typeName, out value))
            {
                return value;
            }


            // Nullables..
            if (type.FullName.Contains("Nullable"))
            {
                value = MakeFromNullable(type);
            }
            // Tasks..
            else if (type.FullName.Contains("Task") || type.GetInterfaces().Any(X => X.FullName.Contains("Task")))
            {
                value = MakeFromTask(type);
            }
            // Dictionaries -- these should come before IEnumerables, because they also implement IEnumerable
            else if (type.GetInterfaces().Any(X => X.FullName.Contains("IDictionary")))
            {
                value = MakeDictionary(type);
            }
            // Arrays
            else if (typeName.Contains("[]"))
            {
                value = MakeArray(typeof(List<Object>));
            }
            else if (type.GetInterfaces().Any(X => X.FullName.Contains("IList") || X.FullName.Contains("IEnumerable")))
            {
                value = MakeArray(type);
            }
            else if (type.GetInterfaces().Any(X => X.FullName.Contains("Tuple")))
            {
                value = MakeTuple(type);
            }
            else if (type.IsEnum)
            {
                value = MakeEnum(type);
            }
            else if (type.IsClass || type.IsInterface)
            {
                value = MakeClassOrInterface(type);
            }
            else if (type.FullName == "System.Void")
            {
                value = new TypeScriptType
                {
                    Name = "void",
                    Declaration = ""
                };
            }
            else
            {
                Console.WriteLine("Warning:" + type);
                value = new TypeScriptType
                {
                    Name = "UNKNOWN TYPE " + type,
                    Declaration = ""
                };
            }


            cache.Add(type.FullName, value);

            return value;
        }
    }
}
