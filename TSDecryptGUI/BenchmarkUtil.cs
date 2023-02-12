﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TSDecryptGUI
{
    internal class BenchmarkUtil
    {
        static int PACKET_SIZE = 188;
        static int TS_PKTS_FOR_TEST = 500 * 1000;
        static string test_2_key = "2022014317aacc8d";
        static byte[] test_2_encrypted = {
            0x47, 0x40, 0x21, 0x91, 0x2A, 0xFA, 0xA8, 0x84, 0x09, 0x9F, 0x86, 0x68, 0xCD, 0x16, 0xA0, 0x4E,
            0xC2, 0x58, 0x2E, 0xA3, 0xCC, 0x00, 0x0F, 0x5A, 0xD6, 0x9B, 0x07, 0x83, 0x67, 0x79, 0x94, 0xE0,
            0x55, 0xB8, 0xA9, 0xF0, 0x85, 0xE1, 0xE9, 0x5F, 0x5C, 0xA4, 0xBC, 0x24, 0x24, 0xA8, 0x0D, 0x98,
            0xFF, 0xA7, 0xCE, 0x1D, 0x52, 0xE5, 0x12, 0xDE, 0xC1, 0x74, 0x1D, 0xD1, 0xFF, 0xBD, 0x89, 0xC6,
            0x10, 0x66, 0x42, 0xE0, 0x28, 0x0A, 0x53, 0xF8, 0xAB, 0xC1, 0xE8, 0xF2, 0x65, 0xD4, 0xE6, 0x50,
            0xE8, 0x1D, 0x92, 0x24, 0x2C, 0x73, 0xE4, 0x2D, 0x83, 0x97, 0xBD, 0x94, 0xED, 0x97, 0xF4, 0x2A,
            0x12, 0xB1, 0x4D, 0xC0, 0x4B, 0x38, 0x89, 0x31, 0x97, 0x99, 0x4E, 0xF9, 0xF2, 0x8E, 0x90, 0x34,
            0x9B, 0x37, 0xC8, 0xCB, 0xA3, 0x5F, 0x20, 0x8F, 0x5C, 0xF2, 0x91, 0xDB, 0xA8, 0x46, 0x1B, 0x6B,
            0x9D, 0xFB, 0xCC, 0x18, 0x89, 0x9C, 0x02, 0x5D, 0xA0, 0xE8, 0x5C, 0x83, 0xDD, 0x73, 0xF4, 0xCC,
            0x36, 0x9A, 0xB7, 0xD0, 0xB9, 0x06, 0xE2, 0xE3, 0xC3, 0x5E, 0xA7, 0x71, 0x11, 0xB1, 0xDF, 0x0C,
            0xEB, 0x07, 0x03, 0x7F, 0xFE, 0x43, 0x1B, 0xC0, 0x42, 0xB6, 0x09, 0xC5, 0xBF, 0xD5, 0xCA, 0x23,
            0xAA, 0x41, 0xA1, 0xAA, 0xEE, 0x68, 0xAC, 0xE1, 0x8F, 0xD6, 0x13, 0x3F
        };

        public static void Run(Action<int, long> action)
        {
            var encrypted = new byte[PACKET_SIZE * TS_PKTS_FOR_TEST];
            for (int i = 0; i < TS_PKTS_FOR_TEST; i++)
            {
                Array.Copy(test_2_encrypted, 0, encrypted, i * PACKET_SIZE, PACKET_SIZE);
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var stream = new MemoryStream(encrypted))
            {
                var tsdecrypt = new TSDecrypt();
                tsdecrypt.SetKey(test_2_key);
                var readSize = 0;
                var buffer = new byte[PACKET_SIZE * tsdecrypt.PARALL_SIZE]; // MAX TS Packets
                while ((readSize = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    tsdecrypt.DecryptBytes(readSize, ref buffer);
                }
            }

            stopwatch.Stop();

            action(encrypted.Length, stopwatch.ElapsedMilliseconds);
        }
    }
}