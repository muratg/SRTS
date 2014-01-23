using System.Linq;
using System.Reflection;

namespace SRTS
{    
    public class SignalRModule
    {
        public static string Create(Assembly assembly)
        {
            ServiceTypes.AddHubsFromAssembly(assembly);

            var dec = "";

            dec += "/*\n   Client interfaces:\n      These are to be implemented by the user.\n      These are for Hub -> Client calls.\n */ \n\n";
            foreach (var k in ServiceTypes.ClientCache.Keys)
            {
                var dt = ServiceTypes.ClientCache[k];
                if (dt.Declaration != "")
                {
                    dec += dt.Declaration + "\n";
                }
            }

            dec += "\n//Promise interface\n";
            dec += "interface IPromise<T> {\n";
            dec += "    done(cb: (result: T) => any): IPromise<T>;\n";
            dec += "    error(cb: (error: any) => any): IPromise<T>;\n";
            dec += "}\n";
            if (DataTypes.HasDictionary)
            {
                dec += "\n//Generic dictionary interface\n";
                dec += "interface IDictionary<T> {\n    [key: any]: T;\n}\n";
            }
            if (DataTypes.TupleTypesByCount.Count() > 0)
            {
                dec += "\n// Tuple types\n";
                DataTypes.TupleTypesByCount.ForEach(c =>
                {
                    dec += "interface Tuple" + c + "<";
                    for (var i = 0; i < c; i++)
                    {
                        dec += "T" + i + (i == c - 1 ? "" : ", ");
                    }
                    dec += "> {\n";
                    for (var i = 0; i < c; i++)
                    {
                        dec += "    Item" + i + ": T" + i + ";\n";
                    }
                    dec += "}\n";
                });
            }

            dec += "\n// Data interfaces \n";
            foreach (var k in DataTypes.Cache.Keys)
            {
                var dt = DataTypes.Cache[k];
                if (dt.Declaration != "")
                {
                    dec += dt.Declaration + "\n";
                }
            }

            dec += "\n// Hub interfaces \n";
            foreach (var k in ServiceTypes.HubCache.Keys)
            {
                var dt = ServiceTypes.HubCache[k];
                if (dt.Declaration != "")
                {
                    dec += dt.Declaration + "\n";
                }
            }

            dec += "\n// Generetated proxies \n";

            foreach (var k in ServiceTypes.HubCache.Keys)
            {
                var dt = ServiceTypes.HubCache[k];
                var hubName = ServiceTypes.HubAliasesCache[k];
                var hubAttribute = ServiceTypes.HubAliasesCache[k];

                dec += "interface " + dt.Name + "Proxy {\n";
                dec += "     server: " + dt.Name + ";\n";
                dec += "     client: " + dt.Name + "Client;\n";
                dec += "}\n";

            }
            return dec;
        }
    }
}
