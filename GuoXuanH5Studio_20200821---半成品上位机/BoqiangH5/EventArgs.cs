namespace BoqiangH5
{
    public class EventArgs<T>
    {
        public T Args { get; private set; }
        public EventArgs(T t)
        {
            Args = t;
        }
    }
}