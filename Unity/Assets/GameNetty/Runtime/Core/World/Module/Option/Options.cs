namespace ET
{
    public class Options
    {
        private static Options _instance;

        public static Options Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Options();
                }
                return _instance;
            }
        }
        
        public int Process { get; set; } = 1;
    }
}