namespace API_Adaption.Model
{
    public class ResponseModel
    {
        public int status {  get; set; }  
        public string mes {  get; set; } 
        public ResponseModel() { }
        public ResponseModel(int _status,string _mes) { 
            status = _status;
            mes = _mes;
        }
    }
}
