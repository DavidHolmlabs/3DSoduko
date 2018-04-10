using System;

namespace Soduko
{
    public static class IndexExtensions
    {
        public static void VerifyIndex(this int x)
        {
            if (x < 0 || x > 8)
                throw new IndexOutOfRangeException("Index must be between 0 and 8");
        }

        public static void VerifySmallIndex(this int x)
        {
            if (x != 0 && x != 1 && x != 2)
                throw new IndexOutOfRangeException("Index must be between 0 and 2");
        }
    }
}
