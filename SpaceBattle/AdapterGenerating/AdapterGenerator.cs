using Scriban;
using SpaceBattle.Interfaces;
using System.Reflection;

namespace SpaceBattle.AdapterGenerating
{
    public class AdapterGenerator: IBuilder
    {
        private Template template = Template.Parse(text:
        @"
public class {{ class_name }} : {{ int_name }} {
    private System.Collections.Generic.IDictionary<string, object> target;
    public {{ class_name }}(System.Collections.Generic.IDictionary<string, object> target) {
        this.target = target;
    }
    {{for property in properties }}
    public {{ property.type }} {{ property.name }} {
            {{if property.can_read}}
                get => IoC.Resolve<{{property.type}}>(""UObjectGetValue"", target, {{property.name}});
            {{end}}
            {{if property.can_write}}
                set => IoC.Resolve<ICommand>(""UObjectSetValue"", target, {{property.name}}, propertyValue).Execute();
            {{end}}
    }
    {{end}}
        }
        ");
        private IEnumerable<PropertyInfo> propertyInfos;
        private Type dtype;
        private readonly IDictionary<string, Action<object[]>> configurationHandlers = new Dictionary<string, Action<object[]>>{
        {"Dtype",
            args => {
                var ctx = (AdapterGenerator)args[0];
                ctx.dtype = (Type)args[1];
            }
        },
        {"Property",
            args => {
                var ctx = (AdapterGenerator)args[0];
                var propInfo = (PropertyInfo)args[1];
                ctx.propertyInfos = ctx.propertyInfos.Append(propInfo).ToArray();
            }
        }
        };
        public AdapterGenerator()
        {
            this.propertyInfos = new LinkedList<PropertyInfo>();
            this.dtype = null!;
        }
        public IBuilder addMembers(string param, params object[] args)
        {
            Action<object[]> handler = configurationHandlers[param];
            handler(new object[] { this }.Concat(args).ToArray());
            return this;
        }
        public object Build()
        {
            object model = new
            {
                class_name = dtype.Name + "_adapter",
                int_name = dtype.FullName,
                properties = this.propertyInfos.Select(
               (PropertyInfo p) =>
               {
                   object property = new
                   {
                       name = p.Name,
                       type = p.PropertyType.FullName,
                       can_read = p.CanRead,
                       can_write = p.CanWrite
                   };
                   return property;
               }
               ).ToList()
            };
            var result = this.template.Render(model);
            return result.Replace("\r\n", "").Replace("    ", "").Replace("\n", "");
        }
    }
}
