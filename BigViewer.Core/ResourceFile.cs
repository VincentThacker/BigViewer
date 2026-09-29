using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BigViewer.Core
{
    public class ResourceFile
    {
        private struct RLEEntry
        {
            public int startingVirtualId;
            public ushort count;
            public ushort startingId;
            public RLEEntry(int _startingVirtualId, ushort _count, ushort _startingId)
            {
                if (_startingVirtualId >= 0 && _count > 0)
                {
                    startingVirtualId = _startingVirtualId;
                    count = _count;
                    startingId = _startingId;
                }
                else
                {
                    throw new ArgumentException("Invalid RLE entry!");
                }
            }
        }

        private class Resource
        {
            private uint type;
            public uint Type
            {
                get
                {
                    return type;
                }
                set
                {
                    type = value;
                    TypeName = Common.GetTypeName(type);
                }
            }
            public string TypeName { get; private set; }
            private uint format;
            public uint Format
            {
                get
                {
                    return format;
                }
                set
                {
                    format = value;
                    FormatName = Common.GetFormatName(format);
                    data = Common.EncodeResource(rawData, format);
                }
            }
            public string FormatName { get; private set; }
            private byte[] data;
            private byte[] rawData;

            public Resource(uint _type, byte[] _data)
            {
                type = _type;
                data = _data;
                TypeName = Common.GetTypeName(type);
                (format, rawData) = Common.DecodeResource(data);
                FormatName = Common.GetFormatName(format);
            }

            public Resource(uint _type, uint _format, byte[] _rawData)
            {
                type = _type;
                format = _format;
                rawData = _rawData;
                TypeName = Common.GetTypeName(type);
                FormatName = Common.GetFormatName(format);
                data = Common.EncodeResource(rawData, format);
            }

            public int Size()
            {
                return data.Length;
            }

            public int RawSize()
            {
                return rawData.Length;
            }

            public ReadOnlySpan<byte> Data()
            {
                return new ReadOnlySpan<byte>(data);
            }

            public ReadOnlySpan<byte> RawData()
            {
                return new ReadOnlySpan<byte>(rawData);
            }

            public void ReplaceRawData(byte[] newRawData)
            {
                rawData = newRawData;
                data = Common.EncodeResource(rawData, format);
            }
        }

        private class AggResource
        {
            public ushort Identifier { get; private set; }
            public bool Flag1 { get; private set; }
            public bool Flag2 { get; private set; }
            public bool Flag3 { get; private set; }
            public ushort ResourceCount { get; private set; }
            public int Size { get; private set; }
            private List<Resource> resources = [];
            private List<int> offsets = [];
            private List<int> virtualIds = [];

            public AggResource(ReadOnlySpan<byte> totalDataSpan)
            {
                ref byte startPos = ref MemoryMarshal.GetReference(totalDataSpan);
                if (totalDataSpan.Length >= 0x10)
                {
                    Identifier = Unsafe.ReadUnaligned<ushort>(ref startPos);
                    Flag1 = (Identifier & 0x8000) != 0;
                    Flag2 = (Identifier & 0x4000) != 0;
                    Flag3 = (Identifier & 0x2000) != 0;
                    ResourceCount = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref startPos, 0x2));
                    if (ResourceCount == 0)
                    {
                        throw new InvalidDataException("Invalid header");
                    }
                    if (Flag1)
                    {
                        int startingVirtualId = Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref startPos, 0x4));
                        if (Flag2)
                        {
                            for (int i = 0; i < ResourceCount; i++)
                            {
                                virtualIds.Add(startingVirtualId + i);
                                offsets.Add(Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, 0x6 + (i * 4))));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ResourceCount; i++)
                            {
                                virtualIds.Add(startingVirtualId + i);
                                offsets.Add(Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref startPos, 0x6 + (i * 2))));
                            }
                        }
                    }
                    else
                    {
                        if (Flag2)
                        {
                            for (int i = 0; i < ResourceCount; i++)
                            {
                                int pos = 0x4 + (i * 6);
                                virtualIds.Add(Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref startPos, pos)));
                                offsets.Add(Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, pos + 2)));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ResourceCount; i++)
                            {
                                int pos = 0x4 + (i * 4);
                                virtualIds.Add(Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref startPos, pos)));
                                offsets.Add(Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref startPos, pos + 2)));
                            }
                        }
                        if (virtualIds.Distinct().Count() != ResourceCount)
                        {
                            throw new InvalidDataException("Duplicate virtual IDs");
                        }
                    }
                    int offsetEnd = (ResourceCount * (Flag2 ? 0x4 : 0x2)) + (Flag1 ? 0x6 : (ResourceCount * 0x2) + 0x4);
                    Size = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, offsetEnd));
                    offsets.Add(Size);
                    if (Size != totalDataSpan.Length)
                    {
                        throw new InvalidDataException("TOC end mismatch!");
                    }
                    int typeStart = offsetEnd + 0x4;
                    if (typeStart + (Flag3 ? ResourceCount * 0x4 : 0x0) != offsets[0])
                    {
                        throw new InvalidDataException("TOC first offset and content start mismatch!");
                    }
                    for (int i = 0; i < ResourceCount; i++)
                    {
                        if (offsets[i + 1] - offsets[i] < 4)
                        {
                            throw new InvalidDataException("TOC offsets invalid!");
                        }
                    }
                    if (Flag3)
                    {
                        for (int i = 0; i < ResourceCount; i++)
                        {
                            resources.Add(new Resource(
                                _type: Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref startPos, typeStart + (i * 4))),
                                _data: totalDataSpan[offsets[i]..offsets[i + 1]].ToArray()
                            ));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ResourceCount; i++)
                        {
                            resources.Add(new Resource(
                                _type: 0,
                                _data: totalDataSpan[offsets[i]..offsets[i + 1]].ToArray()
                            ));
                        }
                    }
                    offsets.RemoveAt(ResourceCount);
                }
                else
                {
                    throw new InvalidDataException("File is too short!");
                }
            }

            public AggResource(Stream input)
            {
                if (input.CanRead)
                {
                    using (BinaryReader reader = new BinaryReader(input, System.Text.Encoding.UTF8, true))
                    {
                        long start = reader.BaseStream.Position;
                        Identifier = reader.ReadUInt16();
                        Flag1 = (Identifier & 0x8000) != 0;
                        Flag2 = (Identifier & 0x4000) != 0;
                        Flag3 = (Identifier & 0x2000) != 0;
                        ResourceCount = reader.ReadUInt16();
                        if (ResourceCount == 0)
                        {
                            throw new InvalidDataException("Invalid header");
                        }
                        if (Flag1)
                        {
                            int startingVirtualId = reader.ReadUInt16();
                            if (Flag2)
                            {
                                for (int i = 0; i < ResourceCount; i++)
                                {
                                    virtualIds.Add(startingVirtualId + i);
                                    offsets.Add(reader.ReadInt32());
                                }
                            }
                            else
                            {
                                for (int i = 0; i < ResourceCount; i++)
                                {
                                    virtualIds.Add(startingVirtualId + i);
                                    offsets.Add(reader.ReadUInt16());
                                }
                            }
                        }
                        else
                        {
                            if (Flag2)
                            {
                                for (int i = 0; i < ResourceCount; i++)
                                {
                                    int pos = 0x4 + (i * 6);
                                    virtualIds.Add(reader.ReadUInt16());
                                    offsets.Add(reader.ReadInt32());
                                }
                            }
                            else
                            {
                                for (int i = 0; i < ResourceCount; i++)
                                {
                                    int pos = 0x4 + (i * 4);
                                    virtualIds.Add(reader.ReadUInt16());
                                    offsets.Add(reader.ReadUInt16());
                                }
                            }
                            if (virtualIds.Distinct().Count() != ResourceCount)
                            {
                                throw new InvalidDataException("Duplicate virtual IDs");
                            }
                        }
                        Size = reader.ReadInt32();
                        offsets.Add(Size);
                        // Check offsets
                        if ((Flag1 ? 0x6 : (ResourceCount * 0x2) + 0x4) + (ResourceCount * (Flag2 ? 0x4 : 0x2)) + (Flag3 ? ResourceCount * 0x4 : 0x0) + 0x4 != offsets[0])
                        {
                            throw new InvalidDataException("TOC first offset and content start mismatch!");
                        }
                        for (int i = 0; i < ResourceCount; i++)
                        {
                            if (offsets[i + 1] - offsets[i] < 4)
                            {
                                throw new InvalidDataException("TOC offsets invalid!");
                            }
                        }
                        // Read type table (if present) and resources
                        if (Flag3)
                        {
                            uint[] typeList = new uint[ResourceCount];
                            for (int i = 0; i < ResourceCount; i++)
                            {
                                typeList[i] = reader.ReadUInt32();
                            }
                            for (int i = 0; i < ResourceCount; i++)
                            {
                                resources.Add(new Resource(
                                    _type: typeList[i],
                                    _data: reader.ReadBytes(offsets[i + 1] - offsets[i])
                                ));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ResourceCount; i++)
                            {
                                resources.Add(new Resource(
                                    _type: 0,
                                    _data: reader.ReadBytes(offsets[i + 1] - offsets[i])
                                ));
                            }
                        }
                    }
                }
                else
                {
                    throw new InvalidDataException("Input stream is not readable!");
                }
            }

            public uint GetTypes(int id)
            {
                return resources[id].Type;
            }

            public string GetTypeName(int id)
            {
                return resources[id].TypeName;
            }

            public string GetFormatName(int id)
            {
                return resources[id].FormatName;
            }

            public int GetOffset(int id)
            {
                return offsets[id];
            }

            public int GetVirtualId(int id)
            {
                return virtualIds[id];
            }

            // Returns a copy of virtualIds. For performance reasons, minimize the number of calls.
            public int[] GetVirtualIds()
            {
                return virtualIds.ToArray();
            }

            public int GetSize(int id)
            {
                return resources[id].Size();
            }

            public int GetRawSize(int id)
            {
                return resources[id].RawSize();
            }

            public ReadOnlySpan<byte> GetData(int id)
            {
                return resources[id].Data();
            }

            public ReadOnlySpan<byte> GetRawData(int id)
            {
                return resources[id].RawData();
            }

            public byte[] Construct()
            {
                byte[] result = new byte[Size];
                ref byte startPos = ref MemoryMarshal.GetArrayDataReference(result);

                // Construct header
                Unsafe.WriteUnaligned(ref startPos, Identifier);
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, 0x2), ResourceCount);

                if (Flag1)
                {
                    Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, 0x4), (ushort)virtualIds[0]);
                    if (Flag2)
                    {
                        for (int i = 0; i < ResourceCount; i++)
                        {
                            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, 0x6 + (i * 4)), offsets[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ResourceCount; i++)
                        {
                            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, 0x6 + (i * 2)), (ushort)offsets[i]);
                        }
                    }
                }
                else
                {
                    if (Flag2)
                    {
                        for (int i = 0; i < ResourceCount; i++)
                        {
                            int pos = 0x4 + (i * 6);
                            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, pos), (ushort)virtualIds[i]);
                            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, pos + 2), offsets[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ResourceCount; i++)
                        {
                            int pos = 0x4 + (i * 4);
                            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, pos), (ushort)virtualIds[i]);
                            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, pos + 2), (ushort)offsets[i]);
                        }
                    }
                }
                int offsetEnd = (ResourceCount * (Flag2 ? 0x4 : 0x2)) + (Flag1 ? 0x6 : (ResourceCount * 0x2) + 0x4);
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, offsetEnd), Size);
                int typeStart = offsetEnd + 0x4;
                if (Flag3)
                {
                    for (int i = 0; i < ResourceCount; i++)
                    {
                        Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, typeStart + (i * 4)), resources[i].Type);
                    }
                }
                // Construct data
                for (int i = 0; i < ResourceCount; i++)
                {
                    resources[i].Data().CopyTo(result.AsSpan(offsets[i], resources[i].Size()));
                }
                return result;
            }

            public void ConstructAndWrite(Stream target)
            {
                if (target != null && target.CanWrite)
                {
                    using (BinaryWriter writer = new BinaryWriter(target, System.Text.Encoding.UTF8, true))
                    {
                        // Construct header
                        writer.Write(Identifier);
                        writer.Write(ResourceCount);

                        if (Flag1)
                        {
                            writer.Write((ushort)virtualIds[0]);
                            if (Flag2)
                            {
                                for (int i = 0; i < ResourceCount; i++)
                                {
                                    writer.Write(offsets[i]);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < ResourceCount; i++)
                                {
                                    writer.Write((ushort)offsets[i]);
                                }
                            }
                        }
                        else
                        {
                            if (Flag2)
                            {
                                for (int i = 0; i < ResourceCount; i++)
                                {
                                    writer.Write((ushort)virtualIds[i]);
                                    writer.Write(offsets[i]);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < ResourceCount; i++)
                                {
                                    writer.Write((ushort)virtualIds[i]);
                                    writer.Write((ushort)offsets[i]);
                                }
                            }
                        }
                        writer.Write(Size);
                        if (Flag3)
                        {
                            for (int i = 0; i < ResourceCount; i++)
                            {
                                writer.Write(resources[i].Type);
                            }
                        }
                        // Construct data
                        foreach (Resource res in resources)
                        {
                            writer.Write(res.Data());
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Target stream is either null or not writable!");
                }
            }
        }

        private class DictResource
        {
            private OrderedDictionary<uint, int> dict = [];

            public DictResource(ReadOnlySpan<byte> totalDataSpan)
            {
                if (totalDataSpan.Length >= 0x1C)
                {
                    ref byte startPos = ref MemoryMarshal.GetReference(totalDataSpan);
                    uint count = Unsafe.ReadUnaligned<uint>(ref startPos);
                    if (totalDataSpan.Length == (count * 8) + 4)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            int entryPos = (i * 8) + 4;
                            dict.Add(Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref startPos, entryPos)), Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, entryPos + 4)));
                        }
                        if (!(dict.ContainsKey(Constants.dictKeyInitData) && dict.ContainsKey(Constants.dictKeyGameTocKeyset) && dict.ContainsKey(Constants.dictKeyObjectScriptCounts)))
                        {
                            throw new InvalidDataException("Dictionary does not contain required keys!");
                        }
                    }
                    else
                    {
                        throw new InvalidDataException("Invalid length!");
                    }
                }
                else
                {
                    throw new InvalidDataException("Data is too short!");
                }
            }

            public DictResource(Stream input)
            {
                if (input.CanRead)
                {
                    using (BinaryReader reader = new BinaryReader(input, System.Text.Encoding.UTF8, true))
                    {
                        uint count = reader.ReadUInt32();
                        if (count > 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                dict.Add(reader.ReadUInt32(), reader.ReadInt32());
                            }
                            if (!(dict.ContainsKey(Constants.dictKeyInitData) && dict.ContainsKey(Constants.dictKeyGameTocKeyset) && dict.ContainsKey(Constants.dictKeyObjectScriptCounts)))
                            {
                                throw new InvalidDataException("Dictionary does not contain required keys!");
                            }
                        }
                        else
                        {
                            throw new InvalidDataException("Invalid count!");
                        }
                    }
                }
            }

            public int GetValueByKey(uint key)
            {
                return dict[key];
            }

            public uint GetKeyByIndex(int index)
            {
                return dict.GetAt(index).Key;
            }

            public int GetValueByIndex(int index)
            {
                return dict.GetAt(index).Value;
            }

            public int GetCount()
            {
                return dict.Count;
            }

            public int GetTotalSize()
            {
                return (dict.Count * 8) + 4;
            }

            public byte[] Construct()
            {
                byte[] result = new byte[GetTotalSize()];
                ref byte startPos = ref MemoryMarshal.GetArrayDataReference(result);
                Unsafe.WriteUnaligned(ref startPos, dict.Count);
                for (int i = 0; i < dict.Count; i++)
                {
                    int entryPos = (i * 8) + 4;
                    KeyValuePair<uint, int> pair = dict.GetAt(i);
                    Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, entryPos), pair.Key);
                    Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, entryPos + 4), pair.Value);
                }
                return result;
            }

            public void ConstructAndWrite(Stream target)
            {
                if (target != null && target.CanWrite)
                {
                    using (BinaryWriter writer = new BinaryWriter(target, System.Text.Encoding.UTF8, true))
                    {
                        writer.Write(dict.Count);
                        for (int i = 0; i < dict.Count; i++)
                        {
                            KeyValuePair<uint, int> pair = dict.GetAt(i);
                            writer.Write(pair.Key);
                            writer.Write(pair.Value);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Target stream is either null or not writable!");
                }
            }
        }

        private class KeysetResource
        {
            private List<uint> values = [];

            public KeysetResource(ReadOnlySpan<byte> totalDataSpan)
            {
                if (totalDataSpan.Length >= 0x6)
                {
                    ref byte startPos = ref MemoryMarshal.GetReference(totalDataSpan);
                    int count = Unsafe.ReadUnaligned<ushort>(ref startPos);
                    if (totalDataSpan.Length == (count * 4) + 2)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            values.Add(Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref startPos, (i * 4) + 2)));
                        }
                    }
                    else
                    {
                        throw new InvalidDataException("Invalid length!");
                    }
                }
                else
                {
                    throw new InvalidDataException("Data is too short!");
                }
            }

            public KeysetResource(Stream input)
            {
                if (input.CanRead)
                {
                    using (BinaryReader reader = new BinaryReader(input, System.Text.Encoding.UTF8, true))
                    {
                        int count = reader.ReadUInt16();
                        if (count > 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                values.Add(reader.ReadUInt32());
                            }
                        }
                        else
                        {
                            throw new InvalidDataException("Invalid count");
                        }
                    }
                }
            }

            public uint GetValue(int index)
            {
                return values[index];
            }

            // Returns a copy of values. For performance reasons, minimize the number of calls.
            public uint[] GetAllValues()
            {
                return values.ToArray();
            }

            public int GetCount()
            {
                return values.Count;
            }

            public int GetTotalSize()
            {
                return (values.Count * 4) + 2;
            }

            public byte[] Construct()
            {
                byte[] result = new byte[GetTotalSize()];
                ref byte startPos = ref MemoryMarshal.GetArrayDataReference(result);
                Unsafe.WriteUnaligned(ref startPos, (ushort)values.Count);
                for (int i = 0; i < values.Count; i++)
                {
                    Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, (i * 4) + 2), values[i]);
                }
                return result;
            }

            public void ConstructAndWrite(Stream target)
            {
                if (target != null && target.CanWrite)
                {
                    using (BinaryWriter writer = new BinaryWriter(target, System.Text.Encoding.UTF8, true))
                    {
                        writer.Write((ushort)values.Count);
                        for (int i = 0; i < values.Count; i++)
                        {
                            writer.Write(values[i]);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Target stream is either null or not writable!");
                }
            }
        }

        private class ObjectScriptCountsResource
        {
            private List<byte> counts = [];

            public ObjectScriptCountsResource(ReadOnlySpan<byte> totalDataSpan)
            {
                if (totalDataSpan.Length >= 0x2)
                {
                    ref byte startPos = ref MemoryMarshal.GetReference(totalDataSpan);
                    int count = Unsafe.ReadUnaligned<byte>(ref startPos);
                    if (totalDataSpan.Length == count + 1)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            counts.Add(Unsafe.ReadUnaligned<byte>(ref Unsafe.Add(ref startPos, i + 1)));
                        }
                    }
                    else
                    {
                        throw new InvalidDataException("Invalid length!");
                    }
                }
                else
                {
                    throw new InvalidDataException("Data is too short!");
                }
            }

            public ObjectScriptCountsResource(Stream input)
            {
                if (input.CanRead)
                {
                    using (BinaryReader reader = new BinaryReader(input, System.Text.Encoding.UTF8, true))
                    {
                        int count = reader.ReadByte();
                        if (count > 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                counts.Add(reader.ReadByte());
                            }
                        }
                        else
                        {
                            throw new InvalidDataException("Invalid count");
                        }
                    }
                }
            }

            public byte GetCount(int index)
            {
                return counts[index];
            }

            // Returns a copy of counts. For performance reasons, minimize the number of calls.
            public byte[] GetAllCounts()
            {
                return counts.ToArray();
            }

            public int GetCountNumber()
            {
                return counts.Count;
            }

            public int GetTotalSize()
            {
                return counts.Count + 1;
            }

            public byte[] Construct()
            {
                byte[] result = new byte[GetTotalSize()];
                ref byte startPos = ref MemoryMarshal.GetArrayDataReference(result);
                Unsafe.WriteUnaligned(ref startPos, (byte)counts.Count);
                for (int i = 0; i < counts.Count; i++)
                {
                    Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, i + 1), counts[i]);
                }
                return result;
            }

            public void ConstructAndWrite(Stream target)
            {
                if (target != null && target.CanWrite)
                {
                    using (BinaryWriter writer = new BinaryWriter(target, System.Text.Encoding.UTF8, true))
                    {
                        writer.Write((byte)counts.Count);
                        for (int i = 0; i < counts.Count; i++)
                        {
                            writer.Write(counts[i]);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Target stream is either null or not writable!");
                }
            }
        }

        private byte byteFive = 0;
        private byte byteSix = 0;
        private byte byteSeven = 0;
        public int HeaderSize { get; private set; } = 0;
        public int RleCount { get; private set; } = 0;
        public int ResourceCount { get; private set; } = 0;
        public int TableStart { get; private set; } = 0;
        public int ContentStart { get; private set; } = 0;
        public int ContentSize { get; private set; } = 0;

        private byte[] addHeaderBytes = [];
        private List<RLEEntry> rleEntries = [];
        private List<Resource> resources = [];
        private List<int> offsets = [];

        private int dictResId = -1;
        public int DictResId
        {
            get
            {
                return dictResId;
            }
            set
            {
                // If set value is different, attempt to reinitialize dictRes, which checks for validity
                if (value != dictResId)
                {
                    dictRes = new DictResource(resources[value].RawData());
                    dictResId = value;
                }
            }
        }
        private DictResource? dictRes = null;
        public bool IsEnhanced { get; private set; } = false;
        private AggResource? aggResource = null;
        private List<int> keysetResourceIds = [];
        private List<KeysetResource> keysetResources = [];
        public int GameTocKeysetIndexInList { get; private set; } = -1;
        public int ObjectScriptCountsResId { get; private set; } = -1;
        private ObjectScriptCountsResource? objectScriptCountsResource = null;

        public ResourceFile(byte[] totalData)
        {
            ReadOnlySpan<byte> totalDataSpan = new ReadOnlySpan<byte>(totalData);
            ref byte startPos = ref MemoryMarshal.GetReference(totalDataSpan);
            if (totalDataSpan.Length >= 0x3C)
            {
                if (!totalDataSpan[0x0..0x5].SequenceEqual(new byte[] { 0x46, 0x47, 0x49, 0x42, 0x01 }) || (totalDataSpan[0x6] & 0x80) == 0)
                {
                    throw new InvalidDataException("Incorrect starting bytes!");
                }
                byteFive = totalDataSpan[0x5];
                byteSix = totalDataSpan[0x6];
                byteSeven = totalDataSpan[0x7];

                HeaderSize = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, 0x8));
                RleCount = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, 0xC));
                TableStart = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, 0x10));
                ResourceCount = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, 0x14));
                ContentStart = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, 0x18));
                ContentSize = Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, 0x1C));
                if (HeaderSize < 0x20 || RleCount <= 0 || TableStart < 0x28 || ResourceCount <= 0 || ContentStart < 0x38 || ContentSize < 4)
                {
                    throw new InvalidDataException("Invalid header!");
                }
                addHeaderBytes = totalDataSpan[0x20..HeaderSize].ToArray();

                // Check for inconsistencies
                if (ContentStart - TableStart < 0x10)
                {
                    throw new InvalidDataException("TOC must have at least one entry!");
                }
                if (ResourceCount * 8 != (ContentStart - TableStart - 8))
                {
                    throw new InvalidDataException("TOC length and resource count mismatch!");
                }
                if (TableStart - HeaderSize != RleCount * 8)
                {
                    throw new InvalidDataException("Additional header items mismatch!");
                }
                if ((ContentSize + ContentStart) != totalDataSpan.Length)
                {
                    throw new InvalidDataException("Content Size + TOC and file size mismatch!");
                }
                if (Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, ContentStart - 0x8)) != 0)
                {
                    throw new InvalidDataException("TOC end invalid!");
                }
                if (Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, ContentStart - 0x4)) != totalDataSpan.Length)
                {
                    throw new InvalidDataException("TOC end mismatch!");
                }

                // Read and check RLE entries
                for (int i = 0; i < RleCount; i++)
                {
                    int pos = HeaderSize + (i * 8);
                    rleEntries.Add(new RLEEntry(Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, pos)), Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref startPos, pos + 0x4)), Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref startPos, pos + 0x6))));
                }
                if (rleEntries[0].startingId != 0 || rleEntries[^1].startingId + rleEntries[^1].count != ResourceCount)
                {
                    throw new InvalidDataException("Invalid RLE entries!");
                }
                for (int i = 0; i < RleCount - 1; i++)
                {
                    if (rleEntries[i + 1].startingId - rleEntries[i].startingId != rleEntries[i].count)
                    {
                        throw new InvalidDataException("Invalid RLE entries!");
                    }
                }
                for (int i = 0; i < RleCount; i++)
                {
                    int vid = rleEntries[i].startingVirtualId;
                    int minVid = vid + rleEntries[i].count;
                    if (rleEntries.Exists(x => (x.startingVirtualId > vid && x.startingVirtualId < minVid)) || rleEntries.Count(x => (x.startingVirtualId == vid)) != 1)
                    {
                        throw new InvalidDataException("Invalid RLE entries!");
                    }
                }

                // Read offsets from TOC
                for (int i = 0; i <= ResourceCount; i++)
                {
                    int num = TableStart + (i * 8);
                    offsets.Add(Unsafe.ReadUnaligned<int>(ref Unsafe.Add(ref startPos, num + 0x4)));
                }
                if (offsets[0] != ContentStart)
                {
                    throw new InvalidDataException("TOC first offset and content start mismatch!");
                }
                for (int i = 0; i < ResourceCount; i++)
                {
                    // Ensure offsets in TOC increase by at least 4
                    if (offsets[i + 1] - offsets[i] < 4)
                    {
                        throw new InvalidDataException("TOC offsets invalid!");
                    }
                    resources.Add(new Resource(
                        _type: Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref startPos, TableStart + (i * 8))),
                        _data: totalDataSpan[offsets[i]..offsets[i + 1]].ToArray()
                    ));
                }
                offsets.RemoveAt(ResourceCount);
            }
            else
            {
                throw new InvalidDataException("File is too short!");
            }
        }

        public ResourceFile(Stream input)
        {
            if (input.CanRead)
            {
                using (BinaryReader reader = new BinaryReader(input, System.Text.Encoding.UTF8, true))
                {
                    if (!reader.ReadBytes(5).SequenceEqual(new byte[] { 0x46, 0x47, 0x49, 0x42, 0x01 }))
                    {
                        throw new InvalidDataException("Incorrect starting bytes!");
                    }
                    byteFive = reader.ReadByte();
                    byteSix = reader.ReadByte();
                    byteSeven = reader.ReadByte();
                    if ((byteSix & 0x80) == 0)
                    {
                        throw new InvalidDataException("Incorrect starting bytes!");
                    }
                    HeaderSize = reader.ReadInt32();
                    RleCount = reader.ReadInt32();
                    TableStart = reader.ReadInt32();
                    ResourceCount = reader.ReadInt32();
                    ContentStart = reader.ReadInt32();
                    ContentSize = reader.ReadInt32();
                    if (HeaderSize < 0x20 || RleCount <= 0 || TableStart < 0x28 || ResourceCount <= 0 || ContentStart < 0x38 || ContentSize < 4)
                    {
                        throw new InvalidDataException("Invalid header!");
                    }
                    addHeaderBytes = reader.ReadBytes(HeaderSize - 0x20);

                    // Check for inconsistencies (first 3 of 6)
                    if (ContentStart - TableStart < 0x10)
                    {
                        throw new InvalidDataException("TOC must have at least one entry!");
                    }
                    if (ResourceCount * 8 != (ContentStart - TableStart - 8))
                    {
                        throw new InvalidDataException("TOC length and resource count mismatch!");
                    }
                    if (TableStart - HeaderSize != RleCount * 8)
                    {
                        throw new InvalidDataException("Additional header items mismatch!");
                    }

                    // Read and check RLE entries
                    for (int i = 0; i < RleCount; i++)
                    {
                        int pos = HeaderSize + (i * 8);
                        rleEntries.Add(new RLEEntry(reader.ReadInt32(), reader.ReadUInt16(), reader.ReadUInt16()));
                    }
                    if (rleEntries[0].startingId != 0 || rleEntries[^1].startingId + rleEntries[^1].count != ResourceCount)
                    {
                        throw new InvalidDataException("Invalid RLE entries!");
                    }
                    for (int i = 0; i < RleCount - 1; i++)
                    {
                        if (rleEntries[i + 1].startingId - rleEntries[i].startingId != rleEntries[i].count)
                        {
                            throw new InvalidDataException("Invalid RLE entries!");
                        }
                    }
                    for (int i = 0; i < RleCount; i++)
                    {
                        int vid = rleEntries[i].startingVirtualId;
                        int minVid = vid + rleEntries[i].count;
                        if (rleEntries.Exists(x => (x.startingVirtualId > vid && x.startingVirtualId < minVid)) || rleEntries.Count(x => (x.startingVirtualId == vid)) != 1)
                        {
                            throw new InvalidDataException("Invalid RLE entries!");
                        }
                    }

                    // Read TOC
                    uint[] typeList = new uint[ResourceCount + 1];
                    for (int i = 0; i <= ResourceCount; i++)
                    {
                        typeList[i] = reader.ReadUInt32();
                        offsets.Add(reader.ReadInt32());
                    }
                    // Check for inconsistencies (4 and 5)
                    if (offsets[0] != ContentStart)
                    {
                        throw new InvalidDataException("TOC first offset and content start mismatch!");
                    }
                    if (typeList[^1] != 0)
                    {
                        throw new InvalidDataException("TOC end invalid!");
                    }
                    for (int i = 0; i < ResourceCount; i++)
                    {
                        // Ensure offsets in TOC increase by at least 4
                        if (offsets[i + 1] - offsets[i] < 4)
                        {
                            throw new InvalidDataException("TOC offsets invalid!");
                        }
                        resources.Add(new Resource(
                            _type: typeList[i],
                            _data: reader.ReadBytes(offsets[i + 1] - offsets[i])
                        ));
                    }
                    // Check for inconsistencies (6)
                    // The previous checks already guarantee that offsets[0] = ContentStart = HeaderSize + (RleCount * 8) + (ResourceCount * 8) + 8.
                    // Therefore, by definition, the number of bytes read is (HeaderSize + (RleCount * 8) + (ResourceCount * 8) + 8 + (offsets[^1] - offsets[0])) = offsets[^1].
                    // Therefore, unlike the constructor with the total array, there is no independent size value to check; the only variable left to check is ContentSize.
                    if ((ContentSize + ContentStart) != offsets[^1])
                    {
                        throw new InvalidDataException("Content Size + TOC and file size mismatch!");
                    }
                    offsets.RemoveAt(ResourceCount);
                }
            }
            else
            {
                throw new InvalidDataException("Input stream is not readable!");
            }
        }

        public int GetRleEntryStartingVirtualId(int id)
        {
            return rleEntries[id].startingVirtualId;
        }

        public ushort GetRleEntryCount(int id)
        {
            return rleEntries[id].count;
        }

        public ushort GetRleEntryStartingId(int id)
        {
            return rleEntries[id].startingId;
        }

        public uint GetTypes(int id)
        {
            return resources[id].Type;
        }

        public string GetTypeName(int id)
        {
            return resources[id].TypeName;
        }

        public string GetFormatName(int id)
        {
            return resources[id].FormatName;
        }

        public int GetOffset(int id)
        {
            return offsets[id];
        }

        public int GetSize(int id)
        {
            return resources[id].Size();
        }

        public int GetRawSize(int id)
        {
            return resources[id].RawSize();
        }

        public ReadOnlySpan<byte> GetData(int id)
        {
            return resources[id].Data();
        }

        public ReadOnlySpan<byte> GetRawData(int id)
        {
            return resources[id].RawData();
        }

        public int GetTotalSize()
        {
            return checked(ContentStart + ContentSize);
        }

        public int RealToVirtualId(int id)
        {
            RLEEntry ent = rleEntries.Where(x => (x.startingId <= id)).MaxBy(x => x.startingId);
            int displacement = id - ent.startingId;
            return ent.startingVirtualId + displacement;
        }

        public int VirtualToRealId(int input)
        {
            // int virtualId = Common.GetRealId(input);
            RLEEntry ent = rleEntries.Where(x => (x.startingVirtualId <= input)).MaxBy(x => x.startingVirtualId);
            int displacement = input - ent.startingVirtualId;
            if (displacement < ent.count)
            {
                return ent.startingId + displacement;
            }
            else
            {
                return -1;
            }
        }

        public void ReplaceResourceRaw(int id, byte[] newRawData)
        {
            if (id > 0 && id < ResourceCount)
            {
                if (id != dictResId && !keysetResourceIds.Contains(id) && id != ObjectScriptCountsResId)
                {
                    int oldSize = resources[id].Size();
                    resources[id].ReplaceRawData(newRawData);
                    int newSize = resources[id].Size();

                    // Update subsequent offsets
                    for (int i = id + 1; i < ResourceCount; i++)
                    {
                        offsets[i] = checked(offsets[i] + newSize - oldSize);
                    }
                    // Update contentSize
                    ContentSize = checked(ContentSize + newSize - oldSize);
                }
                else
                {
                    throw new NotSupportedException("Cannot replace selected resource!");
                    //if (Common.IsEligibleForDict(newRawData))
                    //{
                    //    int oldSize = resources[id].Size();
                    //    resources[id].ReplaceRawData(newRawData);
                    //    int newSize = resources[id].Size();

                    //    // Update subsequent offsets
                    //    for (int i = id + 1; i < ResourceCount; i++)
                    //    {
                    //        offsets[i] = checked(offsets[i] + newSize - oldSize);
                    //    }
                    //    // Update contentSize
                    //    ContentSize = checked(ContentSize + newSize - oldSize);
                    //    // Update dictionary
                    //    ReadDict();
                    //}
                    //else
                    //{
                    //    throw new InvalidDataException("The new data is ineligible for dictionary resource");
                    //}
                }
            }
            else
            {
                throw new ArgumentException("Resource number out of range!");
            }
        }

        public void AddResourceRaw(int pos, uint type, uint format, byte[] newRawData)
        {
            throw new NotImplementedException();
            /*
            if (pos >= 0 && pos <= ResourceCount)
            {
                resources.Insert(pos, new Resource(type, format, newRawData));
                offsets.Insert(pos, pos == ResourceCount ? GetTotalSize() : offsets[pos]);
                // Update subsequent offsets
                if (pos < ResourceCount)
                {
                    for (int i = pos + 1; i < ResourceCount + 1; i++)
                    {
                        offsets[i] = checked(offsets[i] + resources[pos].Size());
                    }
                }
                // Update parameters
                ResourceCount += 1;
                ContentStart += 8;
                ContentSize = checked(ContentSize + resources[pos].Size());
                for (int i = 0; i < ResourceCount; i++)
                {
                    offsets[i] += 8;
                }
                if (dictResId >= pos)
                {
                    dictResId += 1;
                }
            }
            else
            {
                throw new ArgumentException("Resource number out of range!");
            }
            */
        }

        public void RemoveResource(int pos)
        {
            throw new NotImplementedException();
            /*
            if (pos >= 0 && pos < ResourceCount && pos != dictResId)
            {
                int removedSize = resources[pos].Size();
                resources.RemoveAt(pos);
                // Update subsequent ids and offsets
                if (pos < ResourceCount - 1)
                {
                    for (int i = pos; i < ResourceCount - 1; i++)
                    {
                        offsets[i] = checked(offsets[i] - removedSize);
                    }
                }
                // Update parameters
                ResourceCount -= 1;
                ContentStart -= 8;
                ContentSize = checked(ContentSize - removedSize);
                for (int i = 0; i < ResourceCount; i++)
                {
                    offsets[i] -= 8;
                }
                if (dictResId > pos)
                {
                    dictResId -= 1;
                }
            }
            else
            {
                throw new ArgumentException("Resource number is either the dictionary resource or out of range!");
            }
            */
        }

        public byte[] ConstructFile()
        {
            int tSize = GetTotalSize();
            byte[] result = new byte[tSize];
            ref byte startPos = ref MemoryMarshal.GetArrayDataReference(result);

            // Construct header
            result[0] = 0x46;
            result[1] = 0x47;
            result[2] = 0x49;
            result[3] = 0x42;
            result[4] = 0x01;
            result[5] = byteFive;
            result[6] = byteSix;
            result[7] = byteSeven;
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, 0x8), HeaderSize);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, 0xC), RleCount);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, 0x10), TableStart);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, 0x14), ResourceCount);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, 0x18), ContentStart);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, 0x1C), ContentSize);
            for (int i = 0; i < addHeaderBytes.Length; i++)
            {
                result[0x20 + i] = addHeaderBytes[i];
            }
            // Construct additional header
            for (int i = 0; i < RleCount; i++)
            {
                int pos = HeaderSize + (i * 8);
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, pos), rleEntries[i].startingVirtualId);
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, pos + 4), rleEntries[i].count);
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, pos + 6), rleEntries[i].startingId);
            }
            // Construct TOC
            for (int i = 0; i < ResourceCount; i++)
            {
                int pos = TableStart + (i * 8);
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, pos), resources[i].Type);
                Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, pos + 4), offsets[i]);
            }
            // Construct TOC ending
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, ContentStart - 0x8), 0);
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref startPos, ContentStart - 0x4), tSize);
            // Construct data
            for (int i = 0; i < ResourceCount; i++)
            {
                resources[i].Data().CopyTo(result.AsSpan(offsets[i], resources[i].Size()));
            }
            return result;
        }

        public void ConstructFileAndWrite(Stream target)
        {
            if (target != null && target.CanWrite)
            {
                using (BinaryWriter writer = new BinaryWriter(target, System.Text.Encoding.UTF8, true))
                {
                    // Construct header
                    writer.Write([0x46, 0x47, 0x49, 0x42, 0x01, byteFive, byteSix, byteSeven]);
                    writer.Write(HeaderSize);
                    writer.Write(RleCount);
                    writer.Write(TableStart);
                    writer.Write(ResourceCount);
                    writer.Write(ContentStart);
                    writer.Write(ContentSize);
                    writer.Write(addHeaderBytes);
                    // Construct additional header
                    foreach (RLEEntry addHeader in rleEntries)
                    {
                        writer.Write(addHeader.startingVirtualId);
                        writer.Write(addHeader.count);
                        writer.Write(addHeader.startingId);
                    }
                    // Construct TOC
                    for (int i = 0; i < ResourceCount; i++)
                    {
                        writer.Write(resources[i].Type);
                        writer.Write(offsets[i]);
                    }
                    // Construct TOC ending
                    writer.Write([0x00, 0x00, 0x00, 0x00]);
                    writer.Write(GetTotalSize());
                    // Construct data
                    foreach (Resource res in resources)
                    {
                        writer.Write(res.Data());
                    }
                }
            }
            else
            {
                throw new ArgumentException("Target stream is either null or not writable!");
            }
        }

        public int[] ScanForEligibleDict(bool autoSet)
        {
            List<int> valid = [];
            for (int i = 0; i < ResourceCount; i++)
            {
                if (Common.IsEligibleForDict(resources[i].RawData()))
                {
                    valid.Add(i);
                }
            }
            if (autoSet && valid.Count == 1)
            {
                // Use DictResId instead of dictResId in order to initialize the file
                DictResId = valid[0];
            }
            return valid.ToArray();
        }

        /*
         * The following lists the criteria that must be satisfied for Enhanced mode to work:
         * (1a) The only resource of type agg is resource 0.
         * (1b) The virtual IDs of the resources within aggResource must not overlap any virtual IDs in the main file (as specified by the RLE entries).
         * (2) The GAME_TOC_KEYSET resource as claimed in dictRes must be of type keyset.
         * (3) The OBJECT_SCRIPT_COUNTS resource as claimed in dictRes must be of type data.
         * (4) The GAME_TOC_KEYSET resource must have at least (N + 5) keys, where N is the number of object script counts. (N = 28 for versions with Deathmatch and should be 27 otherwise.)
         * (5a) The virtual IDs of these (N + 5) keys must be in strictly ascending order.
         * (5b) The virtual ID of the last of these (N + 5) keys must point to the OBJECT_SCRIPT_COUNTS resource located previously.
         * (6) The virtual IDs of these (N + 5) keys must fully span exactly one RLE virtual ID block in the main file. (Together with (5b), this implies that the virtual ID of the OBJECT_SCRIPT_COUNTS resource must be the last one of that RLE block. This also implies that their real and virtual IDs differ by a single constant.)
         * (7) The consecutive differences of the real/virtual IDs of the first (N + 1) of these keys, which is an array of length N, must equal (element-wise) the array formed by applying max(element, 1) to each element of the OBJECT_SCRIPT_COUNTS array.
        */
        public void TryEnableEnhanced()
        {
            // Check if dictResId is properly set and dictRes is initialized
            if (dictResId <= 0 || dictResId >= ResourceCount || dictRes == null)
            {
                RemoveEnhanced(false);
                return;
            }
            // Check if resource 0 is of type agg (1a)
            if (resources[0].Type != Constants.resourceTypeAgg)
            {
                RemoveEnhanced(false);
                return;
            }
            // Check that no other resource is of type agg, and try to initialize all keyset resources (1a)
            for (int i = 1; i < ResourceCount; i++)
            {
                if (resources[i].Type == Constants.resourceTypeAgg)
                {
                    RemoveEnhanced(false);
                    return;
                }
                else if (resources[i].Type == Constants.resourceTypeKeyset)
                {
                    try
                    {
                        keysetResources.Add(new KeysetResource(resources[i].RawData()));
                        keysetResourceIds.Add(i);
                    }
                    catch (Exception)
                    {
                        RemoveEnhanced(false);
                        return;
                    }
                }
            }
            int claimedGameTocKeySetResId = VirtualToRealId(Common.GetSmallestFifteenBits(dictRes.GetValueByKey(Constants.dictKeyGameTocKeyset)));
            // Check if GAME_TOC_KEYSET indeed points to a keyset type resource (2)
            if (resources[claimedGameTocKeySetResId].Type != Constants.resourceTypeKeyset)
            {
                RemoveEnhanced(false);
                return;
            }
            else
            {
                GameTocKeysetIndexInList = keysetResourceIds.IndexOf(claimedGameTocKeySetResId);
            }
            // Try to parse resource 0 (agg)
            try
            {
                aggResource = new AggResource(resources[0].RawData());
            }
            catch (Exception)
            {
                RemoveEnhanced(false);
                return;
            }
            // Check that virtual IDs of AggResource do not overlap with those of the main file. (1b)
            // If AggResource's Flag1 is true, then its virtual IDs are contiguous so a simple interval overlap check will do.
            // Otherwise, they need to be checked individually.
            if (aggResource.Flag1)
            {
                for (int i = 0; i < RleCount; i++)
                {
                    if (aggResource.GetVirtualId(0) < rleEntries[i].startingVirtualId + rleEntries[i].count && aggResource.GetVirtualId(0) + aggResource.ResourceCount > rleEntries[i].startingVirtualId)
                    {
                        RemoveEnhanced(false);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < aggResource.ResourceCount; i++)
                {
                    for (int j = 0; j < RleCount; j++)
                    {
                        if (aggResource.GetVirtualId(i) >= rleEntries[j].startingVirtualId && aggResource.GetVirtualId(i) < rleEntries[j].startingVirtualId + rleEntries[j].count)
                        {
                            RemoveEnhanced(false);
                            return;
                        }
                    }
                }
            }
            // Check ObjectScriptCounts Resource and try to initialize it (3)
            int claimedObjectScriptCountsResId = VirtualToRealId(Common.GetSmallestFifteenBits(dictRes.GetValueByKey(Constants.dictKeyObjectScriptCounts)));
            if (resources[claimedObjectScriptCountsResId].Type != Constants.resourceTypeData)
            {
                RemoveEnhanced(false);
                return;
            }
            else
            {
                
                try
                {
                    objectScriptCountsResource = new ObjectScriptCountsResource(resources[claimedObjectScriptCountsResId].RawData());
                    ObjectScriptCountsResId = claimedObjectScriptCountsResId;
                }
                catch (Exception)
                {
                    RemoveEnhanced(false);
                    return;
                }
            }
            // Check whether GAME_TOC_KEYSET offsets has at least as many keys as number of named sections (N + 5, where N = 27/28) (4)
            int numObjectScriptCounts = objectScriptCountsResource.GetCountNumber(); // N
            int numNamedSections = numObjectScriptCounts + 5; // N + 5
            if (keysetResources[GameTocKeysetIndexInList].GetCount() < numNamedSections)
            {
                RemoveEnhanced(false);
                return;
            }
            // Create array of virtual indices of the (N + 5) keys
            int[] keysetVirtualIds = new int[numNamedSections];
            for (int i = 0; i < numNamedSections; i++)
            {
                keysetVirtualIds[i] = Common.GetSmallestFifteenBits(keysetResources[GameTocKeysetIndexInList].GetValue(i));
            }
            // Check if they are strictly ascending (5a)
            for (int i = 0; i < numNamedSections - 1; i++)
            {
                if (keysetVirtualIds[i + 1] <= keysetVirtualIds[i])
                {
                    RemoveEnhanced(false);
                    return;
                }
            }
            // Check if last of the (N + 5) keys points to the object script count resource (5b)
            if (VirtualToRealId(keysetVirtualIds[^1]) != ObjectScriptCountsResId)
            {
                RemoveEnhanced(false);
                return;
            }
            // Check if the virtual indices of the (N + 5) keys fully span exactly one RLE block (6)
            if (rleEntries.Where(x => (keysetVirtualIds[0] == x.startingVirtualId && keysetVirtualIds[^1] == x.startingVirtualId + x.count - 1)).Count() != 1)
            {
                RemoveEnhanced(false);
                return;
            }
            // Check that the N object script counts are consistent with the consecutive differences of the first virtual indices of the (N + 5) keys (7)
            for (int i = 0; i < numObjectScriptCounts; i++)
            {
                if (Math.Max(objectScriptCountsResource.GetCount(i), (byte)1) != keysetVirtualIds[i + 1] - keysetVirtualIds[i])
                { 
                    RemoveEnhanced(false);
                    return;
                }
            }
            IsEnhanced = true;
        }

        private void RemoveEnhanced(bool removeDict)
        {
            aggResource = null;
            keysetResourceIds.Clear();
            keysetResources.Clear();
            GameTocKeysetIndexInList = -1;
            ObjectScriptCountsResId = -1;
            objectScriptCountsResource = null;
            IsEnhanced = false;
            if (removeDict)
            {
                dictRes = null;
                dictResId = -1;
            }
        }

        public string[,] GetEnhancedTable()
        {
            if (IsEnhanced)
            {
                int[] aggVirtualIds = aggResource.GetVirtualIds();
                // 0: virtualId, 1: realId/aggResourceId; 2: RLE Block; 3: Section
                string[,] result = new string[ResourceCount + aggVirtualIds.Length, 4];
                for (int i = 0; i < ResourceCount + aggVirtualIds.Length; i++)
                {
                    result[i, 3] = "-";
                }
                for (int i = 0; i < RleCount; i++)
                {
                    for (int j = 0; j < rleEntries[i].count; j++)
                    {
                        int resId = rleEntries[i].startingId + j;
                        result[resId, 0] = (rleEntries[i].startingVirtualId + j).ToString();
                        result[resId, 1] = "Res " + resId.ToString();
                        result[resId, 2] = i.ToString();
                    }
                }
                for (int i = 0; i < aggVirtualIds.Length; i++)
                {
                    result[ResourceCount + i, 0] = aggVirtualIds[i].ToString();
                    result[ResourceCount + i, 1] = "Agg " + i.ToString();
                    result[ResourceCount + i, 2] = "-";
                }

                int numNamedSections = objectScriptCountsResource.GetCountNumber() + 5; // N + 5
                int[] keysetVirtualIds = new int[numNamedSections];
                for (int i = 0; i < numNamedSections; i++)
                {
                    keysetVirtualIds[i] = Common.GetSmallestFifteenBits(keysetResources[GameTocKeysetIndexInList].GetValue(i));
                }
                RLEEntry rleNamedSections = rleEntries.First(x => (keysetVirtualIds[0] == x.startingVirtualId && keysetVirtualIds[^1] == x.startingVirtualId + x.count - 1));
                int shiftVToR = rleNamedSections.startingId - rleNamedSections.startingVirtualId;
                for (int i = 0; i < numNamedSections - 1; i++)
                {
                    int sectionStartRealId = keysetVirtualIds[i] + shiftVToR;
                    int nextSectionStartRealId = keysetVirtualIds[i + 1] + shiftVToR;
                    for (int j = sectionStartRealId; j < nextSectionStartRealId; j++)
                    {
                        result[j, 3] = (i + 1).ToString();
                    }
                }
                result[ObjectScriptCountsResId, 3] = numNamedSections.ToString();
                return result;
            }
            else
            {
                throw new InvalidOperationException("Enhanced mode is not enabled!");
            }
        }
    }
}