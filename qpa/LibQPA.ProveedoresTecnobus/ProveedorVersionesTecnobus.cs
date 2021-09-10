using Comun;
using System;
using System.Collections.Generic;

namespace LibQPA.ProveedoresTecnobus
{
    public class ProveedorVersionesTecnobus : IQPAProveedorRecorridosTeoricos
    {
        public string[] DirRepos    { get; set; }
        public bool     RepoRandom  { get; set; }

        public ProveedorVersionesTecnobus(string[] dirRepos, bool repoRandom = false)
        {
            DirRepos    = dirRepos;
            RepoRandom  = repoRandom;
        }

        public List<RecorridoLinBan> Get(int[] lineas, DateTime vigenteEn)
        {
            int index = 0; // TODO: hacer random si se pide en RepoRandom...
            return ProveedorVersionesTecnobusHelper.LeerRecorridos(DirRepos[index], lineas, vigenteEn);
        }
    }
}
