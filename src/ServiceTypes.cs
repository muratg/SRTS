using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SRTS
{
    public class ServiceTypes
    {
        static Dictionary<string, TypeScriptType> hubCache;
        static Dictionary<string, string> hubAliasesCache;
        static Dictionary<string, TypeScriptType> clientCache;

        public static Dictionary<string, TypeScriptType> HubCache { get { return hubCache; } }
        public static Dictionary<string, TypeScriptType> ClientCache { get { return clientCache; } }
        public static Dictionary<string, string> HubAliasesCache { get { return hubAliasesCache; } }

        static ServiceTypes() 
        {
            hubCache = new Dictionary<string, TypeScriptType>();
            clientCache = new Dictionary<string, TypeScriptType>();
            hubAliasesCache = new Dictionary<string, string>();
        }

        static string CamelCase(string s)
        {
            return s[0].ToString().ToLower() + s.Substring(1);
        }

        static TypeScriptType MakeHubInterface(Type hubType) 
        {
            var name = "I" + hubType.Name;
            var cName = name + "Client";

            var declaration = "interface " + name + " {\n";
            var count = 0;
            var sep = "";
            var hubAttribute = "";
            hubType.GetMethods()
                .Where(mi => mi.DeclaringType.Name == hubType.Name).ToList()
                .ForEach(mi =>
                {                    
                    declaration += "    " + CamelCase(mi.Name) + "(";

                    var retTS = DataTypes.GetTypeScriptType(mi.ReturnType);
                    var retType = retTS.Name == "System.Void" ? "void" : "IPromise<" + retTS.Name + ">";
                    mi.GetParameters().ToList()
                    .ForEach((pi) =>
                    {
                        sep = (count != 0 ? ", " : "");
                        count++;
                        var tst = DataTypes.GetTypeScriptType(pi.ParameterType);
                        declaration += sep + pi.Name  + ": " + tst.Name;
                    });

                    declaration += "): "+ retType + ";\n";
                    count = 0;
                });
            
            declaration += "}";

            hubAttribute = hubType.CustomAttributes
                .Where(ad => ad.AttributeType.Name == "HubNameAttribute")
                .Select(ad => ad.ConstructorArguments.FirstOrDefault().Value.ToString())
                .FirstOrDefault();

            var ret = new TypeScriptType
            {
                Name = name,
                Declaration = declaration
            };

            hubCache.Add(name, ret);

            hubAliasesCache.Add(name, hubAttribute);

            clientCache.Add(cName, new TypeScriptType { 
                Name = cName,
                Declaration = "interface " + cName + " {  \n    /* Not implemented */ \n}"
            });

            return ret;
        }

        /*
         declare var client: IClient // To be filled by the user...
         declare var server: IServer
         
         */

        public static void AddHubsFromAssembly(Assembly assembly) 
        {            
            assembly.GetTypes()
	            .Where(t => t.BaseType != null && t.BaseType.Name == "Hub").ToList()
                .ForEach(t => MakeHubInterface(t) );
        }
    }
}
