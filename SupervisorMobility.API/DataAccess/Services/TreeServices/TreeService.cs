using SupervisorMobility.API.DataAccess.Entities.TreeStruct;
using SupervisorMobility.API.Entities.CDMS.Directory;
using DuoVia.FuzzyStrings;

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

        public TreeItemData FindNodeByPath(TreeItemData rootNode, string path)
        {
            // Divide la ruta en partes
            string[] pathParts = path.Split('/');

            // Comienza desde la raíz del árbol
            TreeItemData currentNode = rootNode;

            foreach (string part in pathParts)
            {
                // Busca el nodo hijo con el nombre actual
                currentNode = currentNode.TreeItems.FirstOrDefault(child => child.Nombre == part);

                // Si no se encuentra un nodo hijo con ese nombre, devuelve null
                if (currentNode == null)
                {
                    return null;
                }
            }

            // Devuelve el nodo encontrado
            return currentNode;
        }


        public TreeItemData EncontrarMejorCoincidenciaDifusa(TreeItemData nodoActual, string rutaUsuario)
        {
            // Itera sobre los nodos del árbol
            TreeItemData mejorCoincidencia = null;
            double puntuacionMaxima = 0;

            // Calcula la puntuación de coincidencia difusa con el nodo actual
            double puntuacion = nodoActual.Ruta.DiceCoefficient(rutaUsuario);

            // Actualiza la mejor coincidencia si la puntuación es mayor
            if (puntuacion > puntuacionMaxima)
            {
                puntuacionMaxima = puntuacion;
                mejorCoincidencia = nodoActual;
            }

            // Itera sobre los hijos de forma recursiva
            foreach (var hijo in nodoActual.TreeItems)
            {
                TreeItemData mejorCoincidenciaHijo = EncontrarMejorCoincidenciaDifusa(hijo, rutaUsuario);

                // Actualiza la mejor coincidencia si la del hijo es mejor
                if (mejorCoincidenciaHijo != null && mejorCoincidenciaHijo.Ruta.DiceCoefficient(rutaUsuario) > puntuacionMaxima)
                {
                    puntuacionMaxima = mejorCoincidenciaHijo.Ruta.DiceCoefficient(rutaUsuario);
                    mejorCoincidencia = mejorCoincidenciaHijo;
                }
            }

            return mejorCoincidencia;
        }



        public string NormalizarRutaUsuario(string rutaUsuario)
        {
            // Eliminar espacios adicionales y convertir números a su forma sin ceros a la izquierda
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
