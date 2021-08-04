# Create Power Automate Flows Programmatically
A C# console application sample using the [Power Automate Web API](https://docs.microsoft.com/en-us/power-automate/web-api) to programmatically read/write Power Automate flows. 

## Quick How-To
1. Download or clone the repo so that you have a local copy.
1. Open the sample solution in Visual Studio
2. Review [this article](https://techcommunity.microsoft.com/t5/microsoft-365-pnp-blog/create-and-retrieve-power-automate-flows-with-code/ba-p/2600551) for guidance on interacting with API flows
3. Replace the variable on line #17 with your Dataverse environment URL
    ### Retrieve a Flow
    1. Uncomment the ```RetrieveFlow``` method call on line #40
    2. Update the variable on line #113 with flow ID (GUID)
    3. Run the project!
    ### Create a Flow
    1. Uncomment the ```CreateFlow``` method call on line #41
    2. Update the variable on line #61 with relevant clientData content
    3. Run the project!

## Tips & Tricks
6. You will be prompted to enter a valid username and password. 
7. The account you provide must have access to the Dynamics CRM API's. (I have only tested this solution using a tenant global admin account.)
9. If you have not run any CDS for Apps samples before, you will be asked to give consent.
10. The auth functionality is borrowed from [Jim Daly's sample](https://github.com/microsoft/PowerApps-Samples/tree/master/cds/webapi/C%23/ADALV3WhoAmI/ADALV3WhoAmI). It's a great way to get started quickly, but should not be used to access sensitive/production data. 
11. The [GET request](https://docs.microsoft.com/en-us/power-automate/web-api#update-a-cloud-flow) for a single flow seems to fail intermittently. I have reported the behavior [here](https://stackoverflow.com/questions/68627818/power-automate-web-api-get-failing-intermittently).
12. The ```CreateFlow``` method makes two different API calls. The first (POST) actually creates the flow and the second (PATCH) updates the ```statecode``` property to 1. This seems to be necessary and the flow will not function in the UI without such an update. 
13.  I have only been able to programmatically create/access flows within the Default Solution for a relevant [Dataverse environment](https://docs.microsoft.com/en-us/learn/modules/create-manage-environments/). Microsoft does not mention this limitation in the [web API documentation](https://docs.microsoft.com/en-us/power-automate/web-api) and I am uncertain of the scalability/performance implications.
14. When the sample is finished, press any key to exit.

## What this sample does
* The ```CreateFlow``` method creates a Power Automate flow in the Solutions tab and then updates its ```statecode``` property to 1.
* The ```RetrieveFlow``` method retrieves the flow object matching the ID/GUID provided.
