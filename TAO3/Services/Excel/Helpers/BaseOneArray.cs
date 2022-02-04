namespace TAO3.Excel;

internal static class BaseOneArray
{
    public static object[,] Create(int rows, int cols)
    {
        int[] lengths = new int[] { rows, cols };
        int[] lowerBounds = new int[] { 1, 1 };

        return (object[,])Array.CreateInstance(typeof(object), lengths, lowerBounds);
    }
}
