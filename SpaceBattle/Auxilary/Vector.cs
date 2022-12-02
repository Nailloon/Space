namespace SpaceBattle.Auxiliary
{
    public class Vector
    {
        public int[] array;
        public Vector(params int[] list)
        {
            array = list;
        }

        public static bool AreSameSize(Vector first, Vector second)
        {
            return first.array.Length == second.array.Length;
        }

        public static Vector Sum(Vector first, Vector second)
        {
            if (AreSameSize(first, second))
            {
                int[] arr = new int[first.array.Length];
                for (int i = 0; i < first.array.Length; i++)
                {
                    arr[i] = first.array[i] + second.array[i];
                }
                return new Vector(arr);
            }
            else
            {
                throw new ArgumentException();
            }
        }
        public static Vector operator +(Vector first, Vector second)
        {
            return Sum(first, second);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(array);
        }
        public override bool Equals(object? obj)
        {
            return obj is Vector;
        }

        public static bool operator ==(Vector first, Vector second)
        {
            return first.array.SequenceEqual(second.array);
        }
        public static bool operator !=(Vector first, Vector second)
        {
            return !(first == second);
        }
    }
}
