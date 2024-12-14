using System.ComponentModel;
using System.ServiceProcess;

namespace FakturowniaService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            // Service will run under the system account
            processInstaller.Account = ServiceAccount.LocalSystem;

            // Set the service name and display name
            serviceInstaller.ServiceName = "VIR_Fakturownia_Import_Service";
            serviceInstaller.DisplayName = "VIR Fakturownia import";
            serviceInstaller.Description = "A VIR Fakturownia import szolgáltatása.";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            // Add the installers to the installer collection
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }

}
