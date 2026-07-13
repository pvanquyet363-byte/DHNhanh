namespace Deobely_sync.Model
{
    public class AttributeModel
    {
        public string Name { get; set; }
        public string Ma_vt { get; set; }
        public string Size => Ma_vt.Split('-')[Ma_vt.Split('-').Length-1];
        public List<Stock> Stocks { get; set; }
      

    }

    public class Stock
    {
        public string Ma_vt { get; set; }
        public string Ma_kho { get; set; }
        public string Ten_kho { get; set; }
        public int Quantity  { get; set; }
    }
}
