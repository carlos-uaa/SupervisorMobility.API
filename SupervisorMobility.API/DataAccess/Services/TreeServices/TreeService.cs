using SupervisorMobility.API.DataAccess.Entities.TreeStruct;
using SupervisorMobility.API.Entities.CDMS.Directory;
using DuoVia.FuzzyStrings;
using DocumentFormat.OpenXml.Spreadsheet;
using Serilog;
using System.Drawing;

namespace SupervisorMobility.API.DataAccess.Services.TreeServices
{
    public class TreeService: ITreeService
    {


        public TreeItemData ConstruirArbolCCP(List<FolderCCP> elementos)
        {
            TreeItemData root = new TreeItemData { Nombre = "Raíz", Ruta = "", EsDirectorio = true };
            root.TreeItems = new HashSet<TreeItemData>();

            foreach (var itemData in elementos)
            {
                // Dividir la ruta en partes y crear cada nodo del árbol
                string[] rutaPartes = itemData.ruta.Split('/');
                TreeItemData parent = root;

                for (int i = 0; i < rutaPartes.Length; i++)
                {
                    string nombre = rutaPartes[i];

                    // Buscar el nodo en los hijos del padre actual
                    TreeItemData nodoActual = parent.TreeItems.ToList().Find(child => child.Nombre == nombre);

                    if (nodoActual == null)
                    {
                        // Si el nodo no existe, crearlo y agregarlo como hijo del padre actual
                        nodoActual = new TreeItemData() { Nombre = nombre, Ruta = itemData.ruta, EsDirectorio = true };
                        nodoActual.TreeItems = new HashSet<TreeItemData>();
                        parent.TreeItems.Add(nodoActual);
                    }

                    // Actualizar el padre actual
                    parent = nodoActual;
                }

                // Agregar el nodo final (hoja)
                //TreeItemData hoja = new TreeItemData { Nombre = itemData.Nombre, Ruta = itemData.Ruta, EsDirectorio = true };
                //hoja.TreeItems = null;
                //parent.TreeItems.Add(hoja);
            }

            // Imprimir el árbol
            return root;
        }
        public TreeItemData ConstruirArbolHOE(List<FolderHOE> elementos)
        {
            TreeItemData root = new TreeItemData { Nombre = "Raíz", Ruta = "", EsDirectorio = true };
            root.TreeItems = new HashSet<TreeItemData>();

            foreach (var itemData in elementos)
            {
                // Dividir la ruta en partes y crear cada nodo del árbol
                string[] rutaPartes = itemData.ruta.Split('/');
                TreeItemData parent = root;

                for (int i = 0; i < rutaPartes.Length; i++)
                {
                    string nombre = rutaPartes[i];

                    // Buscar el nodo en los hijos del padre actual
                    TreeItemData nodoActual = parent.TreeItems.ToList().Find(child => child.Nombre == nombre);

                    if (nodoActual == null)
                    {
                        // Si el nodo no existe, crearlo y agregarlo como hijo del padre actual
                        nodoActual = new TreeItemData() { Nombre = nombre, Ruta = itemData.ruta, EsDirectorio = true };
                        nodoActual.TreeItems = new HashSet<TreeItemData>();
                        parent.TreeItems.Add(nodoActual);
                    }

                    // Actualizar el padre actual
                    parent = nodoActual;
                }

                // Agregar el nodo final (hoja)
                //TreeItemData hoja = new TreeItemData { Nombre = itemData.Nombre, Ruta = itemData.Ruta, EsDirectorio = true };
                //hoja.TreeItems = null;
                //parent.TreeItems.Add(hoja);
            }

            // Imprimir el árbol
            return root;
        }
        public TreeItemData ConstruirArbolGOS(List<FolderGOS> elementos)
        {
            TreeItemData root = new TreeItemData { Nombre = "Raíz", Ruta = "", EsDirectorio = true };
            root.TreeItems = new HashSet<TreeItemData>();

            foreach (var itemData in elementos)
            {
                // Dividir la ruta en partes y crear cada nodo del árbol
                string[] rutaPartes = itemData.ruta.Split('/');
                TreeItemData parent = root;

                for (int i = 0; i < rutaPartes.Length; i++)
                {
                    string nombre = rutaPartes[i];

                    // Buscar el nodo en los hijos del padre actual
                    TreeItemData nodoActual = parent.TreeItems.ToList().Find(child => child.Nombre == nombre);

                    if (nodoActual == null)
                    {
                        // Si el nodo no existe, crearlo y agregarlo como hijo del padre actual
                        nodoActual = new TreeItemData() { Nombre = nombre, Ruta = itemData.ruta, EsDirectorio = true };
                        nodoActual.TreeItems = new HashSet<TreeItemData>();
                        parent.TreeItems.Add(nodoActual);
                    }

                    // Actualizar el padre actual
                    parent = nodoActual;
                }

                // Agregar el nodo final (hoja)
                //TreeItemData hoja = new TreeItemData { Nombre = itemData.Nombre, Ruta = itemData.Ruta, EsDirectorio = true };
                //hoja.TreeItems = null;
                //parent.TreeItems.Add(hoja);
            }

            // Imprimir el árbol
            return root;
        }


        public TreeItemData EncontrarMejorCoincidenciaDifusa(TreeItemData nodoActual, string rutaUsuario, string palabraClave)
        {
            TreeItemData mejorCoincidencia = null;
            double puntuacionMaxima = 0;

            double puntuacion = nodoActual.Ruta.DiceCoefficient(rutaUsuario);

            bool contienePalabraClave = nodoActual.Ruta.Contains(palabraClave);

            if (contienePalabraClave && (mejorCoincidencia == null || puntuacion > puntuacionMaxima))
            {
                puntuacionMaxima = puntuacion;
                mejorCoincidencia = nodoActual;
            }
            else if (!contienePalabraClave && puntuacion > puntuacionMaxima)
            {
                mejorCoincidencia = nodoActual;
            }

            foreach (var hijo in nodoActual.TreeItems)
            {
                TreeItemData mejorCoincidenciaHijo = EncontrarMejorCoincidenciaDifusa(hijo, rutaUsuario, palabraClave);
                if (mejorCoincidenciaHijo != null)
                {
                    bool contienePalabraClaveHijo = mejorCoincidenciaHijo.Ruta.Contains(palabraClave);

                    if (contienePalabraClaveHijo && mejorCoincidenciaHijo.Ruta.DiceCoefficient(rutaUsuario) > puntuacionMaxima)
                    {
                        puntuacionMaxima = mejorCoincidenciaHijo.Ruta.DiceCoefficient(rutaUsuario);
                        mejorCoincidencia = mejorCoincidenciaHijo;
                    }
                    else if (!contienePalabraClaveHijo && mejorCoincidenciaHijo.Ruta.DiceCoefficient(rutaUsuario) > puntuacionMaxima)
                    {
                        mejorCoincidencia = mejorCoincidenciaHijo;
                    }
                }
            }

            return mejorCoincidencia;
        }

        public TreeItemData EncontrarMejorCoincidenciaDifusaInternal(TreeItemData nodoActual, string rutaUsuario, string palabraClave)
        {
            TreeItemData mejorCoincidencia = null;
            double puntuacionMaxima = 0;

            double puntuacion = nodoActual.Ruta.DiceCoefficient(rutaUsuario);
            bool contienePalabraClave = palabraClave != null && nodoActual.Nombre.Contains(palabraClave);

            if ((contienePalabraClave && puntuacion > puntuacionMaxima) ||
                (!contienePalabraClave && puntuacion > puntuacionMaxima))
            {
                puntuacionMaxima = puntuacion;
                mejorCoincidencia = nodoActual;
            }

            foreach (var hijo in nodoActual.TreeItems)
            {
                TreeItemData mejorCoincidenciaHijo = EncontrarMejorCoincidenciaDifusaInternal(hijo, rutaUsuario, palabraClave);

                if (mejorCoincidenciaHijo != null)
                {
                    double puntuacionHijo = mejorCoincidenciaHijo.Ruta.DiceCoefficient(rutaUsuario);
                    bool contienePalabraClaveHijo = palabraClave != null && mejorCoincidenciaHijo.Nombre.Contains(palabraClave);

                    if ((contienePalabraClaveHijo && puntuacionHijo > puntuacionMaxima) ||
                        (!contienePalabraClaveHijo && puntuacionHijo > puntuacionMaxima))
                    {
                        puntuacionMaxima = puntuacionHijo;
                        mejorCoincidencia = mejorCoincidenciaHijo;
                    }
                }
            }

            return mejorCoincidencia;
        }




        public string NormalizarRutaUsuario(string rutaUsuario)
        {
            string[] segmentos = rutaUsuario.Split(' ');
            for (int i = 0; i < segmentos.Length; i++)
            {
                if (int.TryParse(segmentos[i], out int numero))
                {
                    segmentos[i] = numero.ToString();
                }
            }
            return string.Join(" ", segmentos);
        }
    }
}
