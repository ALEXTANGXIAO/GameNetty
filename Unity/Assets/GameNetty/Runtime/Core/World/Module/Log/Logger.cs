namespace ET
{
    public class Logger: Singleton<Logger>, ISingletonAwake
    {
        private ILog log;

        public ILog Log
        {
            set => log = value;
            get => log;
        }
        
        public void Awake()
        {
        }
    }
}