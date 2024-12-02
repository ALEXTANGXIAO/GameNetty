namespace ET
{
    public static class ByteHelper
    {
        public static void WriteTo(this byte[] bytes, int offset, ActorId num)
        {
            bytes.WriteTo(offset, num.Process);
            bytes.WriteTo(offset + 4, num.Fiber);
            bytes.WriteTo(offset + 8, num.InstanceId);
        }
    }
}