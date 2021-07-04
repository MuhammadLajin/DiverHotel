using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DiverHotel.Models;

namespace DiverHotel.Controllers
{
    public class ReservationGuestsController : Controller
    {
        private Model1 db = new Model1();

        // GET: ReservationGuests
        public ActionResult Index()
        {
            var reservationGuests = db.ReservationGuests.Include(r => r.MealPlan1).Include(r => r.Type);
            return View(reservationGuests.ToList());
        }

        // GET: ReservationGuests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReservationGuest reservationGuest = db.ReservationGuests.Find(id);
            if (reservationGuest == null)
            {
                return HttpNotFound();
            }
            return View(reservationGuest);
        }

        // GET: ReservationGuests/Create
        public ActionResult Create()
        {
            ViewBag.MealPlan = new SelectList(db.MealPlans, "Id", "Plans");
            ViewBag.RoomType = new SelectList(db.Types, "Id", "type1");
            return View();
        }

        // POST: ReservationGuests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,Country,Adults,Children,RoomType,MealPlan,CheckIn,Checkout")] ReservationGuest reservationGuest)
        {
            if (ModelState.IsValid)
            {
                // GetReservationTotal (check-in date, check-out date, number of guests, room type, Meal plan)
                reservationGuest.TotalPrice = GetReservationTotal(reservationGuest.CheckIn, reservationGuest.Checkout,
                     (int)reservationGuest.RoomType, (int)(reservationGuest.Adults + reservationGuest.Children), (int)reservationGuest.MealPlan);

                db.ReservationGuests.Add(reservationGuest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MealPlan = new SelectList(db.MealPlans, "Id", "Plans", reservationGuest.MealPlan);
            ViewBag.RoomType = new SelectList(db.Types, "Id", "type1", reservationGuest.RoomType);
            return View(reservationGuest);
        }




        private int GetReservationTotal(DateTime check_in, DateTime check_out, int roomTypeUser, int guests = 1, int MealPlan = 0)
        {
            int totalBudget = 0;

            #region Budget of meal
            //use linq to get the salary of the choosen meal
            List<ReservationMealPlan> listPlans = db.ReservationMealPlans.ToList();

            //get number of days of the given period
            int numOfDays = (check_out - check_in).Days;

            //calculate budget meal depend on the low season or high season for each day
            DateTime temp = check_in;
            while (temp <= check_out)
            {
                int budgetMeal = listPlans.Select(x => x).Where(x => x.MealID == MealPlan && temp.Month >= x.StartMonth && temp.Month <= x.EndMonth)
                    .Select(x => x.Price).ToList().SingleOrDefault();

                totalBudget += budgetMeal * numOfDays * guests;

                temp = temp.AddDays(1);
            }


            #endregion

            #region budget of rooms
            temp = check_in;

            // 1 >>list of the rooms
            List<Room> listRooms = db.Rooms.ToList();
            // 1 >> get id of the room available by rateId and TypeID
            int roomId = listRooms.Select(x => x).Where(x => x.RoomTypeID == roomTypeUser && x.Available == "1")
                .Select(x => x.Id).ToList().FirstOrDefault();
            //Console.WriteLine($"Room ID: {roomId}");

            while (temp <= check_out)
            {
                // 2 >> list of the rates
                List<RateSeason> listRate = db.RateSeasons.ToList();
                // 2 >> get the rate id for the room depend on each day
                int rateId = listRate.Select(x => x).Where(x => temp.Month >= x.StartDate.Month && temp.Month <= x.EndDate.Month
                                    && temp.Day >= x.StartDate.Day && temp.Day <= x.EndDate.Day)
                    .Select(x => x.Id).ToList().SingleOrDefault();
                Console.WriteLine($"rate:  {rateId}");


                // 3 >> list of the reservation table
                List<ReservationRoom> listReservationsRooms = db.ReservationRooms.ToList();
                // 3 >> get price of this room depend on the night 
                int budgetRoom = listReservationsRooms.Select(x => x).Where(x => x.IdRoom == roomId && x.IdRate == rateId)
                    .Select(x => x.price).ToList().SingleOrDefault();
                Console.WriteLine($"budget of the room: {budgetRoom}");

                totalBudget += budgetRoom;

                temp = temp.AddDays(1);
            }

            #endregion


            //Console.WriteLine();
            //Console.WriteLine($"check_int is    : { check_in}");
            //Console.WriteLine($"check_out is    : { check_out}");
            //Console.WriteLine($"guests is       : { guests}");
            //Console.WriteLine($"numOfDays is    : { numOfDays}");
            //Console.WriteLine($"totalBudget is  : { totalBudget}");

            return totalBudget;
        }



        // GET: ReservationGuests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReservationGuest reservationGuest = db.ReservationGuests.Find(id);
            if (reservationGuest == null)
            {
                return HttpNotFound();
            }
            ViewBag.MealPlan = new SelectList(db.MealPlans, "Id", "Plans", reservationGuest.MealPlan);
            ViewBag.RoomType = new SelectList(db.Types, "Id", "type1", reservationGuest.RoomType);
            return View(reservationGuest);
        }

        // POST: ReservationGuests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Country,Adults,Children,RoomType,MealPlan,CheckIn,Checkout,TotalPrice")] ReservationGuest reservationGuest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservationGuest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MealPlan = new SelectList(db.MealPlans, "Id", "Plans", reservationGuest.MealPlan);
            ViewBag.RoomType = new SelectList(db.Types, "Id", "type1", reservationGuest.RoomType);
            return View(reservationGuest);
        }

        // GET: ReservationGuests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReservationGuest reservationGuest = db.ReservationGuests.Find(id);
            if (reservationGuest == null)
            {
                return HttpNotFound();
            }
            return View(reservationGuest);
        }

        // POST: ReservationGuests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReservationGuest reservationGuest = db.ReservationGuests.Find(id);
            db.ReservationGuests.Remove(reservationGuest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
