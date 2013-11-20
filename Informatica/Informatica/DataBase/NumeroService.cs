using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Informatica.DataBase {
    public class NumeroService:IDisposable {

        NumeroDbDataContext numeroDb;

        public NumeroService() {
            numeroDb = new NumeroDbDataContext();
        }

        /// <summary>
        /// Genera la cantidad solicitada de números aleatorios en el rango de signed smallint.
        /// Pendiente 
        /// </summary>
        /// <param name="nombre">nombre de usuario</param>
        /// <param name="hash">identificador hash</param>
        /// <param name="cantidad">cantidad de números a generar</param>
        public void GeneraAleatorios(string nombre, string hash, int cantidad) {
            Random rand = new Random();
            do {
                int generar = 0;
                if (cantidad > 10000) {
                    generar = 10000;
                    cantidad -= 10000;
                } else {
                    generar = cantidad;
                    cantidad = 0;
                }
                List<Numero> numeros = new List<Numero>();
                for (int i = 0; i < generar; i++) {
                    numeros.Add(new Numero() {
                        hash_user = hash,
                        name = nombre,
                        num = rand.Next(-32768, 32676)
                    });
                }
                numeroDb.Numeros.InsertAllOnSubmit(numeros);
                numeroDb.SubmitChanges();       //Almacena en listas de 10000
            } while (cantidad > 0);
        }

        public void AlmacenaNumeros(string nombre, string hash, List<int> nums) {
            List<Numero> numeros = new List<Numero>();
            foreach (int num in nums) {
                numeros.Add(new Numero() {
                    hash_user = hash,
                    name = nombre,
                    num = num
                });
            }
            numeroDb.Numeros.InsertAllOnSubmit(numeros);
            numeroDb.SubmitChanges();       //Almacena en listas de 10000
        }

        public void ModificarEstado(string hash, string name, int estado) {
            EstadoUsuario ea = numeroDb.EstadoUsuarios.FirstOrDefault(p => p.hash_user == hash && p.name == name);
            if (ea != null) {
                ea.status = estado;
            } else {
                ea = new EstadoUsuario() {
                    hash_user = hash,
                    name = name,
                    status = estado
                };
                numeroDb.EstadoUsuarios.InsertOnSubmit(ea);
            }
            numeroDb.SubmitChanges();
        }

        public EstadoUsuario VerificarEstado(string hash, string name) {
            return numeroDb.EstadoUsuarios.FirstOrDefault(p => p.hash_user == hash && p.name == name);
        }

        public int ObtenerMayorAlmacenado(string hash, string name) { 
            int max=(from num in numeroDb.Numeros 
                         where num.hash_user==hash && num.name==name
                         group num by new {num.hash_user,num.name} into order
                         select order.Max(p=>p.num)).SingleOrDefault<int>();
            return max;
        }

        public void LimpiarListaUsuario(string hash, string name) { 
            List<Numero> numeros= numeroDb.Numeros.Where(p => p.hash_user == hash && p.name == name).ToList();
            numeroDb.Numeros.DeleteAllOnSubmit(numeros);
            numeroDb.SubmitChanges();
        }

        #region IDisposable Members

        public void Dispose() {
            numeroDb.Dispose();
        }

        #endregion
    }
}