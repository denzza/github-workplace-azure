using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;

namespace GithubWorkplaceAzure
{
    public static class GithubWebhookWorkplaceFunction
    {
        [FunctionName("GithubWebhookWorkplaceFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Extract github data from request
            string repoName = data?.repository?.name;
            string repoDescription = data?.repository?.description;
            string issueDescription = data?.issue?.body;
            string gitSender = data?.sender?.login;
            string gitHubIssue = data?.issue?.title;
            string issueURL = data?.issue?.html_url;


            //Creating a new issue post content for Workplace
            string message = "New issue in repo: " + repoName + "\n" +
                             "Repo description: " + repoDescription + "\n" +
                             "\n" +
                             "Issue: " + gitHubIssue + "\n" +
                             "Comment: " + issueDescription + "\n" + "\n" +
                             "\n" +
                             "Sender: " + gitSender + "\n" +
                             "Issue url: " + issueURL;

            using (var httpClient = new HttpClient())
            {

                httpClient.BaseAddress = new Uri("https://graph.facebook.com/");

                var parametters = new Dictionary<string, string>
                {
                    { "access_token", "DQVJ0WTZAkeWcza0h4eVlTeFRPZAkVMaEVyajAtbDlXS3dBZAnM0QzhnUTJkYXh6ck5oc3dwLWYxNXBwVlVRQ1BYVDdpSVRfNy1IX0hBbUduNGtMREw5WDlMY0tYU016ZAzA0WjlHUFFHLTY3Qy0tTXViMURNb09KMTZAXcFhRM1BrY3E3eVBfNXRrLUxCYXlxcTJIT3FvV3dUV2RRYzRFa0NJMDR0QjcxUUlUQTRHZAFhlUVBpRUF6TnZAtQlp0YXJ4LWpGQl9GX3B3" },
                    { "message", message }
                };                

                var encodedContent = new FormUrlEncodedContent(parametters);
                string groupId = "318299162411179";

                var result = await httpClient.PostAsync($"{groupId}/feed", encodedContent);
                var responseStr = await result.Content.ReadAsStringAsync();                
                var msg = result.EnsureSuccessStatusCode();
                return req.CreateResponse(HttpStatusCode.OK, "Response Workplace: " + msg);
            }
            
        }
    }
}
