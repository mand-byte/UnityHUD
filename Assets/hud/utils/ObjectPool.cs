using System.Collections.Generic;

namespace GameHUD
{
    public static class ObjectPool<T> where T : class, new()
    {
        static Queue<T> list = new Queue<T>();
        public static T Pop()
        {
            if (list.Count == 0)
            {
                return System.Activator.CreateInstance<T>();
            }
            else
            {
                return list.Dequeue();
            }

        }
        public static void Push( T t)
        {
            list.Enqueue(t);
        }
    }
}
