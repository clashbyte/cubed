// /***************************************************************************
//  * PushbackStream.cs
//  * Copyright (c) 2015 the authors.
//  * 
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the GNU Lesser General Public License
//  * (LGPL) version 3 which accompanies this distribution, and is available at
//  * https://www.gnu.org/licenses/lgpl-3.0.en.html
//  *
//  * This library is distributed in the hope that it will be useful,
//  * but WITHOUT ANY WARRANTY; without even the implied warranty of
//  * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  * Lesser General Public License for more details.
//  *
//  ***************************************************************************/

using System;
using System.IO;

namespace MP3Sharp.Decoding
{
    /// <summary>
    /// A PushbackStream is a stream that can "push back" or "unread" data. This is useful in situations where it is convenient for a
    /// fragment of code to read an indefinite number of data bytes that are delimited by a particular byte value; after reading the
    /// terminating byte, the code fragment can "unread" it, so that the next read operation on the input stream will reread the byte
    /// that was pushed back.
    /// </summary>
    internal class PushbackStream
    {
        private readonly Stream m_Stream;

        public PushbackStream(Stream s, int backBufferSize)
        {
            m_Stream = s;
        }

        public int Read(sbyte[] toRead, int offset, int length)
        {
            // Read 
            int currentByte = 0;
            bool canReadStream = true;
			byte[] tempBuffer = new byte[length];
            while (currentByte < length && canReadStream)
            {
                // from stream
                int newBytes = length - currentByte;
                int numRead = m_Stream.Read(tempBuffer, 0, newBytes);
                canReadStream = numRead >= newBytes;
                for (int i = 0; i < numRead; i++)
                {
                    toRead[offset + currentByte + i] = (sbyte)tempBuffer[i];
                }
                currentByte += numRead;
            }
            return currentByte;
        }

        public void UnRead(int length)
        {
			m_Stream.Position -= length;
        }

        public void Close()
        {
            m_Stream.Close();
        }
    }
}