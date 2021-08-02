using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    class SampleProgram
    {
        static void Main(string[] args)
        {
            // The URL to the Dataverse environment you want to connect with
            // i.e. https://yourOrg.crm.dynamics.com
            string resource = "[ENVIRONMENT URL]";

            // Azure Active Directory registered app clientid for Microsoft samples
            var clientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
            // Azure Active Directory registered app Redirect URI for Microsoft samples
            var redirectUri = new Uri("app://58145B91-0C36-4500-8554-080854F2AC97");
            string authority = "https://login.microsoftonline.com/common";

            var context = new AuthenticationContext(authority, false);

            //Display prompt allowing user to select account to use.
            var platformParameters = new PlatformParameters(
                PromptBehavior.SelectAccount
            );

            //Get the token based on the credentials entered
            var token = context.AcquireTokenAsync(resource,
                clientId,
                redirectUri,
                platformParameters,
                UserIdentifier.AnyUser)
            .Result;

            //RetrieveFlow(resource, token.AccessToken);
            //CreateFlow(resource, token.AccessToken);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static void CreateFlow(string resource, string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(resource + "/api/data/v9.1/");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
                client.Timeout = new TimeSpan(0, 2, 0);
                client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                //Replace with clientData from preferred Power Automate flow.
                string clientData = "[CLIENT DATA]";
                
                WorkFlow workflow = new WorkFlow
                {
                    category = 5,
                    statecode = 1,
                    name = "MyFlow",
                    type = 1,
                    description = "Sample flow to test programmatic creation.",
                    primaryentity = "none",
                    clientdata = clientData
                };

                try
                {
                    //Create Workflow
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, resource + "/api/data/v9.1/workflows");
                    request.Content = new StringContent(JsonConvert.SerializeObject(workflow), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.SendAsync(request).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        //NOTE:Update New Workflow - This is necessary before the flow is functional/editable in the UI. 
                        request = new HttpRequestMessage(new HttpMethod("PATCH"), response.Headers.Location.ToString());
                        request.Content = new StringContent("{\"statecode\":1}", Encoding.UTF8, "application/json");
                        response = client.SendAsync(request).Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Flow update failed: {0}", response.ReasonPhrase);
                        }
                    }
                    else
                        Console.WriteLine("Flow creation failed: {0}", response.ReasonPhrase);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                    return;
                }
            }
        }

        static async Task<object> RetrieveFlow(string resource, string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
                client.Timeout = new TimeSpan(0, 2, 0);
                client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                string flowID = "[FLOW GUID]";

                try
                {
                    //Get Workflow - Sometimes this fails inexplicably, reported bug here: https://stackoverflow.com/questions/68627818/power-automate-web-api-get-failing-intermittently
                    HttpResponseMessage response = client.GetAsync(resource + "/api/data/v9.1/workflows(" + flowID + ")").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject(jsonString);
                    }
                    else
                    {
                        Console.WriteLine("Flow retrieval failed: {0}", response.ReasonPhrase);
                        return null;
                    }
                }
                catch (Exception ex)
                {   
                    Console.WriteLine("Error: {0}", ex.Message);
                    return null;
                }
            }
        }
    }


    public class WorkFlow
    {
        public int category { get; set; }
        public int statecode { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public string description { get; set; }
        public string primaryentity { get; set; }
        public string clientdata { get; set; }
    }
}
