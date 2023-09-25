﻿using System.ComponentModel;

namespace Acn.Rdm
{
    public abstract class RdmResponsePacket:RdmPacket
    {
        public RdmResponsePacket(RdmCommands command, RdmParameters parameterId):base(command, parameterId)
        {
        }

        [Browsable(false)]
        public RdmResponseTypes ResponseType { get; set; }
    }
}
