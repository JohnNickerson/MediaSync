using System.IO;
public static class IntHelpers
{
    public static ulong RotateLeft(this ulong original, int bits)
    {
        return (original << bits) | (original >> (64 - bits));
    }

    public static ulong RotateRight(this ulong original, int bits)
    {
        return (original >> bits) | (original << (64 - bits));
    }

    unsafe public static ulong GetUInt64(this byte[] bb, int pos)
    {
        // we only read aligned longs, so a simple casting is enough
        fixed (byte* pbyte = &bb[pos])
        {
            return *((ulong*)pbyte);
        }
    }

    public static ulong GetUInt64(this FileStream file)
    {
        byte[] b = new byte[8];
        for (int x = 0; x < 8; x++)
        {
            b[x] = (byte)file.ReadByte();
        }
        return b.GetUInt64(0);
    }
}