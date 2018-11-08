using System;
using UnityEngine;

namespace Megamap {

    public static class SequenceLoader {

        public static int[][] LoadSequences(TextAsset file)
        {
            var content = file.text;
            var lines = content.Split('\n');
            var sequences = new int[lines.Length][];

            for (int i = 0; i < lines.Length; ++i) {
                var numbers = lines[i].Split(new string[] { ", " }, StringSplitOptions.None);
                sequences[i] = new int[numbers.Length];
                for (int j = 0; j < numbers.Length; ++j) {
                    sequences[i][j] = Int32.Parse(numbers[j]);
                }
            }

            return sequences;
        }

    }

}
