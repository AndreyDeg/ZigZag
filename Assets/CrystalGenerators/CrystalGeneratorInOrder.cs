public class CrystalGeneratorInOrder : ICrystalGenerator
{
    int blockLen = 5;

    public CrystalGeneratorInOrder(int blockLen)
    {
        this.blockLen = blockLen;
    }

    public bool Check(int n)
    {
        int blockNum = n / blockLen;
        return n % blockLen == blockNum % blockLen;
    }
}
