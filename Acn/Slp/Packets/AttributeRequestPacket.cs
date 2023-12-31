﻿#region Copyright © 2011 Oliver Waits
//______________________________________________________________________________________________________________
// Service Location Protocol
// Copyright © 2011 Oliver Waits
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//______________________________________________________________________________________________________________
#endregion

using Acn.Slp.IO;

namespace Acn.Slp.Packets
{
    public class AttributeRequestPacket : SlpRequestPacket
    {
        #region Setup and Initialisation

        public AttributeRequestPacket()
            : base(SlpFunctionId.AttributeRequest)
        {
        }

        #endregion

        #region Packet Contents

        public string Url { get; set; }

        public List<string> TagList { get; set; }

        #endregion

        #region Read and Write

        protected override void ReadData(SlpBinaryReader data)
        {
            PRList = new List<string>(data.ReadNetworkStringList());
            Url = data.ReadNetworkString();
            ScopeList = data.ReadNetworkString();
            TagList = new List<string>(data.ReadNetworkStringList());
            SlpSpi = data.ReadNetworkString();
        }

        protected override void WriteData(SlpBinaryWriter data)
        {
            data.Write(PRList);
            data.WriteNetworkString(Url);
            data.WriteNetworkString(ScopeList);
            data.Write(TagList);
            data.WriteNetworkString(SlpSpi);
        }

        #endregion
    }
}
