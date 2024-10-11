using SupervisorMobility.API.DataAccess.Entities.TreeStruct;
using SupervisorMobility.API.Entities.CDMS.Directory;
using DuoVia.FuzzyStrings;
using DocumentFormat.OpenXml.Spreadsheet;
using Serilog;
using System.Drawing;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.DataAccess.Entities;
using FuzzyString;
using System.Diagnostics;
using System.Text.RegularExpressions;

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

        public TreeItemData? EncontrarNodoMejorCoincidencia(TreeItemData rootNode, Plant planta, string? departamento, Area? area, Distribution? distribucion, Product? producto)
        {
            var elementosABuscar = new List<(string? Code, string? Description)>
            {
                (planta.Code, planta.Description),
                (departamento, null), 
                (area?.Code, area?.Description),
                (distribucion?.Code, distribucion?.Description),
                (producto?.Code, producto?.Description)
            };

            HashSet<string> terminosEncontrados = new HashSet<string>();

            TreeItemData? ultimoNodoEncontrado = null;

            TreeItemData? resultado = BuscarNodoRecursivo(rootNode, elementosABuscar, ref ultimoNodoEncontrado, terminosEncontrados, (planta.Code, planta.Description));

            return resultado;
        }

        private TreeItemData? BuscarNodoRecursivo(TreeItemData nodoActual, List<(string? Code, string? Description)> elementosABuscar, ref TreeItemData? ultimoNodoEncontrado, HashSet<string> terminosEncontrados, (string? Code, string? Description) planta)
        {
            var coincidencias = new List<(TreeItemData nodo, double puntuacion)>();

            double umbralPlanta = 0.5; 
            double umbralGenerico = 0.6; 


            foreach (var hijo in nodoActual.TreeItems)
            {
                string nombreLimpiado = LimpiarNombre(hijo.Nombre).ToLower(); 

                foreach (var (code, description) in elementosABuscar)
                {
                    double puntuacionCode = !string.IsNullOrEmpty(code) ? CombineSimilarities(code.ToLower(), nombreLimpiado) : 0;
                    double puntuacionDescription = !string.IsNullOrEmpty(description) ? CombineSimilarities(description.ToLower(), nombreLimpiado) : 0;

                    double umbral = (code == planta.Code && description == planta.Description) ? umbralPlanta : umbralGenerico;

                    if (puntuacionCode >= umbral || puntuacionDescription >= umbral)
                    {
                        double mejorPuntuacion = Math.Max(puntuacionCode, puntuacionDescription);
                        coincidencias.Add((hijo, mejorPuntuacion));

                        terminosEncontrados.Add(nombreLimpiado);
                        ultimoNodoEncontrado = hijo; 
                    }
                }

            }

                if (ultimoNodoEncontrado != null && planta.Code != null && planta.Description != null)
                {
                    elementosABuscar.RemoveAll(e => e.Code == planta.Code && e.Description == planta.Description);
                }

                if (coincidencias.Any())
                {
                    foreach (var (nodoCoincidencia, _) in coincidencias.OrderByDescending(c => c.puntuacion))
                    {
                        var nodoEncontrado = BuscarNodoRecursivo(nodoCoincidencia, elementosABuscar, ref ultimoNodoEncontrado, terminosEncontrados, planta);
                        if (nodoEncontrado != null)
                        {
                            return nodoEncontrado; 
                        }
                    }

                }

            return ultimoNodoEncontrado;
        }
           
        private string LimpiarNombre(string nombre)
        {
            //v1
            //string sinPrefijo = Regex.Replace(nombre, @"^\d+§\d+\.\s*", "");

            //// Mantener letras, números, espacios, paréntesis y guiones. Eliminar cualquier otro carácter.
            //string nombreLimpiado = Regex.Replace(sinPrefijo, @"[^a-zA-Z0-9\s\(\)\-\.]", "").Trim();

            //return nombreLimpiado;

            //V0

            // Primero, eliminar cualquier parte antes de un punto o símbolo '§'
            var partes = nombre.Split(new[] { '§', '.' }, StringSplitOptions.RemoveEmptyEntries);

            // Tomar la última parte y limpiar espacios
            var nombreSignificativo = partes.Last().Trim();

            return nombreSignificativo;

        }

        public double CombineSimilaritiesArea(string areaCode, string areaDescription, string excelCode)
        {
            // Primero calculamos la similitud del código
            double codeSimilarity = areaCode.Equals(excelCode)
                ? 1.0
                : 1 - areaCode.JaccardDistance(excelCode);

            // Calculamos la similitud usando la descripción
            double descriptionSimilarity = areaDescription.Equals(excelCode)
                ? 1.0
                : CombineSimilarities(excelCode.ToLower(), areaDescription.ToLower());

            // Definimos un umbral de similitud mínima para considerar que el código es relevante
            double similarityThreshold = 0.7;

            // Si la similitud del código es suficiente, le damos más peso
            if (codeSimilarity >= similarityThreshold)
            {
                double codeWeight = 0.8;
                double descriptionWeight = 0.2;
                return (codeWeight * codeSimilarity) + (descriptionWeight * descriptionSimilarity);
            }
            else
            {
                // Si la similitud del código es baja, damos más peso a la descripción
                double codeWeight = 0.1;
                double descriptionWeight = 0.9;
                return (codeWeight * codeSimilarity) + (descriptionWeight * descriptionSimilarity);
            }
        }

        public double CombineSimilarities(string description1, string description2)
        {
            double wordSimilarityWeight = 0.5;
            double charSimilarityWeight = 0.5;

            double wordSimilarity = 1 - JaccardDistanceByWords(description1, description2);

            double charSimilarity = 1 - description1.JaccardDistance(description2);

            return (wordSimilarityWeight * wordSimilarity) + (charSimilarityWeight * charSimilarity);
        }

        public double JaccardDistanceByWords(string description1, string description2)
        {
            var words1 = description1.Split(' ').Distinct();
            var words2 = description2.Split(' ').Distinct();

            var intersection = words1.Intersect(words2).Count();
            var union = words1.Union(words2).Count();

            return 1 - ((double)intersection / union);
        }
    }//end treeservice
}
