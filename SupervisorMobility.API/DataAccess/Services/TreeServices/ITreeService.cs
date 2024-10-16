using Microsoft.Identity.Client;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.TreeStruct;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Entities.CDMS.Directory;

namespace SupervisorMobility.API.DataAccess.Services.TreeServices
{
    public interface ITreeService
    {
        public TreeItemData ConstruirArbolCCP(List<FolderCCP> elementos);

        public TreeItemData ConstruirArbolHOE(List<FolderHOE> elementos);

        public TreeItemData ConstruirArbolGOS(List<FolderGOS> elementos);

        public double CombineSimilaritiesArea(string areaCode, string areaDescription, string excelCode);
        public double CombineSimilarities(string description1, string description2);
        public double JaccardDistanceByWords(string description1, string description2);
        TreeItemData? EncontrarNodoMejorCoincidencia(TreeItemData nodoRaiz, Plant planta, string? departamento, Area area, Distribution? distribucion, Product? producto);
    }
}
