using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MemoryLib;
namespace RiftLibrary
{
    //not off riftbase
    public enum ePlayerStatOffsets
    {
        PLAYER_ID = 0x178,
        IN_COMBAT = 0x180,
        HEALTH_CURRENT = 0x184,
        HEALTH_MAX = 0x188,
        MANA_CURRENT = 0x18C,
        MANA_MAX   = 0x190  
    }


    //all off riftbase
    public enum ePlayerOffsets
    {
        PLAYER_H = 0xE8,
        PLAYER_X = 0x100,
        PLAYER_Z = 0x104,
        PLAYER_Y = 0x108,
        PLAYER_NAME = 0x394 //is borked
    }
    //is a pointer off riftbase
    public enum eRotationOffsets
    {

        ROTATION_Y = 0x28,
        ROTATION_X = 0x98

    }



    public class Player
    {


        uint playerInfoAddress = 0;
        uint playerLocAddress = 0;
        uint rotationAddress = 0;
        uint riftBase = 0;

        public Player()
        {

            uint add1,add2, baseAdd = 0;
            baseAdd = Memory.ReadUInt(RiftProcess.handle, (uint)RiftProcess.Modules.Game + 0xDEA4C8);

            add1 = Memory.ReadUInt(RiftProcess.handle, baseAdd + 0x1E0);
            add2 = Memory.ReadUInt(RiftProcess.handle, add1 + 0x0);
            playerInfoAddress = Memory.ReadUInt(RiftProcess.handle, add2 + 0x64);
            riftBase = Memory.ReadUInt(RiftProcess.handle, (uint)RiftProcess.Modules.Game + 0xDC7DA8);

            rotationAddress = Memory.ReadUInt(RiftProcess.handle, riftBase + 0x110);
            
            //int dsffds = 43;
      }
        #region PlayerStuff
        //player stuff
        public int HP_Max
        {

            get
            {
                return Memory.ReadInt(RiftProcess.handle, playerInfoAddress + (uint)ePlayerStatOffsets.HEALTH_MAX);

            }

        }

        public int HP_Current
        {

            get
            {
                return Memory.ReadInt(RiftProcess.handle, playerInfoAddress + (uint)ePlayerStatOffsets.HEALTH_CURRENT);

            }

        }

        public int MP_Max
        {

            get
            {
                return Memory.ReadInt(RiftProcess.handle, playerInfoAddress + (uint)ePlayerStatOffsets.MANA_MAX);

            }

        }

        public int MP_Current
        {

            get
            {
                return Memory.ReadInt(RiftProcess.handle, playerInfoAddress + (uint)ePlayerStatOffsets.MANA_CURRENT);

            }

        }

        public int ID
        {

            get
            {
                return Memory.ReadShort(RiftProcess.handle, playerInfoAddress + (uint)ePlayerStatOffsets.PLAYER_ID);

            }

        }
        public bool InCombat
        {

            get
            {
                return (Memory.ReadByte(RiftProcess.handle, playerInfoAddress + (uint)ePlayerStatOffsets.IN_COMBAT) == 1);

            }

        }

        public float X
        {

            get
            {
                return Memory.ReadFloat(RiftProcess.handle, riftBase + (uint)ePlayerOffsets.PLAYER_X);

            }

        }
        public float Z
        {

            get
            {
                return Memory.ReadFloat(RiftProcess.handle, riftBase + (uint)ePlayerOffsets.PLAYER_Z);

            }

        }
        public float Y
        {

            get
            {
                return Memory.ReadFloat(RiftProcess.handle, riftBase + (uint)ePlayerOffsets.PLAYER_Y);

            }

        }
        public float Heading
        {

            get
            {
                return Memory.ReadFloat(RiftProcess.handle, riftBase + (uint)ePlayerOffsets.PLAYER_H);

            }

        }

        public float RotationX
        {

            get
            {
                return Memory.ReadFloat(RiftProcess.handle, rotationAddress + (uint)eRotationOffsets.ROTATION_X);

            }

        }
        public float RotationY
        {

            get
            {
                return Memory.ReadFloat(RiftProcess.handle, rotationAddress + (uint)eRotationOffsets.ROTATION_Y);

            }

        }

        public string Name
        {
            get
            {
                return Memory.ReadString(RiftProcess.handle, (riftBase + (uint)ePlayerOffsets.PLAYER_NAME), 64, true);
            }
        }

        #endregion



    }
}
