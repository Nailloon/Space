namespace SpaceBattle.Auxiliary
{
    public class Fraction
    {
        private int chislitel;
        private int znamenatel;
        public Fraction(int chislitel, int znamenatel)
        {
            this.chislitel = chislitel;
            this.znamenatel = znamenatel;
        }
        public static bool AreEquals(Fraction first, Fraction second)
        {
            if (first.chislitel == second.chislitel && first.znamenatel == second.znamenatel)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Fraction Transformation(Fraction drob)
        {
            int max = 0;
            if (drob.chislitel > drob.znamenatel)
            {
                max = Math.Abs(drob.znamenatel);
            }
            else
            {
                max = Math.Abs(drob.chislitel);
            }
            for (int i = max; i >= 2; i--)
            {
                if (drob.chislitel % i == 0 & drob.znamenatel % i == 0)
                {
                    drob.chislitel = drob.chislitel / i;
                    drob.znamenatel = drob.znamenatel / i;
                }
            }
            if (drob.znamenatel < 0)
            {
                drob.znamenatel = Math.Abs(drob.znamenatel);
                drob.chislitel = -1 * drob.chislitel;
            }
            return drob;
        }

        public static Fraction Summa(Fraction first, Fraction second)
        {
            var result = new Fraction(1, 1);
            if (first.znamenatel == second.znamenatel)
            {
                result.chislitel = first.chislitel + second.chislitel;
                result.znamenatel = first.znamenatel;
            }
            else
            {
                result.chislitel = first.chislitel * second.znamenatel + second.chislitel * first.znamenatel;
                result.znamenatel = first.znamenatel * second.znamenatel;
            }
            return Transformation(result);
        }
        public static Fraction Subtraction(Fraction first, Fraction second)
        {
            second.chislitel = second.chislitel * -1;
            return Summa(first, second);
        }
        public static Fraction MultInt(int first, Fraction second)
        {
            var result = new Fraction(first * second.chislitel, second.znamenatel);
            return Transformation(result);
        }
        public static Fraction operator + (Fraction first, Fraction second)
        {
            return Summa(first, second);
        }
        public static Fraction operator - (Fraction first, Fraction second)
        {
            return Subtraction(first, second);
        }
        public static Fraction operator * (int i, Fraction first)
        {
            return MultInt(i, first);
        }
    }
}
