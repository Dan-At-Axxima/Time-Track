using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace TimeTrackerRepo.Models
{
    [MetadataType(typeof(FullMonth))]
    public class FullMonth
    {
        public string Client { get; set; }
        public string Project { get; set; }
        public string Activity { get; set; }
        [MyAttribute("1")]
        public string DayOne { get; set; }
        [MyAttribute("2")]
        public string DayTwo { get; set; }
        [MyAttribute("3")]
        public string DayThree { get; set; }
        [MyAttribute("4")]
        public string DayFour { get; set; }
        [MyAttribute("5")]
        public string DayFive { get; set; }
        [MyAttribute("6")]
        public string DaySix { get; set; }
        [MyAttribute("7")]
        public string DaySeven { get; set; }
        [MyAttribute("8")]
        public string DayEight { get; set; }
        [MyAttribute("9")]
        public string DayNine { get; set; }
        [MyAttribute("10")]
        public string DayTen { get; set; }
        [MyAttribute("11")]
        public string DayEleven { get; set; }
        [MyAttribute("12")]
        public string DayTwelve { get; set; }
        [MyAttribute("13")]
        public string DayThirteen { get; set; }
        [MyAttribute("14")]
        public string DayFourTeen { get; set; }
        [MyAttribute("15")]
        public string DayFifteen { get; set; }
        [MyAttribute("16")]
        public string DaySixteen { get; set; }
        [MyAttribute("17")]
        public string DaySeventeen { get; set; }
        [MyAttribute("18")]
        public string DayEighteen { get; set; }
        [MyAttribute("19")]
        public string DayNineteen { get; set; }
        [MyAttribute("20")]
        public string DayTwenty { get; set; }
        [MyAttribute("21")]
        public string DayTwentyOne { get; set; }
        [MyAttribute("22")]
        public string DayTwentyTwo { get; set; }
        [MyAttribute("23")]
        public string DayTwentyThree { get; set; }
        [MyAttribute("24")]
        public string DayTwentyFour { get; set; }
        [MyAttribute("25")]
        public string DayTwentyFive { get; set; }
        [MyAttribute("26")]
        public string DayTwentySix { get; set; }
        [MyAttribute("27")]
        public string DayTwentySeven { get; set; }
        [MyAttribute("28")]
        public string DayTwentyEight { get; set; }
        [MyAttribute("29")]
        public string DayTwentyNine { get; set; }
        [MyAttribute("30")]
        public string DayThirty { get; set; }
        [MyAttribute("31")]
        public string DayThirtyOne { get; set; }
    }
    
}
