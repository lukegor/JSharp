namespace JSharp.Utility.Utility
{
    public static class ArrayHelper
    {
        // Helper method to check if two 2D arrays are equal
        public static bool AreArraysEqual(int[,] array1, int[,] array2)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (array1[i, j] != array2[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
