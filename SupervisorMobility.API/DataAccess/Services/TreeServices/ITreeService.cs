using SupervisorMobility.API.DataAccess.Entities.TreeStruct;
using SupervisorMobility.API.Entities.CDMS.Directory;

namespace SupervisorMobility.API.DataAccess.Services.TreeServices
{
    public interface ITreeService
    {
        public TreeItemData ConstruirArbolCCP(List<FolderCCP> elementos);

        public TreeItemData ConstruirArbolHOE(List<FolderHOE> elementos);

        public TreeItemData ConstruirArbolGOS(List<FolderGOS> elementos);

        public TreeItemData EncontrarMejorCoincidenciaDifusa(TreeItemData nodoActual, string rutaUsuario, string palabraClave);
        public TreeItemData EncontrarMejorCoincidenciaDifusaInternal(TreeItemData nodoActual, string rutaUsuario, string palabraClave);
        public string NormalizarRutaUsuario(string rutaUsuario);
    }
}
