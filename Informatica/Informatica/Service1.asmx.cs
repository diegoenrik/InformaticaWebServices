using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Informatica.DataBase;
using System.Security.Cryptography;
using System.Text;

namespace Informatica {
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://informatica.lynxsurver.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService {

        [WebMethod]
        public int ObtenerMayorArray(string nombre_usuario, int[] numeros) {
            if (numeros == null || numeros.Length == 0) {
                return 0;
            }
            if (numeros.Length > 10000) {
                numeros = numeros.Take(10000).ToArray();
            }
            return numeros.Max();
        }

        [WebMethod]
        public int ObtenerMayorLista(string nombre_usuario, List<int> numeros) {
            if (numeros == null || numeros.Count == 0) {
                return 0;
            }
            if (numeros.Count > 10000) {
                numeros = numeros.Take(10000).ToList();
            }
            return numeros.Max();
        }

        [WebMethod]
        public int GenerarNumeros(string nombre_usuario, int cantidad) {
            string ip = HttpContext.Current.Request.UserHostAddress;
            string browser = HttpContext.Current.Request.UserAgent;
            NumeroService numeroServices = new NumeroService();
            string hash=CalculateMD5Hash(ip + browser + nombre_usuario);
            numeroServices.ModificarEstado(hash, nombre_usuario, 1);        //procesando
            numeroServices.GeneraAleatorios(nombre_usuario, hash, cantidad);
            numeroServices.ModificarEstado(hash, nombre_usuario, 0);        //libre
            return cantidad;
        }

        [WebMethod]
        public int AlmacenarNumeros(string nombre_usuario, int[] numeros) {
            if (numeros == null || numeros.Length == 0) {
                return 0;
            }
            if (numeros.Length > 10000) {
                numeros = numeros.Take(10000).ToArray();
            }
            string ip = HttpContext.Current.Request.UserHostAddress;
            string browser = HttpContext.Current.Request.UserAgent;
            string hash = CalculateMD5Hash(ip + browser + nombre_usuario);
            NumeroService numeroServices = new NumeroService();
            numeroServices.AlmacenaNumeros(nombre_usuario, hash, numeros.ToList());
            return numeros.Length;
        }

        [WebMethod]
        public string ObtenerMayor(string nombre_usuario) {
            string ip = HttpContext.Current.Request.UserHostAddress;
            string browser = HttpContext.Current.Request.UserAgent;
            string hash = CalculateMD5Hash(ip + browser + nombre_usuario);
            NumeroService numeroServices = new NumeroService();
            EstadoUsuario ea = numeroServices.VerificarEstado(hash, nombre_usuario);
            if (ea != null && ea.status == 1) {     //procesando
                return "PROCESANDO";
            }
            int mayor = numeroServices.ObtenerMayorAlmacenado(hash, nombre_usuario);
            return mayor.ToString();
        }

        [WebMethod]
        public string LimpiarLista(string nombre_usuario) {
            string ip = HttpContext.Current.Request.UserHostAddress;
            string browser = HttpContext.Current.Request.UserAgent;
            string hash = CalculateMD5Hash(ip + browser + nombre_usuario);
            NumeroService numeroServices = new NumeroService();
            numeroServices.LimpiarListaUsuario(hash, nombre_usuario);
            return "OK";
        }

        private string CalculateMD5Hash(string input) {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

    }
}