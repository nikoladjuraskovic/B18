using B18.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace B18.Controllers
{
    public class MaksimalneController : Controller
    {


        private PrognozaDBContext db = new PrognozaDBContext();


        // GET: Maksimalne
        public ActionResult Index()
        {

            /*Ovu stranicu postaviti obavezno kao pocetnu u projektu na sledeci nacin:
             https://social.msdn.microsoft.com/Forums/en-US/54f183ae-cf3b-439c-84d0-603e2be5a100/how-to-set-startup-page-with-area-in-aspnet-mvc-4?forum=aspmvc 
            
             */


            string fileName = Server.MapPath("~/App_Data/prognoza.xml");

            foreach(var entity in db.Prognoze)
            {
                db.Prognoze.Remove(entity);
            }
            /*Resetujemo brojanje primarnog kljuca da bi se id brojao od 1
             https://stackoverflow.com/questions/52937700/reset-id-column-of-database-records
            https://www.entityframeworktutorial.net/EntityFramework4.3/raw-sql-query-in-entity-framework.aspx
            https://learn.microsoft.com/en-us/ef/ef6/querying/raw-sql
             
             */
            db.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('dbo.Prognozas', RESEED, 0)");

            db.SaveChanges();


            using (XmlReader reader = XmlReader.Create(fileName))
            {


                string Mesto = "";
                string nazivMesta = "";
                int minTemp = 0;
                int maxTemp = 0;
                string Vreme = "";

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Mesto")
                                Mesto = reader.ReadElementContentAsString().Trim();
                            else if (reader.Name == "NazivMesta")
                                nazivMesta = reader.ReadElementContentAsString().Trim();
                            else if (reader.Name == "MinTemperatura")
                                minTemp = int.Parse(reader.ReadElementContentAsString().Trim());
                            else if (reader.Name == "MaxTemperatura")
                                maxTemp = int.Parse(reader.ReadElementContentAsString().Trim());
                            else if (reader.Name == "Vreme")
                                Vreme = reader.ReadElementContentAsString().Trim();

                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "Prognoza")
                            {
                                Prognoza prognoza = new Prognoza();
                                prognoza.Mesto = Mesto;
                                prognoza.NazivMesta = nazivMesta;
                                prognoza.MinTemperatura = minTemp;
                                prognoza.MaxTemperatura = maxTemp;
                                prognoza.Vreme = Vreme;

                                db.Prognoze.Add(prognoza);
                            }

                            break;
                        default:

                            break;
                    }
                }


            }

            db.SaveChanges();

            return View(db.Prognoze.ToList());
        }


        //GET: Maksimalne/Details/id
        public ActionResult Details(int id = 2)
        {
            /*Ako korisnik ne klikne na ime grada nego odmah ode na ovu stranicu,
             postaviti da default parametar bude 2*/

            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                     
             Prognoza prognoza = db.Prognoze.Find(id);

            if (prognoza == null)
            {
                return HttpNotFound();
            }

            /*Za ispis u DropDownList iz baze
             https://dotnettutorials.net/lesson/dropdownlist-html-helper-mvc/

            DropDownList: https://learn.microsoft.com/en-us/dotnet/api/system.web.mvc.html.selectextensions.dropdownlist?view=aspnet-mvc-5.2
             */


            List<SelectListItem> items = new List<SelectListItem>();
            //Prognoza selektovanaPrognoza = db.Prognoze.Find(id);
            items.Add(new SelectListItem { Text = prognoza.NazivMesta, Value = prognoza.Id.ToString() });

            //prvo ubacujemo selektovano ime grada

            foreach(var entity in db.Prognoze)
            {
                if(entity.Id != id) // da ne bismo ubacivali 2 puta isti grad
                    items.Add(new SelectListItem{ Text = entity.NazivMesta, Value = entity.Id.ToString()});
            }

            ViewBag.ImenaGradova = items;

            return View(prognoza);
        }


        //POST:  Maksimalne/Details
        [HttpPost]       
        public ActionResult Details()
        {
           //string ime_selektovanog_grada = Request.Form["imena_gradova"].Trim();
           /*Dohvata se id grada jer smo u gornjem metodu rekli Value = id, a naziv je samo tekst*/
          int id_selektovanog_grada = int.Parse(Request.Form["imena_gradova"].Trim());

            //System.Diagnostics.Debug.WriteLine("Dohvacen grad: " + id_selektovanog_grada);

            Prognoza trazenaPrognoza = db.Prognoze.Find(id_selektovanog_grada); // moramo dodeliti nesto
            /*
            //trazimo grad sa datim imenom
           foreach(var entity in db.Prognoze)
            {
                if(entity.Id == ime_selektovanog_grada)
                {
                    trazenaPrognoza = entity;
                    break;
                }
            }
            */
            int id = trazenaPrognoza.Id;
            List<SelectListItem> items = new List<SelectListItem>();
            //Prognoza selektovanaPrognoza = db.Prognoze.Find(id);
            items.Add(new SelectListItem { Text = trazenaPrognoza.NazivMesta, Value = trazenaPrognoza.Id.ToString() });

            //prvo ubacujemo selektovano ime grada

            foreach (var entity in db.Prognoze)
            {
                if (entity.Id != id) // da ne bismo ubacivali 2 puta isti grad
                    items.Add(new SelectListItem { Text = entity.NazivMesta, Value = entity.Id.ToString() });
            }

            ViewBag.ImenaGradova = items;

            return View(trazenaPrognoza);
            
        }
    }
}