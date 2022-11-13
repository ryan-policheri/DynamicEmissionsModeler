using DotNetCommon;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using EmissionsMonitorDataAccess;
using FluentAssertions;
using UnitsNet;

namespace Tests.EmissionsMonitorServices
{
    [TestClass]
    public class ModelInitializationServiceTests
    {
        [TestMethod]
        public async Task WhenProvidedModelWithOneFunction_FunctionExecutesCorrectly()
        {
            ExchangeNode node = new ExchangeNode
            {
                Name = "Boiler 1",
                Costs = new List<DataFunction>(),
                Product = new SteamEnergyFunction()
                {
                    FunctionName = "Steam Energy Output",
                    FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Steam Output", FactorUri = new DataSourceSeriesUri { Uri = "B1 Steam Output Tag" }  }
                    },
                    FunctionCode = $"Energy steamEnergy = Energy.FromMegabritishThermalUnits(SteamOutputPoint.Value);{Environment.NewLine}return steamEnergy;",
                    FunctionHostObject = null
                }
            };
            ProcessModel model = new ProcessModel();
            model.ModelName = "TestModel";
            model.AddProcessNode(node);

            ModelInitializationService modelInitService = new ModelInitializationService(new DynamicCompilerService());
            await modelInitService.InitializeModel(model);
            ExchangeNode initializedNode = (ExchangeNode)model.ProcessNodes.First();

            dynamic hostObj = initializedNode.Product.FunctionHostObject;
            DataPoint dataPoint = new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "B1 Steam Output Tag" } },
                Value = 20 //20 MMBTU
            };

            Energy output = hostObj.SteamEnergyOutput(dataPoint);
            output.MegabritishThermalUnits.Should().Be(20);

            DataFunctionResult output2 = initializedNode.Product.ExecuteFunction(new List<DataPoint>() { dataPoint });
            output2.TotalValue.Should().Be(20);
        }


        [TestMethod]
        public async Task WhenProvidedModelWithUninitializedExchangeNode_CorrectlyCalculatesNodeResults()
        {
            //SAME TEST AS Tests.EmissionsMonitorModel --> ExchangeNodeTests --> WhenSettingUpBasicExchangeNode_ShouldCorrectlyCalculateProductCosts
            //Difference is this starts with uncompiled code.

            //UNINITIALIZED MODEL
            ExchangeNode node = new ExchangeNode
            {
                Name = "Boiler 1",
                Costs = new List<DataFunction>()
                {
                    new MoneyFunction()
                    {
                        FunctionName = "Fuel Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = new DataSourceSeriesUri { Uri = "B1 NG Flow Tag" } }
                        },
                        FunctionCode = $"Volume gasVolume = Volume.FromCubicFeet(NaturalGasUsagePoint.Value);{Environment.NewLine}decimal cost = (decimal)(gasVolume.KilocubicFeet * 10.00); //$10 per 1,000 cubic feet{Environment.NewLine}return Money.FromUsDollars(cost);",
                        FunctionHostObject = null
                    },
                    new Co2MassFunction()
                    {
                        FunctionName = "CO2 Emissions Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = new DataSourceSeriesUri { Uri = "B1 NG Flow Tag" } }
                        },
                        FunctionCode = $"Volume gasVolume = Volume.FromCubicFeet(NaturalGasUsagePoint.Value);{Environment.NewLine}Mass co2Emissions = NaturalGas.ToCo2Emissions(gasVolume);{Environment.NewLine}return co2Emissions;",
                        FunctionHostObject = null
                    }
                },
                Product = new SteamEnergyFunction()
                {
                    FunctionName = "Steam Energy Output",
                    FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Steam Output", FactorUri = new DataSourceSeriesUri { Uri = "B1 Steam Output Tag" } }
                    },
                    FunctionCode = $"Energy steamEnergy = Energy.FromMegabritishThermalUnits(SteamOutputPoint.Value);{Environment.NewLine}return steamEnergy;",
                    FunctionHostObject = null
                }
            };


            ProcessModel model = new ProcessModel();
            model.ModelName = "TestModel";
            model.AddProcessNode(node);

            //DATA
            ICollection<DataPoint> dataPoints = new List<DataPoint>();
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "B1 NG Flow Tag" } },
                Value = 5000 //$50 dollars of natural gas, 275.5 kgs of CO2
            });
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "B1 Steam Output Tag" } },
                Value = 20 //20 MMBTU
            });

            ModelInitializationService modelInitService = new ModelInitializationService(new DynamicCompilerService());
            await modelInitService.InitializeModel(model);

            //TOTAL COST: $50
            //TOTAL CO2: 275.5 KG
            //COST OF 2 MMBTU: 0.1 * $50 = $5
            //CO2 OF 2 MMBTU: 0.1 * 275.5 = 27.55

            ProductCostResults results = node.RenderProductAndCosts(dataPoints);
            ICollection<Cost> costs = results.CalculateCostOfAbstractProductAmount(Energy.FromMegabritishThermalUnits(2));
            double money = costs.First(x => x.Name == "Money Currency").Value;
            money.Should().Be(5);
            double co2 = costs.First(x => x.Name == "CO2 Mass").Value;
            co2.Should().Be(27.55);
        }

        [TestMethod]
        public async Task WhenProvidedModelWithUninitializedModelWithMultipleNodes_CorrectlyCalculatesResults()
        {
            //SAME TEST AS Tests.EmissionsMonitorModel --> LikeTermsAggregatorNodeTests --> WhenAggregatingTwoExchangeNodes_ProportionallyCombinesLikeTerms
            //Difference is this starts with uncompiled code.

            //UNINITIALIZED MODEL
            ExchangeNode node1 = new ExchangeNode
            {
                Name = "Boiler 1",
                Costs = new List<DataFunction>()
                {
                    new MoneyFunction()
                    {
                        FunctionName = "Fuel Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = new DataSourceSeriesUri { Uri = "B1 NG Flow Tag" } }
                        },
                        FunctionCode = $"Volume gasVolume = Volume.FromCubicFeet(NaturalGasUsagePoint.Value);{Environment.NewLine}decimal cost = (decimal)(gasVolume.KilocubicFeet * 10.00); //$10 per 1,000 cubic feet{Environment.NewLine}return Money.FromUsDollars(cost);",
                        FunctionHostObject = null
                    },
                    new Co2MassFunction()
                    {
                        FunctionName = "CO2 Emissions Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = new DataSourceSeriesUri { Uri = "B1 NG Flow Tag" } }
                        },
                        FunctionCode = $"Volume gasVolume = Volume.FromCubicFeet(NaturalGasUsagePoint.Value);{Environment.NewLine}Mass co2Emissions = NaturalGas.ToCo2Emissions(gasVolume);{Environment.NewLine}return co2Emissions;",
                        FunctionHostObject = null
                    }
                },
                Product = new SteamEnergyFunction()
                {
                    FunctionName = "Steam Energy Output",
                    FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Steam Output", FactorUri =  new DataSourceSeriesUri { Uri = "B1 Steam Output Tag" }  }
                    },
                    FunctionCode = $"Energy steamEnergy = Energy.FromMegabritishThermalUnits(SteamOutputPoint.Value);{Environment.NewLine}return steamEnergy;",
                    FunctionHostObject = null
                }
            };

            ExchangeNode node2 = new ExchangeNode
            {
                Name = "Boiler 2",
                Costs = new List<DataFunction>()
                {
                    new MoneyFunction()
                    {
                        FunctionName = "Fuel Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Coal Usage", FactorUri = new DataSourceSeriesUri { Uri = "B2 Coal Flow Tag" } },
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = new DataSourceSeriesUri { Uri = "B2 NG Flow Tag" }  }
                        },
                        FunctionCode = $"Mass coalMass = Mass.FromPounds(CoalUsagePoint.Value);{Environment.NewLine}Volume gasVolume = Volume.FromCubicFeet(NaturalGasUsagePoint.Value);{Environment.NewLine}decimal coalCost = (decimal)(coalMass.Kilograms * 0.34); //34 cents per kg{Environment.NewLine}decimal gasCost = (decimal)(gasVolume.KilocubicFeet * 10.00); //$10 per 1,000 cubic feet{Environment.NewLine}return Money.FromUsDollars(coalCost + gasCost);",
                        FunctionHostObject = null
                    },
                    new Co2MassFunction()
                    {
                        FunctionName = "CO2 Emissions Cost",
                        FunctionFactors = new List<FunctionFactor>()
                        {
                            new FunctionFactor() { FactorName = "Coal Usage", FactorUri = new DataSourceSeriesUri { Uri = "B2 Coal Flow Tag" } },
                            new FunctionFactor() { FactorName = "Natural Gas Usage", FactorUri = new DataSourceSeriesUri { Uri = "B2 NG Flow Tag" } }
                        },
                        FunctionCode = $"Mass coalMass = Mass.FromPounds(CoalUsagePoint.Value);{Environment.NewLine}Volume gasVolume = Volume.FromCubicFeet(NaturalGasUsagePoint.Value);{Environment.NewLine}Mass coalCo2Emissions = Coal.ToCo2Emissions(coalMass);{Environment.NewLine}Mass naturalGasCo2Emissions = NaturalGas.ToCo2Emissions(gasVolume);{Environment.NewLine}return coalCo2Emissions + naturalGasCo2Emissions;",
                        FunctionHostObject = null
                    }
                },
                Product = new SteamEnergyFunction()
                {
                    FunctionName = "Steam Energy Output",
                    FunctionFactors = new List<FunctionFactor>()
                    {
                        new FunctionFactor() { FactorName = "Steam Output", FactorUri = new DataSourceSeriesUri { Uri = "B2 Steam Output Tag" } }
                    },
                    FunctionCode = $"Energy steamEnergy = Energy.FromMegabritishThermalUnits(SteamOutputPoint.Value);{Environment.NewLine}return steamEnergy;",
                    FunctionHostObject = null
                }
            };

            LikeTermsAggregatorNode node3 = new LikeTermsAggregatorNode();
            node3.Name = "Boiler 1, 2 Aggregate";
            node3.PrecedingNodes = new List<ExchangeNode>()
            {
                node1,
                node2
            };

            ProcessModel model = new ProcessModel();
            model.ModelName = "TestModel";
            model.AddProcessNode(node1);
            model.AddProcessNode(node2);
            model.AddProcessNode(node3);

            ModelInitializationService modelInitService = new ModelInitializationService(new DynamicCompilerService());
            await modelInitService.InitializeModel(model);

            //DATA
            ICollection<DataPoint> dataPoints = new List<DataPoint>();
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "B1 NG Flow Tag" } },
                Value = 5000 //$50 dollars of natural gas, 275.5 kgs of CO2
            });
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "B1 Steam Output Tag" } },
                Value = 20 //20 MMBTU
            });
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "B2 Coal Flow Tag" } },
                Value = 500 //$77.11064 of coal, 1035 pounds of CO2
            });
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "B2 NG Flow Tag" } },
                Value = 2500 //$25 of NG, 137.75 Kgs of CO2
            });
            dataPoints.Add(new DataPoint()
            {
                Series = new Series { SeriesUri = new DataSourceSeriesUri { Uri = "B2 Steam Output Tag" } },
                Value = 25 //25 MMBTU
            });

            ProductCostResults results = node3.RenderProductAndCosts(dataPoints);
            ICollection<Cost> costs = results.CalculateCostOfAbstractProductAmount(Energy.FromMegabritishThermalUnits(5));

            //BOILER 1 TOTAL COST: $50
            //BOILER 1 TOTAL CO2: 275.5 KG
            //BOILER 2 TOTAL COST: $77.11064 + $25 = $102.11064 
            //BOILER 2 TOTAL CO2: 1035 pounds + 137.75 Kgs = 469.4681 Kgs + 137.75 Kgs = 607.2181 Kgs

            //COMBINED TOTAL COST = $50 + $102.11064 = $152.11064
            //COMBINED TOTAL CO2 = 275.5 Kgs + 607.2181 Kgs = 882.7181 kgs
            //COMBINED TOTAL OUTPUT = 45 MMBTU

            //PARTIAL COST = 0.111111 * $152.11064 = $16.9011653
            //PARTIAL CO2 = 0.111111 * 882.7181 kgs = 98.0796908091

            double money = costs.First(x => x.Name == "Money Currency").Value;
            Math.Round(money, 2).Should().Be(16.90);
            double co2 = costs.First(x => x.Name == "CO2 Mass").Value;
            Math.Round(co2, 2).Should().Be(98.08);
        }
    }
}