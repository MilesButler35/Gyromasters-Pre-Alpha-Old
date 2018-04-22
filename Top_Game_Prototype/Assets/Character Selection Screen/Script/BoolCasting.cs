using UnityEngine;

namespace CharacterSelectionScreen
{
    public class BoolCasting : MonoBehaviour
    {
        // Class written by Bruno Mattos - 2016
        // More info about bit processing can be found at: http://derekwill.com/2015/03/05/bit-processing-in-c/
        //
        //
        // The main purpose of this class is to make it easier to save and load bool values using PlayerPrefs
        // It convert a bool array to a single int and vice-versa
        //
        //
        // HOW TO
        //
        // LOAD
        // myBool = BoolCasting.IntToBoolArray(PlayerPrefs.GetInt("BoolCast"), myBool.Length);
        //
        // SAVE
        // PlayerPrefs.SetInt("BoolCast", BoolCasting.BoolArrayToInt(myBool));





        /// <summary>
        /// Convert a bool array to a single int
        /// </summary>
        /// <param name="array">The bool array to be used in the conversion</param>
        /// <returns>The converted int</returns>
        public static int BoolArrayToInt(bool[] array)
        {
            int result = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i])
                    result = result | (1 << i);
            }
            return result;
        }

        /// <summary>
        /// Convert a single int to a bool array
        /// </summary>
        /// <param name="source">The int to be used in the conversion</param>
        /// <param name="arrayLenght">The desired lenght of the bool, this prevent having to return an array of lenght 32</param>
        /// <returns>The converted bool array</returns>
        public static bool[] IntToBoolArray(int source, int arrayLenght)
        {
            bool[] result = new bool[arrayLenght];
            for (int i = 0; i < arrayLenght; i++)
            {
                result[i] = (source & (1 << i)) != 0;
            }
            return result;
        }

        /// <summary>
        /// Change just one bit in the int based on the bool value
        /// </summary>
        /// <param name="source">The int to be updated</param>
        /// <param name="index">The bit position to be changed (Usually it's used "myBool[index]")</param>
        /// <param name="value">The desired value</param>
        /// <returns>Return the updated int</returns>
        public static int BoolToIntBit(int source, int index, bool value)
        {
            return value ? source | (1 << index) : source & ~(1 << index);
        }

        /// <summary>
        /// Returns a bool based on the bit value from the desired position
        /// </summary>
        /// <param name="source">Int used to find the bool value</param>
        /// <param name="index">The bit position to be checked (Usually it's used "myBool[index]")</param>
        /// <returns>The value found in the bit</returns>
        public static bool IntBitToBool(int source, int index)
        {
            return (source & (1 << index)) != 0;
        }
    }
}
