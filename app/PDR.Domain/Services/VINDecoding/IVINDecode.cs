namespace PDR.Domain.Services.VINDecoding
{
    public interface IVINDecode
    {
        VINInfo Decode(string vinCode);
    }
}
