using Microsoft.JSInterop;

using System.Threading.Tasks;

using uaParserLibrary;
using uaParserLibrary.Models;

namespace uaParserBlazorWebAssemblyDemo.Pages
{
    public partial class UAInfo
    {
        private string uaString;
        private string renderer;

        ClientInfo result= new ClientInfo();
        protected override async Task OnInitializedAsync()
        {
            uaString = await JSRuntime.InvokeAsync<string>("getUserAgent");

            renderer = await JSRuntime.InvokeAsync<string>("getRenderer");
        }

        

        private void ParseUA()
        {
            result = UAParser.GetClientInfo(uaString,renderer);
          


        }
    }
}