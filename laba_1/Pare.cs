namespace laba_1
{
    public class Pare
    {
        string Name;
        int Priority;
        public Pare(string N, int P)
        {
            Name = N;
            Priority = P;
        }
        public string GetName()
        {
            return Name;
        }

        public int GetPr()
        {
            return Priority;
        }
    }
}