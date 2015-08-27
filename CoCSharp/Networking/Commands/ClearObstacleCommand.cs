﻿namespace CoCSharp.Networking.Commands
{
    public class ClearObstacleCommand : ICommand
    {
        public int ID { get { return 0x1FB; } }

        public int ObstacleID;
        private int Unknown1;

        public void ReadCommand(PacketReader reader)
        {
            ObstacleID = reader.ReadInt32();
            Unknown1 = reader.ReadInt32();
        }

        public void WriteCommand(PacketWriter writer)
        {
            writer.WriteInt32(ObstacleID);
            writer.WriteInt32(Unknown1);
        }
    }
}