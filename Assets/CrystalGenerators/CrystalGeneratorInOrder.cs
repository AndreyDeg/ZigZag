//Расстановка кристалов по порядку, в каждом блоке по одному
public class CrystalGeneratorInOrder : ICrystalGenerator
{
    int blockLen = 5; //длинна блока

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
