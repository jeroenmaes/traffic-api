using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traffic.Shared
{
    public class Animation
    {
        public string localisationLayer { get; set; }
        public double localisationLayerRatioX { get; set; }
        public double localisationLayerRatioY { get; set; }
        public double speed { get; set; }
        public string type { get; set; }
        public Unit unit { get; set; }
        public string country { get; set; }
        public List<Sequence> sequence { get; set; }
        public List<object> threshold { get; set; }
        public SequenceHint sequenceHint { get; set; }
    }

    public class Daily
    {
        public DayName dayName { get; set; }
        public string period { get; set; }
        public string day_night { get; set; }
        public string dayNight { get; set; }
        public Text text { get; set; }
        public string dawnRiseSeconds { get; set; }
        public string dawnSetSeconds { get; set; }
        public int? tempMin { get; set; }
        public int? tempMax { get; set; }
        public int ww1 { get; set; }
        public int? ww2 { get; set; }
        public int? wwevol { get; set; }
        public int ff1 { get; set; }
        public int? ff2 { get; set; }
        public int? ffevol { get; set; }
        public int dd { get; set; }
        public DdText ddText { get; set; }
        public Wind wind { get; set; }
        public int precipChance { get; set; }
        public string precipQuantity { get; set; }
    }

    public class Data
    {
        public Url url { get; set; }
        public double ratio { get; set; }
        public double? levelValue { get; set; }
        public Level level { get; set; }
        public Title title { get; set; }
        public int? count { get; set; }
    }

    public class DateShowLocalized
    {
        public string nl { get; set; }
        public string fr { get; set; }
        public string en { get; set; }
        public string de { get; set; }
    }

    public class DayName
    {
        public string fr { get; set; }
        public string nl { get; set; }
        public string en { get; set; }
        public string de { get; set; }
    }

    public class DdText
    {
        public string fr { get; set; }
        public string nl { get; set; }
        public string en { get; set; }
        public string de { get; set; }
    }

    public class DirText
    {
        public string fr { get; set; }
        public string nl { get; set; }
        public string en { get; set; }
        public string de { get; set; }
    }

    public class For
    {
        public List<Daily> daily { get; set; }
        public bool showWarningTab { get; set; }
        public Graph graph { get; set; }
        public List<Hourly> hourly { get; set; }
        public List<object> warning { get; set; }
    }

    public class Graph
    {
        public List<Svg> svg { get; set; }
    }

    public class Hourly
    {
        public string hour { get; set; }
        public int temp { get; set; }
        public string ww { get; set; }
        public string precipChance { get; set; }
        public double precipQuantity { get; set; }
        public int pressure { get; set; }
        public int windSpeedKm { get; set; }
        public object windPeakSpeedKm { get; set; }
        public int windDirection { get; set; }
        public WindDirectionText windDirectionText { get; set; }
        public string dayNight { get; set; }
        public string dateShow { get; set; }
        public DateShowLocalized dateShowLocalized { get; set; }
    }

    public class Level
    {
        public string nl { get; set; }
        public string fr { get; set; }
        public string en { get; set; }
        public string de { get; set; }
    }

    public class Module
    {
        public string type { get; set; }
        public Data data { get; set; }
    }

    public class Obs
    {
        public int temp { get; set; }
        public DateTime timestamp { get; set; }
        public int ww { get; set; }
        public string dayNight { get; set; }
    }

    public class ForecastDto
    {
        public string cityName { get; set; }
        public string country { get; set; }
        public Obs obs { get; set; }
        public For @for { get; set; }
        public List<Module> module { get; set; }
        public Animation animation { get; set; }
        public int todayObsCount { get; set; }
    }

    public class Sequence
    {
        public DateTime time { get; set; }
        public string uri { get; set; }
        public int value { get; set; }
        public int position { get; set; }
        public int positionLower { get; set; }
        public int positionHigher { get; set; }
    }

    public class SequenceHint
    {
        public string nl { get; set; }
        public string fr { get; set; }
        public string en { get; set; }
        public string de { get; set; }
    }

    public class Svg
    {
        public Url url { get; set; }
        public double ratio { get; set; }
    }

    public class Text
    {
        public string nl { get; set; }
        public string fr { get; set; }
    }

    public class Title
    {
        public string nl { get; set; }
        public string fr { get; set; }
        public string en { get; set; }
        public string de { get; set; }
    }

    public class Unit
    {
        public string fr { get; set; }
        public string nl { get; set; }
        public string en { get; set; }
        public string de { get; set; }
    }

    public class Url
    {
        public string nl { get; set; }
        public string fr { get; set; }
        public string en { get; set; }
        public string de { get; set; }
    }

    public class Wind
    {
        public int speed { get; set; }
        public object peakSpeed { get; set; }
        public int dir { get; set; }
        public DirText dirText { get; set; }
    }

    public class WindDirectionText
    {
        public string nl { get; set; }
        public string fr { get; set; }
        public string en { get; set; }
        public string de { get; set; }
    }
}
