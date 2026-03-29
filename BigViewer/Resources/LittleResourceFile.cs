using BigViewer.Common;

namespace BigViewer.Resources
{
    internal class LittleResourceFile
    {
        public bool isCore;
        public int resourceCount = 0;
        public int offsetTableEnd = 0;
        public int typeTableStart = 0;
        public int typeTableEnd = 0;
        public int totalSize = 0;

        public byte[] headerBytes = [];
        public List<Resource> resources = [];

        public LittleResourceFile(byte[] totalData)
        {
            if (totalData.Length > 0x10 && totalData.Length <= 0xFFFF)
            {
                totalSize = totalData.Length;
                
                if (totalData[0x0..0x2].SequenceEqual(new byte[] { 0x00, 0xA0 }))
                {
                    // Format found in non-0 packs
                    isCore = false;
                    headerBytes = totalData[0x0..0x6];
                    resourceCount = BitConverter.ToUInt16(headerBytes.AsSpan()[0x2..0x4]);
                    offsetTableEnd = 0x6 + (resourceCount * 2);
                    typeTableStart = offsetTableEnd + 0x4;
                    typeTableEnd = typeTableStart + (resourceCount * 4);

                    // Check for inconsistencies
                    if (BitConverter.ToUInt16(totalData.AsSpan()[0x6..0x8]) != typeTableEnd)
                    {
                        throw new ArgumentException("TOC end and content start mismatch!");
                    }
                    if (!(totalData[(offsetTableEnd + 2)..(offsetTableEnd + 4)].SequenceEqual(new byte[] { 0x00, 0x00 }) && BitConverter.ToUInt16(totalData[offsetTableEnd..(offsetTableEnd + 2)]) == totalSize))
                    {
                        throw new ArgumentException("TOC end mismatch!");
                    }

                    // Check if TOC is ordered and has no duplicate offsets
                    List<ushort> testList = [];
                    List<ushort> sortList = [];
                    for (int i = 0; i < resourceCount; i++)
                    {
                        int num = 0x6 + (i * 2);
                        testList.Add(BitConverter.ToUInt16(totalData.AsSpan()[num..(num + 0x2)]));
                        sortList.Add(BitConverter.ToUInt16(totalData.AsSpan()[num..(num + 0x2)]));
                    }
                    sortList.Sort();
                    if (!sortList.SequenceEqual(testList))
                    {
                        testList = [];
                        sortList = [];
                        throw new ArgumentException("TOC is not ordered!");
                    }
                    else if (testList.Distinct().Count() != testList.Count)
                    {
                        testList = [];
                        sortList = [];
                        throw new ArgumentException("TOC has duplicate offsets!");
                    }
                    else
                    {
                        testList = [];
                        sortList = [];
                    }

                    // Construct resource list
                    for (int i = 0; i < resourceCount; i++)
                    {
                        int numOffset = 0x6 + (i * 2);
                        int numType = typeTableStart + (i * 4);
                        ushort offset = BitConverter.ToUInt16(totalData.AsSpan()[numOffset..(numOffset + 0x2)]);
                        ushort offsetNext = BitConverter.ToUInt16(totalData.AsSpan()[(numOffset + 0x2)..(numOffset + 0x4)]);
                        resources.Add(new Resource(
                            _id: i,
                            _type: totalData[numType..(numType + 0x4)],
                            _offset: offset,
                            _data: totalData[offset..offsetNext]
                        ));
                    }
                }
                else if (totalData[0x0..0x2].SequenceEqual(new byte[] { 0x00, 0x20 }))
                {
                    // Format found in pack core 0
                    isCore = true;
                    headerBytes = totalData[0x0..0x4];
                    resourceCount = BitConverter.ToUInt16(headerBytes.AsSpan()[0x2..0x4]);
                    offsetTableEnd = 0x4 + (resourceCount * 4);
                    typeTableStart = offsetTableEnd + 0x4;
                    typeTableEnd = typeTableStart + (resourceCount * 4);

                    // Check for inconsistencies
                    if (BitConverter.ToUInt16(totalData.AsSpan()[0x6..0x8]) != typeTableEnd)
                    {
                        throw new ArgumentException("TOC end and content start mismatch!");
                    }
                    if (!(totalData[(offsetTableEnd + 2)..(offsetTableEnd + 4)].SequenceEqual(new byte[] { 0x00, 0x00 }) && BitConverter.ToUInt16(totalData[offsetTableEnd..(offsetTableEnd + 2)]) == totalSize))
                    {
                        throw new ArgumentException("TOC end mismatch!");
                    }

                    // Check if TOC is ordered and has no duplicate offsets
                    List<ushort> testList = [];
                    List<ushort> sortList = [];
                    for (int i = 0; i < resourceCount; i++)
                    {
                        int num = 0x4 + (i * 4);
                        testList.Add(BitConverter.ToUInt16(totalData.AsSpan()[(num + 0x2)..(num + 0x4)]));
                        sortList.Add(BitConverter.ToUInt16(totalData.AsSpan()[(num + 0x2)..(num + 0x4)]));
                    }
                    sortList.Sort();
                    if (!sortList.SequenceEqual(testList))
                    {
                        testList = [];
                        sortList = [];
                        throw new ArgumentException("TOC is not ordered!");
                    }
                    else if (testList.Distinct().Count() != testList.Count)
                    {
                        testList = [];
                        sortList = [];
                        throw new ArgumentException("TOC has duplicate offsets!");
                    }
                    else
                    {
                        testList = [];
                        sortList = [];
                    }

                    // Construct resource list
                    for (int i = 0; i < resourceCount; i++)
                    {
                        int num = 0x4 + (i * 4);
                        int numType = typeTableStart + (i * 4);
                        ushort offset = BitConverter.ToUInt16(totalData.AsSpan()[(num + 0x2)..(num + 0x4)]);
                        ushort offsetNext = BitConverter.ToUInt16(totalData.AsSpan()[(num + 0x6)..(num + 0x8)]);
                        resources.Add(new Resource(
                            _id: i,
                            _type: totalData[numType..(numType + 0x4)],
                            _offset: offset,
                            _data: totalData[offset..(i == (resourceCount - 1) ? totalSize : offsetNext)],
                            _otherData: totalData[num..(num + 0x2)]
                        ));
                    }
                }
                else
                {
                    throw new ArgumentException("Incorrect little header!");
                }
            }
            else
            {
                throw new ArgumentException("Incorrect file size!");
            }
        }

        public void ReplaceResourceRaw(int id, byte[] newRawData)
        {
            // Check if new data will cause size to exceed
            int oldSize = resources[id].size;
            byte[] testEncode = Utils.EncodeResource(newRawData, resources[id].format);
            if (totalSize + testEncode.Length - oldSize <= 0xFFFF)
            {
                resources[id].data = testEncode;
                resources[id].rawData = newRawData;
                resources[id].size = resources[id].data.Length;
                resources[id].rawSize = newRawData.Length;
                int newSize = resources[id].size;

                // Update subsequent offsets
                for (int i = id + 1; i < resourceCount; i++)
                {
                    resources[i].offset = checked(resources[i].offset + newSize - oldSize);
                }
                totalSize = checked(totalSize + newSize - oldSize);
            }
            else
            {
                throw new ArgumentException("File size too large!");
            }
        }

        public byte[] ConstructFile()
        {
            if (!isCore)
            {
                List<byte> result = [];

                // Construct header
                result.AddRange(headerBytes);
                // Construct offset table
                foreach (Resource res in resources)
                {
                    result.AddRange(BitConverter.GetBytes(checked((ushort)res.offset)));
                }
                // Construct offset table ending
                result.AddRange(BitConverter.GetBytes(checked((ushort)totalSize)));
                result.AddRange(new byte[] { 0x00, 0x00 });
                // Construct type table
                foreach (Resource res in resources)
                {
                    result.AddRange(res.type);
                }
                // Construct data
                foreach (Resource res in resources)
                {
                    result.AddRange(res.data);
                }
                return result.ToArray();
            }
            else
            {
                List<byte> result = [];

                // Construct header
                result.AddRange(headerBytes);
                // Construct offset table
                foreach (Resource res in resources)
                {
                    result.AddRange(res.otherData);
                    result.AddRange(BitConverter.GetBytes(checked((ushort)res.offset)));
                }
                // Construct offset table ending
                result.AddRange(BitConverter.GetBytes(checked((ushort)totalSize)));
                result.AddRange(new byte[] { 0x00, 0x00 });
                // Construct type table
                foreach (Resource res in resources)
                {
                    result.AddRange(res.type);
                }
                // Construct data
                foreach (Resource res in resources)
                {
                    result.AddRange(res.data);
                }
                return result.ToArray();
            }
        }
    }
}