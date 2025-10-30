namespace Shared
{
    public static class SkipCalculator
    {
        public static int Calculate(int pageIndex, int pageSize) => pageIndex < 0 ? 0 : pageIndex * pageSize;
    }
}
