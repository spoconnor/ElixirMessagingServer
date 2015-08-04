object Misc
{
    def GetEmptyArray<T>(width: Int, height: Int): T[][] =
    {
        var image = new T[width][]
        for (int i = 0; i < width; i++)
        {
            image[i] = new T[height]
        }
        return image
    }
}

