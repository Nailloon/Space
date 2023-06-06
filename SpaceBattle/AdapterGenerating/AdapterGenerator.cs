using Scriban;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.AdapterGenerating
{
    public class AdapterGenerator: IBuilder
    {
        private Template template = Template.Parse(text:
        @"
namespace SpaceBattle;
public class {{ class_name }} : {{ int_name }} {
    private System.Collections.Generic.IDictionary<string, object> data;
    public {{ class_name }}(System.Collections.Generic.IDictionary<string, object> target) {
        this.target = terget;
    }
    {{- for property in properties }}
    {{ full_property_name = property.type + ""."" + property.name }}
    public {{ property.type }} {{ property.name }} {
        {{
            if property.can_read
                ""get => ("" + property.type + "")data[\"""" + full_property_name + ""\""];""
            end
            if property.can_write
                ""set => IoC.Resolve<ICommand>("" + UObject.setValue + "", object, propertyName, propertyValue).Execute()""
            end
        }}
    }
    {{- end }}
        ");
        private IEnumerable<PropertyInfo> propertyInfos;
        private Type dtype;
        private readonly IDictionary<string, Action<object[]>> configHandlers = new Dictionary<string, Action<object[]>>{
        {"Property",
            args => {
                var ctx = (AdapterGenerator)args[0];
                var propInfo = (PropertyInfo)args[1];
                ctx.propertyInfos = ctx.propertyInfos.Append(propInfo).ToArray();
            }
        },
        {"Dtype",
            args => {
                var ctx = (AdapterGenerator)args[0];
                ctx.dtype = (Type)args[1];
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
            Action<object[]> handler = configHandlers[param];
            handler(new object[] { this }.Concat(args).ToArray());
            return this;
        }
        public object Build()
        {
            return;
        }
    }
}
