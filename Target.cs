using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MemoryLib;

namespace RiftLibrary
{

    public enum eTargetOffsets
    {
        TARGET_ID = 0x178,
        TARGET_IN_COMBAT = 0x180,
        TARGET_HP_CURRENT = 0x184,
        TARGET_HP_MAX = 0x188,
        TARGET_MP_CURRENT = 0x18C,
        TARGET_MP_MAX = 0x190

    }

    class Target
    {
        uint targetAddress = 0;


        public Target()
        {
            uint add1, baseAdd = 0;
            baseAdd = Memory.ReadUInt(RiftProcess.handle, (uint)RiftProcess.Modules.Game + 0xDCABA8);

            add1 = Memory.ReadUInt(RiftProcess.handle, baseAdd + 0x178);
            targetAddress = Memory.ReadUInt(RiftProcess.handle, add1 + 0x244);
            

        }



        public int HP_Max
        {

            get
            {
                if (ID != 0)
                    return Memory.ReadInt(RiftProcess.handle, targetAddress + (uint)eTargetOffsets.TARGET_HP_MAX);
                else
                    return 0;
            }

        }

        public int HP_Current
        {

            get
            {
                if (ID != 0)
                  return  Memory.ReadInt(RiftProcess.handle, targetAddress + (uint)eTargetOffsets.TARGET_HP_CURRENT);
                else
                    return 0;
            }

        }

        public int MP_Max
        {

            get
            {
                if (ID != 0)
                    return Memory.ReadInt(RiftProcess.handle, targetAddress + (uint)eTargetOffsets.TARGET_MP_MAX);
                else
                    return 0;
            }

        }

        public int MP_Current
        {

            get
            {
                if (ID != 0)
                    return Memory.ReadInt(RiftProcess.handle, targetAddress + (uint)eTargetOffsets.TARGET_MP_CURRENT);
                else
                    return 0;
            }

        }

        public int ID
        {

            get
            {
                return Memory.ReadShort(RiftProcess.handle, targetAddress + (uint)eTargetOffsets.TARGET_ID);

            }

        }
        public bool InCombat
        {

            get
            {
                return (Memory.ReadByte(RiftProcess.handle, targetAddress + (uint)eTargetOffsets.TARGET_IN_COMBAT) == 1);

            }

        }












    }
}
