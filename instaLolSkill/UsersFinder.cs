using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace instaLolSkill
{
    class UsersFinder
    {
        /// <summary>
        /// Constantes con la ubicación de directorios del juego y una versión del air Client.
        /// La primer constante almacena la ubicación de la carpeta de League of Legends,
        /// la segunda la ubicación de la carpeta de los releases del air client. 
        /// Finalmente la última constante almacena la última versión que salió del air client. 
        /// No es necesaria ya que la aplicación busca automáticamente la última release, 
        /// pero en caso de que algo fallara (no va a pasar) va a buscar esa release, lo cual 
        /// puede ser un problema si Riot elimina la carpeta con la release anterior al actualizarse. 
        /// Como no lo sé, ahi queda (?) pero funcionar va a funcionar.
        /// </summary>
        private const string gamePath = @"C:\Riot Games\League of Legends";
        private const string airClientPath = @"C:\Riot Games\League of Legends\RADS\projects\lol_air_client\releases";
        private const string airClientRelease = "0.0.1.193";

        private string preferencesPath;

        /// <summary>
        /// Constructor de la clase. Le asigna a la variable preferencesPath 
        /// la ubicación de la carpeta donde se almacenan los usuarios.
        /// 
        /// IMPORTANTE: No hay ningún robo de cuentas ni nada en esta parte. A tener en cuenta:
        /// 1) La aplicación solamente lee el nombre de los archivos, no los abre en ningún momento (se ve más abajo).
        /// 2) League of Legends NO ALMACENA en la computadora la clave de usuario (por eso no hay una opción para
        ///    recordar la contraseña en el juego, y si la hubiera, esta aplicación no hace nada con eso :) ).
        /// </summary>
        public UsersFinder() {
            string release = (getLastRelease() == null) ? airClientRelease : getLastRelease();
            preferencesPath = airClientPath + @"\" + release + @"\deploy\preferences";
        }

        /// <summary>
        /// Acá busca en la carpeta "preferences" del air client los usuarios que han iniciado sesión
        /// en el juego en esa computadora. Air Client (la interfaz que vemos antes de entrar en partida)
        /// guarda preferencias sobre cada usuario que inicia sesión en esa PC (está codificada). Sin 
        /// embargo, el archivo que genera con las preferencias tiene se llama como el nombre del usuario
        /// cuyas preferencias almacena. ESE nombre de archivo es el que la aplicación (en éste método) 
        /// toma. En NINGÚN momento lee tal archivo, ni lo envía a algún lado.
        /// </summary>
        /// <returns>ArrayList con los usuarios encontrados. En caso de no encontrar, null.</returns>
        public ArrayList getCurrentUsers()
        {
            try
            {
                //Me fijo si existe el path del lol
                if(Directory.Exists(gamePath))
                {
                    //Si existe del del air client...
                    if(Directory.Exists(airClientPath))
                    {
                        if (Directory.Exists(preferencesPath))
                        {
                            //Busco todos los archivos con la extension ".properties"
                            string[] propertiesPath = Directory.GetFiles(preferencesPath, "*.properties");
                            //Almaceno los que voy a encontrar. Esto esl o que voy a devolver
                            ArrayList usersFound = new ArrayList();
                            foreach(string file in propertiesPath)
                                if (!file.Contains("shared_"))
                                    usersFound.Add(clearUsername(file));
                            return usersFound;
                        }else {throw new Exception("No se encontró el directorio con las preferencias del air client." + preferencesPath);}
                    }else {throw new Exception("No se encontró el directorio del air client.");}
                }else {throw new Exception("No se encontró el directorio de League of Legends.");}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        /// <summary>
        /// Busca en la carpeta de releases del air client la última versión, ordenando las
        /// carpetas por su fecha de creación de forma descendente (la más nueva primera) y
        /// devuelve el nombre de la carpeta.
        /// </summary>
        /// <returns>Cadena con el nombre de la release (versión)</returns>
        private string getLastRelease()
        {
            try
            {
                if(Directory.Exists(gamePath))
                {
                    if(Directory.Exists(airClientPath))
                    {
                        DirectoryInfo releasesFolder = new DirectoryInfo(airClientPath);
                        return releasesFolder.EnumerateDirectories().OrderBy(rel => rel.CreationTime).Select(rel => rel.Name).First().ToString();
                    }else { throw new Exception("No se encontró el directorio del air client."); }
                }else { throw new Exception("No se encontró el directorio de League of Legends."); }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        /// <summary>
        /// Como el usuario en realidad es la ubicación de un archivo, éste método
        /// agarra ese texto y lo limpia, quitando todo y dejando solamente el 
        /// nombre del usuario.
        /// </summary>
        /// <param name="inUsername">Usuario en "crudo".</param>
        /// <returns>El nombre del usuario limpio.</returns>
        private string clearUsername(string inUsername)
        {
            //Cosas que tiene que limpiar al recibir el usuario
            string[] clearUser = { preferencesPath + @"\", ".properties" };
            foreach(string a in clearUser)
                inUsername = inUsername.Replace(a, "");
            return inUsername;
        }
    }
}
