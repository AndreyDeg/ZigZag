//Расстановка кристалов случайно, в каждом блоке по одному
public class CrystalGeneratorRandom : ICrystalGenerator
{
    int blockLen = 5;
    int rndN = int.MaxValue;

    public CrystalGeneratorRandom(int blockLen)
    {
        this.blockLen = blockLen;
    }

    public bool Check(int n)
    {
        int blockNum = n / blockLen;
        if (rndN / blockLen != blockNum) // выбран другой блок, сгенерить новое положение
            rndN = blockNum * blockLen + UnityEngine.Random.Range(0, blockLen);
        return rndN == n;
    }
}
