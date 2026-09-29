using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BigViewer.Core
{
    public static class Common
    {
        public static string GetTypeName(uint type)
        {
            return type switch
            {
                Constants.resourceTypeData => "data",
                Constants.resourceTypeString => "string",
                Constants.resourceTypePng => "png",
                Constants.resourceTypeWav => "wav",
                Constants.resourceTypeKeyset => "keyset",
                Constants.resourceTypeAgg => "agg",
                _ => type.ToString("X8"),
            };
        }

        public static string GetFormatName(uint format)
        {
            return format switch
            {
                1 => "none",
                2 => "zlib",
                _ => "unknown",
            };
        }

        public static (uint, byte[]) DecodeResource(byte[] data)
        {
            if (data.Length >= 4)
            {
                ReadOnlySpan<byte> dataSpan = data.AsSpan();
                ref byte bas = ref MemoryMarshal.GetReference(dataSpan);
                uint fst = Unsafe.ReadUnaligned<uint>(ref bas);
                if (fst == 0x4)
                {
                    return (1, data[0x4..]);
                }
                else if (fst == 0x800004 && dataSpan.Length >= 0xE && Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref bas, 0x8)) == dataSpan.Length - 0xC && Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref bas, 0xC)) == 0xDA78)
                {
                    int decomSize = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref bas, 0x4));
                    using (MemoryStream input = new MemoryStream(data[0xC..]))
                    using (ZLibStream zs = new ZLibStream(input, CompressionMode.Decompress))
                    using (MemoryStream output = new MemoryStream(decomSize))
                    {
                        zs.CopyTo(output);
                        if (output.Position != decomSize)
                        {
                            throw new InvalidDataException($"Decompressed size mismatch! Expected {decomSize}, got {output.Position}");
                        }
                        return (2, output.ToArray());
                    }
                }
                else
                {
                    throw new InvalidDataException("Unknown format!");
                }
            }
            else
            {
                throw new InvalidDataException("Invalid data!");
            }
        }

        public static byte[] EncodeResource(byte[] rawData, uint format)
        {
            switch (format)
            {
                case 1:
                    // none
                    return [0x04, 0x00, 0x00, 0x00, ..rawData];
                case 2:
                    // zlib
                    using (MemoryStream result = new MemoryStream(rawData.Length))
                    {
                        result.Write([0x04, 0x00, 0x80, 0x00]);
                        result.Write(BitConverter.GetBytes(rawData.Length));
                        result.Write([0x00, 0x00, 0x00, 0x00]);
                        using (ZLibStream zs = new ZLibStream(result, CompressionLevel.SmallestSize, true))
                        {
                            zs.Write(rawData.AsSpan());
                        }
                        int numBytesWritten = (int)result.Position - 0xC;
                        result.Seek(0x8, SeekOrigin.Begin);
                        result.Write(BitConverter.GetBytes(numBytesWritten));
                        return result.ToArray();
                    }
                default:
                    throw new ArgumentException($"Unknown format: {format}!");
            }
        }

        public static int[] FindSequence(List<byte> data, byte[] pattern)
        {
            List<int> matchesList = [];
            if (pattern.Length <= data.Count)
            {
                bool allSame;
                for (int i = 0; i <= data.Count - pattern.Length; i++)
                {
                    allSame = true;
                    for (int j = 0; j < pattern.Length; j++)
                    {
                        if (data[i + j] != pattern[j])
                        {
                            allSame = false;
                            break;
                        }
                    }
                    if (allSame)
                    {
                        matchesList.Add(i);
                    }
                }
            }
            return matchesList.ToArray();
        }

        public static int[] FindSequence(ReadOnlySpan<byte> data, byte[] pattern)
        {
            List<int> matchesList = [];
            if (pattern.Length <= data.Length)
            {
                bool allSame;
                for (int i = 0; i <= data.Length - pattern.Length; i++)
                {
                    allSame = true;
                    for (int j = 0; j < pattern.Length; j++)
                    {
                        if (data[i + j] != pattern[j])
                        {
                            allSame = false;
                            break;
                        }
                    }
                    if (allSame)
                    {
                        matchesList.Add(i);
                    }
                }
            }
            return matchesList.ToArray();
        }

        public static int GetSmallestFifteenBits(int id)
        {
            return id & 0x7FFF;
        }

        public static int GetSmallestFifteenBits(uint id)
        {
            return (int)(id & 0x7FFF);
        }

        public static uint GetHash(string str)
        {
            uint num = (uint)str.Length;
            foreach (char c in str)
            {
                num = BitOperations.RotateLeft(num, 4) ^ c;
            }
            return num;
        }

        public static bool IsEligibleForDict(byte[] data)
        {
            if (data.Length >= 0xC)
            {
                ref byte startPos = ref MemoryMarshal.GetArrayDataReference(data);
                uint count = Unsafe.ReadUnaligned<uint>(ref startPos);
                if (data.Length == (count * 8) + 4)
                {
                    uint[] keys = new uint[count];
                    for (int i = 0; i < count; i++)
                    {
                        keys[i] = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref startPos, (i * 8) + 4));
                    }
                    if (keys.Contains(0xAC12AEBCu) && keys.Contains(0xD8A9DAA5u) && keys.Contains(0x02719514u))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsEligibleForDict(ReadOnlySpan<byte> data)
        {
            if (data.Length >= 0xC)
            {
                ref byte startPos = ref MemoryMarshal.GetReference(data);
                uint count = Unsafe.ReadUnaligned<uint>(ref startPos);
                if (data.Length == (count * 8) + 4)
                {
                    uint[] keys = new uint[count];
                    for (int i = 0; i < count; i++)
                    {
                        keys[i] = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref startPos, (i * 8) + 4));
                    }
                    if (keys.Contains(Constants.dictKeyInitData) && keys.Contains(Constants.dictKeyGameTocKeyset) && keys.Contains(Constants.dictKeyObjectScriptCounts))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}