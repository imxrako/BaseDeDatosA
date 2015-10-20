﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BaseDeDatos
{
    class Secuencial : Organizacion
    {
        public Secuencial(string nombre, Usuario us) : base(nombre, us, "Secuencial")
        {

        }
        #region ----------------------------------ENTIDADES-----------------------------------------------------
        public bool agregaEntidades(List<Entidad> listEnt)
        {
            bool band = false;

            if (listEnt != null)
            {
                foreach (Entidad ent in listEnt)
                {
                    band = base.altaEntidad(ent);
                    if (band)
                    {
                        foreach (Atributo atr in ent.listAtr)
                        {
                            this.altaAtributo(ent, atr, false);
                        }
                    }
                }
            }


            return band;
        }

        /// <summary>
        /// Lee una entidad que se encuentra en una determinada posición del archivo
        /// </summary>
        /// <param name="pos">Posición de la entidad en el archivo</param>
        /// <returns>Regresa la entidad busacada</returns>
        protected override Entidad leeEntidad(long pos)
        {
            EntSecuencial aEnt = new EntSecuencial();

            try
            {
                using (FileStream fs = new FileStream(base.ruta, FileMode.Open))
                {
                    fs.Seek(pos, SeekOrigin.Begin);
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        aEnt.nombre = br.ReadString();
                        aEnt.apAtr = br.ReadInt64();
                        aEnt.apBloq = br.ReadInt64();
                        aEnt.apRef = br.ReadInt64();
                        aEnt.sigEnt = br.ReadInt64();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            return aEnt;
        }


        /// <summary>
        /// Graba en el archivo la entidad
        /// </summary>
        /// <param name="path">Ruta del archivo</param>
        /// <param name="ent">Entidad a grabar en el archivo</param>
        /// <returns></returns>
        protected override long insertaEntidad(Entidad ent)
        {
            long pos = 0;

            try
            {
                using (FileStream fs = new FileStream(base.ruta, FileMode.Append))
                {
                    pos = fs.Position;
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(ent.nombre);
                        bw.Write((ent as EntSecuencial).apAtr);
                        bw.Write((ent as EntSecuencial).apBloq);
                        bw.Write((ent as EntSecuencial).apRef);
                        bw.Write(ent.sigEnt);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            return pos;
        }

        /// <summary>
        /// Reescribe la entidad en el archivo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ent"></param>
        /// <param name="pos"></param>
        protected override bool reescribeEntidad(Entidad ent, long pos)//  <=============Cuidado al reescribir el  nombre==========
        {
            bool band = false;

            try
            {
                using (FileStream fs = new FileStream(base.ruta, FileMode.Open, FileAccess.Write))
                {
                    fs.Seek(pos, SeekOrigin.Begin);
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(ent.nombre);
                        bw.Write((ent as EntSecuencial).apAtr);
                        bw.Write((ent as EntSecuencial).apBloq);
                        bw.Write((ent as EntSecuencial).apRef);
                        bw.Write(ent.sigEnt);
                    }
                }
                band = true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            return band;
        }



        #endregion

        #region-----------------------------------------------------ATRIBUTOS-----------------------------------------------------

        public override bool altaAtributo(string nomEnt, Atributo atr, bool orden)
        {
            bool band = false;
            Entidad aEnt;

            aEnt = this.buscaEntidad(nomEnt);
            this.altaAtributo(aEnt, atr, orden);

            return band;
        }

        /// <summary>
        /// Da de alta un atributo de una entidad
        /// </summary>
        /// <param name="nomEnt">Entidad en la que se va a crear el atributo</param>
        /// <param name="nombreAtr">nombre del atributo</param>
        /// <param name="tipo">tipo del atributo</param>
        /// <returns></returns>
        public bool altaAtributo(Entidad ent, Atributo atr, bool orden)
        {
            bool band = false;

            //            if (this.activeUser.permisos[1] == true)
            //            {
            if (this.buscaAtributo(ent, atr.nombre) == null)
            {
                if (orden)
                {
                    band = this.insertaAtrPrincipio(ent, atr);
                }
                else
                {
                    band = this.insertaAtrFinal(ent, atr);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Ya existe un atributo con el mismo nombre");
            }
            /*            }
                                    else
                                    {
                                        System.Windows.Forms.MessageBox.Show("No tienes permiso para altas");
                                    }
            */

            return band;
        }

        /// <summary>
        /// Lee un atributo del archivo
        /// </summary>
        /// <param name="pos">Posicion del atributo en el archivo</param>
        /// <returns>Regresa el atributo</returns>
        public override Atributo leeAtributo(long pos)
        {
            Atributo aAtr = new Atributo();

            try
            {
                using (FileStream fs = new FileStream(base.ruta, FileMode.Open))
                {
                    fs.Seek(pos, SeekOrigin.Begin);
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        aAtr.llave = br.ReadChar();
                        aAtr.nombre = br.ReadString();
                        aAtr.tipo = br.ReadString();
                        aAtr.campo = br.ReadString();
                        aAtr.comentario = br.ReadString();
                        aAtr.sigAtr = br.ReadInt64();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            return aAtr;
        }

        /// <summary>
        /// Inserta al principio y liga el atributo
        /// </summary>
        /// <param name="ent"> Entidad que va a tener el atributo</param>
        /// <param name="atr"> atributo a insertar</param>
        /// <param name="atr"> Ruta del archivo</param>
        /// <returns>true si se insertó correctamente</returns>
        private bool insertaAtrPrincipio(Entidad ent, Atributo atr)
        {
            bool band = false;
            long posAtr, posEnt;


            atr.sigAtr = (ent as EntSecuencial).apAtr;
            posAtr = this.insertaAtributo(atr);
            (ent as EntSecuencial).apAtr = posAtr;
            posEnt = base.buscaPosEntidad(ent.nombre);
            if (posEnt != -1)
            {
                band = this.reescribeEntidad(ent, posEnt);
            }

            return band;
        }

        /// <summary>
        /// Inserta al final y liga el atributo
        /// </summary>
        /// <param name="ent">Entidad que va a tener el atributo</param>
        /// <param name="atr"> atributo a insertar</param>
        /// <param name="atr"> Ruta del archivo</param>
        /// <returns>true si se insertó correctamente</returns>
        private bool insertaAtrFinal(Entidad ent, Atributo atr)
        {
            bool band = false;
            long posAtr, posIt, posEnt;
            Atributo aAtr;

            posAtr = this.insertaAtributo(atr);
            if (posAtr != 0)
            {
                band = true;
            }
            if ((ent as EntSecuencial).apAtr == -1)
            {
                (ent as EntSecuencial).apAtr = posAtr;
                posEnt = this.buscaPosEntidad(ent.nombre);
                if (posEnt != -1)
                {
                    band = this.reescribeEntidad(ent, posEnt);
                }
            }
            else
            {
                posIt = (ent as EntSecuencial).apAtr;
                do
                {
                    aAtr = this.leeAtributo(posIt);
                    if (aAtr.sigAtr != -1)
                    {
                        posIt = aAtr.sigAtr;
                    }
                } while (aAtr.sigAtr != -1);
                aAtr.sigAtr = posAtr;
                band = this.reescribeAtributo(aAtr, posIt);
            }

            return band;
        }


        public override long insertaAtributo(Atributo atr)
        {
            long pos = 0;

            try
            {
                using (FileStream fs = new FileStream(base.ruta, FileMode.Append))
                {
                    pos = fs.Position;
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(atr.llave);
                        bw.Write(atr.nombre);
                        bw.Write(atr.tipo);
                        bw.Write(atr.campo);
                        bw.Write(atr.comentarioCompleto);
                        bw.Write(atr.sigAtr);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                pos = 0;
            }

            return pos;
        }


        /// <summary>
        /// Busca un atributo existente en el archivo
        /// </summary>
        /// <param name="">Nombre de la entidad que contiene el atributo</param>
        /// <param name="">Nombre del atributo</param>
        /// <returns>Atributo.</returns>
        public override Atributo buscaAtributo(Entidad ent, string nomAtr)
        {
            Atributo aAtr = null;
            long pos;


            if (ent != null)
            {
                pos = (ent as EntSecuencial).apAtr;
                if (pos != -1)
                {
                    do
                    {
                        aAtr = this.leeAtributo(pos);
                        pos = aAtr.sigAtr;
                    }
                    while (!nomAtr.Equals(aAtr.nombre) && pos != -1);
                    if (!nomAtr.Equals(aAtr.nombre))
                    {
                        aAtr = null;
                    }
                }
            }

            return aAtr;
        }

        public override long buscaPosAtributo(string nomEnt, string nomAtr)
        {
            Atributo aAtr = null;
            Entidad aEnt;
            long pos = -1;

            aEnt = this.buscaEntidad(nomEnt);
            if (aEnt != null)
            {
                pos = (aEnt as EntSecuencial).apAtr;
                if (pos != -1)
                {
                    do
                    {
                        aAtr = this.leeAtributo(pos);
                        if (aAtr.sigAtr != -1 && !aAtr.nombre.Equals(nomAtr))
                        {
                            pos = aAtr.sigAtr;
                        }
                    }
                    while (!nomAtr.Equals(aAtr.nombre));
                }
            }

            return pos;
        }


        public override bool reescribeAtributo(Atributo atr, long pos)
        {
            bool band = true;

            try
            {
                using (FileStream fs = new FileStream(base.ruta, FileMode.Open, FileAccess.Write))
                {
                    fs.Seek(pos, SeekOrigin.Begin);
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(atr.llave);
                        bw.Write(atr.nombre);
                        bw.Write(atr.tipo);
                        bw.Write(atr.campo);
                        bw.Write(atr.comentarioCompleto);
                        bw.Write(atr.sigAtr);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                band = false;
            }

            return band;
        }

        /// <summary>
        /// Regresa una lista de attributos con la entidad dada.
        /// </summary>
        /// <param name="nomEnt">Nombre de la entidad que contiene los atributos</param>
        /// <returns></returns>
        public override List<Atributo> listaAtributos(string nomEnt)
        {
            Entidad ent;
            Atributo aAtr;
            List<Atributo> listAtr = new List<Atributo>();
            long pos;

            ent = this.buscaEntidad(nomEnt);
            pos = (ent as EntSecuencial).apAtr;
            while (pos != -1)
            {
                aAtr = this.leeAtributo(pos);
                listAtr.Add(aAtr);
                pos = aAtr.sigAtr;
            }

            return listAtr;
        }

        #endregion


        #region ----------------------------------BLOQUE-----------------------------------------------------

        public override bool altaBloque(Entidad ent, byte[] b)
        {
            long pos;
            bool band;

            pos = Archivo.escribeBloque(base.ruta, b);
            band = this.ligaBloque(ent, pos);

            return band;
        }

        private bool ligaBloque(Entidad ent, long pos)
        {
            bool band = false;
            long posEnt, posIt;
            byte[] bloq = null;
            long apSigBloq;

            if ((ent as EntSecuencial).apBloq == -1)
            {
                (ent as EntSecuencial).apBloq = pos;
                posEnt = this.buscaPosEntidad(ent.nombre);
                if (posEnt != -1)
                {
                    band = this.reescribeEntidad(ent, posEnt);
                }
            }
            else
            {
                posIt = (ent as EntSecuencial).apBloq;
                do
                {
                    bloq = Archivo.leeBloque(base.ruta, Bloque.calculaTamBloque(ent.listAtr), posIt);
                    apSigBloq = Bloque.leeApBloq(bloq);
                    if (apSigBloq != -1)
                    {
                        posIt = apSigBloq;
                    }
                } while (apSigBloq != -1);
                Bloque.reescribeApSigBloq(pos, bloq);
                band = Archivo.reescribeBloque(base.ruta, bloq, posIt);
            }

            return band;
        }



        #endregion

        /// <summary>
        /// regresa todos los registros de la entidad
        /// </summary>
        /// <param name="ent">Entidad con sus atributos</param>
        public override List<byte[]> listaBloques(Entidad ent)
        {
            List<byte[]> listBloq = new List<byte[]>();
            byte[] bloq;

            long pos = (ent as EntSecuencial).apBloq;
            int tamBloq = Bloque.calculaTamBloque(ent.listAtr);

            while (pos != -1)
            {
                bloq = Archivo.leeBloque(base.ruta, tamBloq, pos);
                listBloq.Add(bloq);
                pos = Bloque.leeApBloq(bloq);
            }

            return listBloq;
        }

    }


}
