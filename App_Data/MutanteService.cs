using CerebroXMenAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace CerebroXMenAPI.app_data
{
    public class MutanteService
    {
        private readonly ConexionSQLServerAutogestionWeb _cn;

        public MutanteService(ConexionSQLServerAutogestionWeb Cn)
        {
            _cn = Cn;
        }

        public bool EsMutante(string[] Genes)
        {
            string genesEnString = ArrayToString(Genes);
            bool existe = Gen.ExisteGen(_cn, genesEnString);

            int contador = BuscarEnHorizontal(Genes);
            contador += BuscarEnVertical(Genes);
            contador += BuscarEnDiagonalDerechaAIzquierda(Genes);
            contador += BuscarEnDiagonalIzquierdaADerecha(Genes);

            bool esMutante = (contador >= 2);
            if (existe == false)
            {
                Gen gen = new Gen(true)
                {
                    ADN = genesEnString,
                    EsMutante = esMutante,
                    FechaAlta = DateTime.Now
                };
                gen.Insertar(_cn);
            }

            if (esMutante) return true;
            else return false;
        }

        public static int BuscarEnVertical(string[] Genes)
        {
            string[] dnaTranspuesta = TransponerMatriz(Genes);
            int contadorCadenas = BuscarEnHorizontal(dnaTranspuesta);
            return contadorCadenas;
        }

        public static int BuscarEnDiagonalDerechaAIzquierda(string[] Genes)
        {
            string[] dnaExtendida = ExtenderMatriz(Genes, false);
            string[] dnaExtendidaTranspuesta = TransponerMatriz(dnaExtendida);
            int contadorCadenas = BuscarEnHorizontal(dnaExtendidaTranspuesta);
            return contadorCadenas;
        }

        public static int BuscarEnDiagonalIzquierdaADerecha(string[] Genes)
        {
            string[] dnaInvertidaExtendida = ExtenderMatriz(Genes, true);
            string[] dnaInvertidaExtendidaTranspuesta = TransponerMatriz(dnaInvertidaExtendida);
            int contadorCadenas = BuscarEnHorizontal(dnaInvertidaExtendidaTranspuesta);
            return contadorCadenas;
        }

        public static int BuscarEnHorizontal(string[] Genes)
        {
            int contadorCadenas = 0;

            foreach (string cadena in Genes)
            {
                int contadorRepeticiones = 0;
                char caracterARepetir = ' ';
                foreach (char caracter in cadena.ToArray())
                {
                    if (caracter == '-')
                    {
                        contadorRepeticiones = 0;
                    }
                    if (char.IsWhiteSpace(caracterARepetir))
                    {
                        caracterARepetir = caracter;
                        contadorRepeticiones++;
                        if (contadorRepeticiones == 4)
                        {
                            contadorCadenas++;
                            break;
                        }
                        continue;
                    }

                    if (caracterARepetir == caracter)
                    {
                        contadorRepeticiones++;
                        if (contadorRepeticiones == 4)
                        {
                            contadorCadenas++;
                            break;
                        }
                    }
                    else
                    {
                        caracterARepetir = caracter;
                        contadorRepeticiones = 1;
                    }

                }
            }
            return contadorCadenas;
        }

        public static string[] TransponerMatriz(string[] Matris)
        {
            string[] matrizTranspolada = new string[Matris.Length];
            for (int i = 0; i < Matris.Length; i++)
            {
                for (int j = 0; j < Matris[0].Length; j++)
                {
                    matrizTranspolada[j] += Matris[i][j];
                }
            }
            return matrizTranspolada;
        }

        public static string[] ExtenderMatriz(string[] Matris, bool Invertir)
        {
            string[] matrizExtendida = new string[Matris.Length + (Matris.Length - 1)];
            int inicio = -1;
            int fin = Matris.Length + (Matris.Length - 1);
            for (int i = 0; i < Matris.Length; i++)
            {
                inicio++;

                matrizExtendida[i] += Matris[i];
                if (Invertir)
                {
                    matrizExtendida[i] = matrizExtendida[i].PadRight(inicio + Matris.Length, '-');
                    matrizExtendida[i] = matrizExtendida[i].PadLeft(fin, '-');
                }
                else
                {
                    matrizExtendida[i] = matrizExtendida[i].PadLeft(inicio + Matris.Length, '-');
                    matrizExtendida[i] = matrizExtendida[i].PadRight(fin, '-');
                }
            }
            for (int i = 0; i < matrizExtendida.Length; i++)
            {
                if (matrizExtendida[i] == null) matrizExtendida[i] = "";
                matrizExtendida[i] = matrizExtendida[i].PadLeft(matrizExtendida.Length, '-');
            }
            return matrizExtendida;
        }

        public static string ArrayToString(string[] ADN)
        {
            string adn = string.Join(",", ADN);

            return adn;
        }

        internal IStats RatioMutantes()
        {
            List<Gen> listGenes = Gen.LstGenes(_cn);
            int cantidadHumanos = listGenes.FindAll(p => p.EsMutante == false).Count;
            int cantidadMutantes = listGenes.FindAll(p => p.EsMutante == true).Count;

            decimal ratio = 0;
            if (cantidadHumanos > 0) ratio = ((decimal)cantidadMutantes / (decimal)cantidadHumanos);

            IStats iStats = new IStats()
            {
                CountHumantDNA = cantidadHumanos,
                CountMutantDNA = cantidadMutantes,
                Ratio = ratio
            };

            return iStats;
        }
        internal List<IGenInfo> GetAll()
        {
            List<Gen> listGenes = Gen.LstGenes(_cn);
            List<IGenInfo> listGenesInfo = new List<IGenInfo>();
            foreach(Gen gen in listGenes)
            {
                IGenInfo iGenInfo = new IGenInfo()
                {
                    ID = gen.ID,
                    Dna = gen.ADN.Split(','),
                    EsMutante = gen.EsMutante,
                    FechaAlta = gen.FechaAlta
                };
                listGenesInfo.Add(iGenInfo);
            }
            

            return listGenesInfo;
        }



        internal IGenInfo GetByID(int ID)
        {
            Gen gen = Gen.Get(_cn, ID);
            IGenInfo iGenInfo = null;
            if (gen != null)
            {
                iGenInfo = new IGenInfo()
                {
                    ID = gen.ID,
                    Dna = gen.ADN.Split(','),
                    EsMutante = gen.EsMutante,
                    FechaAlta = gen.FechaAlta
                };
            }


            return iGenInfo;
        }
    }
}

