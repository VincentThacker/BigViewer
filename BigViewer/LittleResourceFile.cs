namespace BigViewer
{
    internal class LittleResourceFile
    {
        public bool isCore;
        public int resourceCount = 0;
        public int offsetTableEnd = 0;
        public int typeTableStart = 0;
        public int typeTableEnd = 0;
        public int totalSize = 0;

        public byte[] headerBytes1;
        public byte[] headerBytes2;
        public List<Resource> resources = [];

        public LittleResourceFile(byte[] totalData)
        {
            if (totalData[0x0..0x2].SequenceEqual(new byte[] { 0x00, 0xA0 }))
            {
                // Format found in non-0 packs
                isCore = false;
                totalSize = totalData.Length;
                resourceCount = BitConverter.ToUInt16(totalData[0x2..0x4]);
                offsetTableEnd = 0x6 + (resourceCount * 2);
                typeTableStart = offsetTableEnd + 0x4;
                typeTableEnd = typeTableStart + (resourceCount * 4);

                headerBytes1 = totalData[0x0..0x2];
                headerBytes2 = totalData[0x4..0x6];

                // Check for inconsistencies
                if (totalSize > 0xFFFF)
                {
                    throw new ArgumentException("File size too large!");
                }
                if (BitConverter.ToUInt16(totalData[0x6..0x8]) != typeTableEnd)
                {
                    throw new ArgumentException("TOC end and content start mismatch!");
                }
                if (!(totalData[(offsetTableEnd + 2)..(offsetTableEnd + 4)].SequenceEqual(new byte[] { 0x00, 0x00 }) && BitConverter.ToUInt16(totalData[offsetTableEnd..(offsetTableEnd + 2)]) == totalSize))
                {
                    throw new ArgumentException("TOC end mismatch!");
                }

                // Check if TOC is ordered
                List<ushort> testList = [];
                List<ushort> sortList = [];
                for (int i = 0; i < resourceCount; i++)
                {
                    var num = 0x6 + (i * 2);
                    testList.Add(BitConverter.ToUInt16(totalData[num..(num + 0x2)]));
                    sortList.Add(BitConverter.ToUInt16(totalData[num..(num + 0x2)]));
                }
                sortList.Sort();
                if (!sortList.SequenceEqual(testList))
                {
                    testList = [];
                    sortList = [];
                    throw new ArgumentException("TOC is not ordered!");
                }
                else
                {
                    testList = [];
                    sortList = [];
                }

                // Construct resource list
                for (int i = 0; i < resourceCount; i++)
                {
                    var numOffset = 0x6 + (i * 2);
                    var numType = typeTableStart + (i * 4);
                    ushort offset = BitConverter.ToUInt16(totalData[numOffset..(numOffset + 0x2)]);
                    ushort offsetNext = BitConverter.ToUInt16(totalData[(numOffset + 0x2)..(numOffset + 0x4)]);
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
                totalSize = totalData.Length;
                resourceCount = BitConverter.ToUInt16(totalData[0x2..0x4]);
                offsetTableEnd = 0x4 + (resourceCount * 4);
                typeTableStart = offsetTableEnd + 0x4;
                typeTableEnd = typeTableStart + (resourceCount * 4);

                headerBytes1 = totalData[0x0..0x2];
                headerBytes2 = [];

                // Check for inconsistencies
                if (totalSize > 0xFFFF)
                {
                    throw new ArgumentException("File size too large!");
                }
                if (BitConverter.ToUInt16(totalData[0x6..0x8]) != typeTableEnd)
                {
                    throw new ArgumentException("TOC end and content start mismatch!");
                }
                if (!(totalData[(offsetTableEnd + 2)..(offsetTableEnd + 4)].SequenceEqual(new byte[] { 0x00, 0x00 }) && BitConverter.ToUInt16(totalData[offsetTableEnd..(offsetTableEnd + 2)]) == totalSize))
                {
                    throw new ArgumentException("TOC end mismatch!");
                }

                // Check if TOC is ordered
                List<ushort> testList = [];
                List<ushort> sortList = [];
                for (int i = 0; i < resourceCount; i++)
                {
                    var num = 0x4 + (i * 4);
                    testList.Add(BitConverter.ToUInt16(totalData[(num + 0x2)..(num + 0x4)]));
                    sortList.Add(BitConverter.ToUInt16(totalData[(num + 0x2)..(num + 0x4)]));
                }
                sortList.Sort();
                if (!sortList.SequenceEqual(testList))
                {
                    testList = [];
                    sortList = [];
                    throw new ArgumentException("TOC is not ordered!");
                }
                else
                {
                    testList = [];
                    sortList = [];
                }

                // Construct resource list
                for (int i = 0; i < resourceCount; i++)
                {
                    var num = 0x4 + (i * 4);
                    var numType = typeTableStart + (i * 4);
                    ushort offset = BitConverter.ToUInt16(totalData[(num + 0x2)..(num + 0x4)]);
                    ushort offsetNext = BitConverter.ToUInt16(totalData[(num + 0x6)..(num + 0x8)]);
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

        public void ReplaceResourceRaw(int id, byte[] newRawData)
        {
            if (newRawData.Length < 0xFFFF)
            {
                uint oldSize = resources[id].size;
                resources[id].data = Utils.EncodeResource(newRawData, resources[id].format);
                resources[id].rawData = newRawData;
                resources[id].size = checked((uint)resources[id].data.Length);
                resources[id].rawSize = checked((uint)newRawData.Length);
                uint newSize = resources[id].size;

                if (newSize > oldSize)
                {
                    // Update subsequent offsets
                    for (int i = id + 1; i < resourceCount; i++)
                    {
                        resources[i].offset += (newSize - oldSize);
                    }
                    totalSize += checked((int)(newSize - oldSize));
                }
                else if (oldSize > newSize)
                {
                    // Update subsequent offsets
                    for (int i = id + 1; i < resourceCount; i++)
                    {
                        resources[i].offset -= (oldSize - newSize);
                    }
                    totalSize -= checked((int)(oldSize - newSize));
                }
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
                result.AddRange(headerBytes1);
                result.AddRange(BitConverter.GetBytes(checked((ushort)resourceCount)));
                result.AddRange(headerBytes2);
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
                result.AddRange(headerBytes1);
                result.AddRange(BitConverter.GetBytes(checked((ushort)resourceCount)));
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
