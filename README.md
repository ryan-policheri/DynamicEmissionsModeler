# General
This repo contains the software components for an energy system modeling tool. The tool was initially built with the specific intention to generate real-time carbon footprints of University of Iowa buildings but over the course of my master's research the software became more robust and capable of modeling (in theory) any energy system.

At its core, the tool offers a low-code environment where a user builds a graph of nodes representing how **products** and **costs** flow through an energy system. The tool integrates with the [PI digital twin software](https://www.aveva.com/en/products/aveva-pi-system/) by AVEVA (originally by OSIsoft) and the [EIA.gov api](https://www.eia.gov/opendata/), with extensibility to add other data sources. Using these integrations the tool provides an interface to find data streams within your energy system, place them in nodes, and write short functions to extract products and costs from your data.

# Research
We used this tool to model the University of Iowa energy system. In particular we tracked the generation of three products; electric, steam, and chilled water; and some of their associated costs, fuel cost (dollars) and carbon emissions. We then used the model to calculate the building carbon footprints mentioned above as well as to run experiments on our energy system. Details of this research are located in the repo paper [e-energy_publication_initial_submission.docx](https://github.com/ryan-policheri/DynamicEmissionsModeler/blob/main/e-energy_publication_initial_submission.docx) and there is a corresponding presentation [here on YouTube](https://www.youtube.com/watch?v=Tmr88_AMA9E).

Code related to our specific research at the University of Iowa is still embedded within this repo. Obviously code of this nature is not reusable as it assumes access and knowledge of the UIowa energy system. In the future we plan to decouple this kind of code from the modeling tool.

# System Components
The tool is composed of multiple software components described below:
- **Unified Data Explorer:** This is an interactive Windows desktop application for exploring data within your energy system and building your product-cost model. It is built on Windows Presentation Foundation and thus only works on Windows.
- **Emissions Monitor Web API:** This is a web API that can be used for programmatically managing and executing energy system models. Unified Data Explorer needs to connect to an instance of the API.
- **Entity Framework Managed Database**: The system requires a relational database to function. We use SQL Server at UIowa but because we go through Entity Framework I imagine other relational database would work.
- **Experiment Console App**: There's a console application in the code base that is used to run our experiments. This app does not go through the API but instead uses the internal libraries to get and execute energy system models, perform experiments, and push the experimental results back to the database.

# Collaboration and License
This repo is copyrighted under the GNU General Public License 3.0, and the license is assumed to apply to all source files in this repo. In short the software and its source code is free to use, modify, and distribute in academic and commercial contexts so long as any derivative versions retain the protections of this license and source code and derivative source code remains open source. If you have copyright inquiries or are interested in collaboration please contact the author at [ryan-policheri@uiowa.edu]().