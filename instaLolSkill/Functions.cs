using System;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Diagnostics;

namespace instaLolSkill
{
    class Functions
    {
        /// <summary>
        /// Dos timers. El primero se encarga de buscar el proceso del
        /// juego, y el segundo (activado por el primero al encontrar 
        /// el juego en ejecución) espera la finalización del proceso 
        /// del juego (que termine la partida) para dar comienzo
        /// nuevamente al primer timer.
        /// </summary>
        private Timer processSearchTimer;
        private Timer processEndTimer;

        /// <summary>
        /// Estas dos constantes almacenan el nombre de dos procesos.
        /// El primero es el proceso del cliente de League Of Legends,
        /// el segundo el nombre del proceso de ésta aplicación.
        /// </summary>
        private const string processName = "League of Legends";
        private const string thisProcessName = "instaLolSkill";

        /// <summary>
        /// Constructor de la clase. Instancia los dos timers necesarios:
        /// processSearchTimer para la búsqueda del proceso de League of Legends y
        /// processEndTimer para detectar que finalizó el proceso del juego y buscar
        /// nuevamente.
        /// </summary>
        public Functions()
        {
            processSearchTimer = new Timer();
            processSearchTimer.Interval = 5000; //Ticks de cinco segundos
            processSearchTimer.Enabled = false;
            processSearchTimer.Tick += new EventHandler(process_check);

            processEndTimer = new Timer();
            processEndTimer.Interval = 10000; //Ticks de 10 minutos. Podrían ser 20.
            processEndTimer.Enabled = false;
            processEndTimer.Tick += new EventHandler(process_end_check);
        }

        /// <summary>
        /// Revisa si el proceso de ésta aplicación está en ejecución o no. Sirve 
        /// para evitar que se inicie múltiples veces por el usuario.
        /// </summary>
        /// <returns>
        /// Boolean. Si se encontró el proceso retorna TRUE, caso contrario
        /// FALSE.
        /// </returns>
        public bool alreadyRunning()
        {
            Process[] procesos = Process.GetProcessesByName(thisProcessName);
            return (procesos.Length == 0) ? false : true;
        }

        /// <summary>
        /// Agrega el valor de la aplicación al registro para que esta inicie (o no)
        /// automáticamente al encender el equipo.
        /// </summary>
        /// <param name="status">
        /// Parámetro booleano para saber qué escribir en el registro. Si se recibe 
        /// TRUE, el valor que se escribirá será el de la ubicación de la aplicación,
        /// para que pueda ser ejecutada al iniciar el sistema. En caso de recibir FALSE,
        /// se escribe dicho valor en el registro para que sea ignorado al iniciar el sistema.
        /// </param>
        public void setStartup(bool status)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (status == true)
                rk.SetValue("Insta lol-skill", Application.ExecutablePath.ToString());
            else
                rk.SetValue("Insta lol-skill", false);
        }

        /// <summary>
        /// Habilita el timer processSearchTimer, encargado de buscar el proceso del juego.
        /// </summary>
        public void startLooking()
        {
            processSearchTimer.Enabled = true;
            processSearchTimer.Start();
        }

        /// <summary>
        /// Entra en funcionamiento por cada tick del timer processSearchTimer.
        /// Busca el proceso del juego. Si lo encuentra abre la pestaña de LolSkill,
        /// finaliza el timer e inicia el de processEndTimer.
        /// </summary>
        /// <param name="sender">Objeto que llama al método.</param>
        /// <param name="e">Datos del evento.</param>
        private void process_check(object sender, EventArgs e) 
        {
            Process[] procesos = Process.GetProcessesByName(processName);
            if(procesos.Length > 0) {
                openLolSkill();
                processSearchTimer.Stop();
                processSearchTimer.Enabled = false;
                processEndTimer.Enabled = true;
                processEndTimer.Start();
            }
        }

        /// <summary>
        /// Entra en funcionamiento por cada tick del processEndTimer.
        /// Se fija si el proceso del juego se cerró. En caso de no encontrarlo 
        /// detiene el timer e inicia el processSearchTimer.
        /// </summary>
        /// <param name="sender">Objeto que llama al método.</param>
        /// <param name="e">Datos del evento.</param>
        private void process_end_check(object sender, EventArgs e)
        {
            Process[] procesos = Process.GetProcessesByName(processName);
            if(procesos.Length == 0)
            {
                processEndTimer.Stop();
                processEndTimer.Enabled = false;
                processSearchTimer.Enabled = true;
                processSearchTimer.Start();
            }
        }

        /// <summary>
        /// Revisa si hay guardado un nombre de usuario y región. En caso de haberlos
        /// inicia un proceso con la URL de LolSkill. Toma el navegador que haya por 
        /// defecto.
        /// </summary>
        private void openLolSkill()
        {
            if(Properties.Settings.Default.region != null && Properties.Settings.Default.actual_username != null)
                Process.Start("http://www.lolskill.net/game/" + Properties.Settings.Default.region + "/" + Properties.Settings.Default.actual_username);
        }

        /// <summary>
        /// Abre mi perfil de GitHub (?)
        /// </summary>
        public void openGithub()
        {
            Process.Start("http://www.github.com/sberlati");
        }

        /// <summary>
        /// Devuelve las iniciales de una región.
        /// </summary>
        /// <param name="name">Nombre completo de esa región</param>
        /// <returns>Las iniciales de la región en caso de encontrarla. Caso contrario null.</returns>
        public string convertRegion(string name)
        {
            switch (name)
            {
                case "Europa Nórdica y Este" : return "EUNE";
                case "Europa Oeste" : return "EUW";
                case "Rusia" : return "RU";
                case "Turquía" : return "TR";
                case "Norteamérica" : return "NA";
                case "Brasil" : return "BR";
                case "Latinoamérica Norte" : return "LAN";
                case "Latinoamérica Sur" : return "LAS";
                case "Korea" : return "KR";
                case "Oceanía" : return "OCE";
                default: return null;
            }
        }

        /// <summary>
        /// Lo contrario al método anterior. Toma las iniciales de la región y devuelve
        /// el nombre completo.
        /// </summary>
        /// <param name="initials">Iniciales de la región.</param>
        /// <returns>El nombre de la región. Si no la encuentra devuelve null.</returns>
        public string reverseRegion(string initials)
        {
            switch(initials)
            {
                case "EUNE": return "Europa Nórdica y Este";
                case "EUW": return "Europa Oeste";
                case "RU": return "Rusia";
                case "TR": return "Turquía";
                case "NA": return "Norteamérica";
                case "BR": return "Brasil";
                case "LAN": return "Latinoamérica Norte";
                case "LAS": return "Latinoamérica Sur";
                case "KR": return "Korea";
                case "OCE": return "Oceanía";
                default: return null;
            }
        }
    }
}
