using System;

namespace ArticulosSAP
{
    public class ArtMATHEAD_Enc
    {
        /*public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }

        public string TokenBus { get; set; }
        */
        public string Material_MATNR { get; set; }
        public string TipoMaterial_MATL_TYPE { get; set; }
        public string GpoArt_MATL_GROUP { get; set; }
        public string CategMat_MATL_CAT { get; set; }
        public string UltiDoc_NUMDOC { get; set; }
        public DateTime Fec_Movto { get; set; }
}
}
