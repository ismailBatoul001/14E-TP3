using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Utils
{
    public static class ResourceHelper
    {
        public static string LireRessourceTexte(string cheminRessource)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var flux = assembly.GetManifestResourceStream(cheminRessource)
                ?? throw new FileNotFoundException($"Ressource introuvable : {cheminRessource}");

            using var lecteur = new StreamReader(flux);
            return lecteur.ReadToEnd();
        }

        public static Stream LireRessourceStream(string cheminRessource)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return assembly.GetManifestResourceStream(cheminRessource)
                ?? throw new FileNotFoundException($"Ressource introuvable : {cheminRessource}");
        }
    }
}
