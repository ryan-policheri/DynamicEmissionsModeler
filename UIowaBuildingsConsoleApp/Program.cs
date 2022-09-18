using DotNetCommon.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PiServices;
using UIowaBuildingsConsoleApp;
using UIowaBuildingsConsoleApp.Startup;
using EmissionsMonitorModel;
using UIowaBuildingsModelConsoleApp;
using UIowaBuildingsServices;
using UnitsNet;

Config config = Bootstrapper.LoadConfiguration();
IServiceProvider serviceProvider = Bootstrapper.BuildServiceProvider(config);

ReportingService reportingService =  serviceProvider.GetService<ReportingService>();

CampusSnapshot postDaylightChange = await reportingService.GenerateCampusSnapshot(new HourlyEmissionsReportParameters
{
    AssetLinks = { "https://pi.facilities.uiowa.edu/piwebapi/elements/F1EmAVYciAZHVU6DzQbJjxTxWwcE7mI49J6RGuHFS_ZKR9xgSVRTTlQyMjU5XFVJLUVORVJHWVxNQUlOIENBTVBVU1xNQUNMRUFOIEhBTEw" },
    StartDateInLocalTime = new DateTime(2022, 3, 14, 0, 0, 0, DateTimeKind.Local),
    EndDateInLocalTime = new DateTime(2022, 5, 5, 0, 0, 0, DateTimeKind.Local),
    GridStrategy = ElectricGridStrategy.MidAmericanAverageFuelMix
});

CampusSnapshot preDaylightChange = await reportingService.GenerateCampusSnapshot(new HourlyEmissionsReportParameters
{
    AssetLinks = { "https://pi.facilities.uiowa.edu/piwebapi/elements/F1EmAVYciAZHVU6DzQbJjxTxWwcE7mI49J6RGuHFS_ZKR9xgSVRTTlQyMjU5XFVJLUVORVJHWVxNQUlOIENBTVBVU1xNQUNMRUFOIEhBTEw" },
    StartDateInLocalTime = new DateTime(2021, 12, 16, 0, 0, 0, DateTimeKind.Local),
    EndDateInLocalTime = new DateTime(2022, 3, 12, 0, 0, 0, DateTimeKind.Local),
    GridStrategy = ElectricGridStrategy.MidAmericanAverageFuelMix
});

List<HourDetails> details = postDaylightChange.EnergyResources.ToList();
details.AddRange(preDaylightChange.EnergyResources.ToList());

//BuildingUsage usage = new BuildingUsage();
//usage.ElectricUsage = Energy.FromKilowattHours(100);
//usage.SteamUsageAsEnergy = Energy.FromMegabritishThermalUnits(1);
//usage.ChilledWaterUsage = Volume.FromUsGallons(500);

List<BuildingUsage> buildingUsages = preDaylightChange.BuildingUsageSummaries.First().BuildingUsages.ToList();
buildingUsages.AddRange(postDaylightChange.BuildingUsageSummaries.First().BuildingUsages.ToList());

List<UsageTime> usageTimes = new List<UsageTime>();


foreach(HourDetails detail in details)
{
    var matchingUsage = buildingUsages.First(x => x.Timestamp.HourMatches(detail.Hour));
    detail.PopulateCo2Emissions(matchingUsage);
    usageTimes.Add(new UsageTime
    {
        Co2Mass = matchingUsage.TotalCo2,
        Time = detail.Hour
    });    
}

foreach(var item in usageTimes.OrderBy(x => x.Time))
{
    Console.WriteLine(item.Time.ToLocalTime().ToString() + ", " + item.Co2Mass.Kilograms);
}