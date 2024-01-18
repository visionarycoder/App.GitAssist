using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Xml.Linq;

using vc.GitHelper.Helpers;
using vc.GitHelper.Models;

namespace vc.GitHelper;

public class ConsoleClient
{

    public ConsoleClient()
    {
    }

    public void Run(string[] args)
    {

        ConsoleHelper.DisplayHeader();
        var repositories = DefineRepositories();
        ProcessRepositories(repositories);
        ConsoleHelper.DisplayExit();

    }

    private List<Repository> DefineRepositories()
    {
        // Example https://git.vspglobal.com/scm/pm/access-employee.git

        var repoNames = PopulateRepoNames();
        var repositories = repoNames.Select(repoName => new Repository
        {
            Source = Constant.Source.PREFIX + repoName.Trim().ToLower() + Constant.Source.SUFFIX,
            Status = Constant.Status.NOT_PROCESSED,
            DirectoryInfo = new DirectoryInfo(Path.Combine(Constant.FileSystem.ROOT_FOLDER, repoName.Trim()))
        }).ToList();
        foreach (var repo in repositories.Where(r => !r.DirectoryInfo.Exists))
        {
            repo.DirectoryInfo.Create();
        }
        return repositories;

    }

    private void ProcessRepositories(IEnumerable<Repository> repositories)
    {

        foreach (var repo in repositories)
        {
            ConsoleHelper.DisplayUpdate($"Processing {repo.DirectoryInfo.FullName}");
            if (!repo.DirectoryInfo.Exists)
            {
                ConsoleHelper.DisplayUpdate($"Creating {repo.DirectoryInfo.FullName}");
                repo.DirectoryInfo.Create();
            }
            
            do
            {
                switch (repo.Status)
                {
                    case Constant.Status.NOT_PROCESSED:
                    case Constant.Status.PULL:
                        {
                            repo.Status = Pull(repo.DirectoryInfo);
                            break;
                        }
                    case Constant.Status.RESET:
                        {
                            repo.Status = Reset(repo.DirectoryInfo);
                            break;
                        }
                    case Constant.Status.CLONE:
                        {
                            repo.Status = Clone(repo.DirectoryInfo);
                            break;
                        }
                }
            } while (repo.Status != Constant.Status.FINISHED);
            Console.WriteLine();
        }

    }

    private string Clone(DirectoryInfo di)
    {

        var processCommand = ProcessCommandFactory.CreateCloneCommand(di.Parent.FullName, di.Name);
        var startInfo = ProcessHelper.CreateStartInfo(processCommand);
        var process = ProcessHelper.CreateProcess(startInfo);
        var (stdOut, errOut) = ProcessHelper.ExecuteProcess(process);
        var workflowState = ValidateResult(stdOut, errOut);

        ConsoleHelper.DisplayUpdate(process.StartInfo.Arguments);
        ConsoleHelper.DisplayUpdate(stdOut);
        ConsoleHelper.DisplayUpdate(errOut, true);
        return workflowState;

    }

    private string Reset(DirectoryInfo directory)
    {

        var processCommand = ProcessCommandFactory.CreateResetCommand(directory.FullName);
        var startInfo = ProcessHelper.CreateStartInfo(processCommand);
        var process = ProcessHelper.CreateProcess(startInfo);
        var (stdOut, errOut) = ProcessHelper.ExecuteProcess(process);
        var workflowState = ValidateResult(stdOut, errOut);

        ConsoleHelper.DisplayUpdate(process.StartInfo.Arguments);
        ConsoleHelper.DisplayUpdate(stdOut);
        ConsoleHelper.DisplayUpdate(errOut, true);
        return workflowState;

    }

    private string Pull(DirectoryInfo directory)
    {

        var processCommand = ProcessCommandFactory.CreatePullCommand(directory.FullName);
        var startInfo = ProcessHelper.CreateStartInfo(processCommand);
        var process = ProcessHelper.CreateProcess(startInfo);
        var (stdOut, errOut) = ProcessHelper.ExecuteProcess(process);
        var workflowState = ValidateResult(stdOut, errOut);

        ConsoleHelper.DisplayUpdate(process.StartInfo.Arguments);
        ConsoleHelper.DisplayUpdate(stdOut);
        ConsoleHelper.DisplayUpdate(errOut, true);
        return workflowState;

    }

    private DirectoryInfo DeterminePath(string[] args)
    {

        do
        {
            var input = @"C:\Dev\VSP\Eyefinity.PM"; // ConsoleHelper.RequestInput("Enter path: ", @"C:\Dev\VSP\Eyefinity.PM");
            input = input?.Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                ConsoleHelper.DisplayUpdate("Invalid path.", true);
                continue;
            }
            var validationResult = EnvironmentHelper.VerifyPath(input);
            if (validationResult != ValidationResult.Success)
            {
                ConsoleHelper.DisplayUpdate("Invalid path.", true);
                continue;
            }
            return EnvironmentHelper.GetDirectoryInfo(input);
        } while (true);

    }

    private string ValidateResult(string stdOut, string errOut)
    {

        errOut = errOut.ToLower().Trim();
        if (errOut.Contains("error:") && errOut.Contains("unmerged"))
        {
            return Constant.Status.RESET;
        }

        if (errOut.Contains("fatal:") && errOut.Contains("not a git repository"))
        {
            return Constant.Status.CLONE;
        }

        stdOut = stdOut.ToLower().Trim();
        if (stdOut.Contains("already up to date"))
        {
            return Constant.Status.FINISHED;
        }

        return Constant.Status.FINISHED;
    }

    private List<string> PopulateRepoNames()
    {
        var list = new List<string>
        {
            "access-amazonremotestorage",
            "access-attribute",
            "access-calendar",
            "access-catalog",
            "access-catalogrules",
            "access-claim",
            "access-configuration",
            "access-document",
            "access-employee",
            "access-exam",
            "access-eyefinitysalestax",
            "access-fulfillment",
            "access-insurance",
            "access-integratedsolutionsconfig",
            "access-inventory",
            "access-jobson",
            "access-lab",
            "access-location",
            "access-marchon",
            "access-mmifhir",
            "access-organization",
            "access-package",
            "access-patient",
            "access-primarycatalog",
            "access-promotion",
            "access-questionnaire",
            "access-referencedata",
            "access-sales",
            "access-salestax",
            "access-sharedservices",
            "access-trizetto",
            "access-visionweb",
            "access-workflow",
            "CHIntegration",
            "ChwMockApi",
            "client-businessmanagement-web",
            "client-catalogcuration-api",
            "client-clinical-api",
            "client-content-web",
            "client-fulfillment-api",
            "client-insurance-api",
            "client-patientdocument-api",
            "client-practicemanagement-web",
            "client-userexperience-api",
            "database-catalog",
            "dotnet-templates",
            "engine-billingclaimprocessing",
            "engine-businessconfiguration",
            "engine-calendar",
            "engine-catalogentrybuilder",
            "engine-catalogentryloader",
            "engine-catalogentryvalidator",
            "engine-eligibility",
            "engine-laborderprocessing",
            "engine-ordervalidating",
            "engine-patientdocument",
            "engine-patientinsurance",
            "engine-pricing",
            "engine-questionnaire",
            "engine-recommendation",
            "engine-referencedatavalidation",
            "engine-rx",
            "engine-sales",
            "engine-stockorderfulfillment",
            "EyefinityPlatformScheduler",
            "EyefinityPlatformScheduler.Configuration",
            "framework",
            "hosting-businessconfiguration",
            "hosting-clinical",
            "hosting-fulfillment",
            "hosting-insurance",
            "hosting-referencedata",
            "hosting-retail",
            "hosting-schedulemanager",
            "manager-businessconfiguration",
            "manager-catalogcuration",
            "manager-claim",
            "manager-clinical",
            "manager-fulfillment",
            "manager-insurance",
            "manager-mmipatient",
            "manager-patientdocument",
            "manager-referencedata",
            "manager-retail",
            "manager-schedule",
            "manager-userexperience",
            "OAuthIntegration",
            "resource-iam",
            "resource-sharedservices",
            "TestHarness",
            "training-vbd",
            "utility-auditlogging",
            "utility-errorhandling",
            "utility-featureflagging",
            "utility-servicemessaging"
        };
        return list;
    }
}