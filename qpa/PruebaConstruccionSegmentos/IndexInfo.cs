namespace PruebaConstruccionSegmentos
{
    public class IndexInfo<TValue>
    {
        public TValue Value { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int Length => (EndIndex - StartIndex) + 1;
        public override string ToString()
        {
            return $"value={Value} start={StartIndex} end={EndIndex} len={Length}";
        }
    }   
}
