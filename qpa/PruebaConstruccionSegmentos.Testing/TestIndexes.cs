using System;
using Xunit;
using PruebaConstruccionSegmentos;
using System.Linq;
using System.Collections.Generic;

namespace PruebaConstruccionSegmentos.Testing
{
    public class TestIndexes
    {
        //[Fact]
        //public void UnaColeccionNulaTiraExcepcion()
        //{
        //    char[] elems = null;
        //    Assert.Throws<ArgumentNullException>(() => 
        //    {
        //        RepetitionIndexHelper.GetIndexes(elems)
        //            .ToList() // para que se ejecute
        //        ;
        //    });
        //}

        [Fact]
        public void IntegridadDeIndices0()
        {
            var elems = new int[] { };
            var indexes = RepetitionIndexHelper.GetIndexes(elems);

            // nunca puede ser nulo, por mas que no le pasen elementos
            Assert.NotNull(indexes);

            // cantidad de índices ok
            Assert.Empty(indexes);
        }

        [Fact]
        public void IntegridadDeIndices1()
        {
            // index                0  1  2
            var elems = new int[] { 1, 2, 3 };
            var indexes = RepetitionIndexHelper.GetIndexes(elems);

            // cantidad de índices ok
            Assert.Equal(3, indexes.Count());

            // 1
            Assert.Equal(1, indexes.ElementAt(0).Value);
            Assert.Equal(0, indexes.ElementAt(0).StartIndex);
            Assert.Equal(0, indexes.ElementAt(0).EndIndex);
            Assert.Equal(1, indexes.ElementAt(0).Length);

            // 2
            Assert.Equal(2, indexes.ElementAt(1).Value);
            Assert.Equal(1, indexes.ElementAt(1).StartIndex);
            Assert.Equal(1, indexes.ElementAt(1).EndIndex);
            Assert.Equal(1, indexes.ElementAt(1).Length);

            // 3
            Assert.Equal(3, indexes.ElementAt(2).Value);
            Assert.Equal(2, indexes.ElementAt(2).StartIndex);
            Assert.Equal(2, indexes.ElementAt(2).EndIndex);
            Assert.Equal(1, indexes.ElementAt(2).Length);
        }

        [Fact]
        public void IntegridadDeIndices2()
        {
            // index                0  1  2  3  4  5  6  7  8
            var elems = new int[] { 8, 2, 2, 2, 9, 5, 5, 2, 2 };
            var indexes = RepetitionIndexHelper.GetIndexes(elems);

            // cantidad de índices ok
            Assert.Equal(5, indexes.Count());

            // 8
            Assert.Equal(8, indexes.ElementAt(0).Value);
            Assert.Equal(0, indexes.ElementAt(0).StartIndex);
            Assert.Equal(0, indexes.ElementAt(0).EndIndex);
            Assert.Equal(1, indexes.ElementAt(0).Length);

            // 2, 2, 2
            Assert.Equal(2, indexes.ElementAt(1).Value);
            Assert.Equal(1, indexes.ElementAt(1).StartIndex);
            Assert.Equal(3, indexes.ElementAt(1).EndIndex);
            Assert.Equal(3, indexes.ElementAt(1).Length);

            // 9
            Assert.Equal(9, indexes.ElementAt(2).Value);
            Assert.Equal(4, indexes.ElementAt(2).StartIndex);
            Assert.Equal(4, indexes.ElementAt(2).EndIndex);
            Assert.Equal(1, indexes.ElementAt(2).Length);

            // 5, 5
            Assert.Equal(5, indexes.ElementAt(3).Value);
            Assert.Equal(5, indexes.ElementAt(3).StartIndex);
            Assert.Equal(6, indexes.ElementAt(3).EndIndex);
            Assert.Equal(2, indexes.ElementAt(3).Length);

            // 2, 2
            Assert.Equal(2, indexes.ElementAt(4).Value);
            Assert.Equal(7, indexes.ElementAt(4).StartIndex);
            Assert.Equal(8, indexes.ElementAt(4).EndIndex);
            Assert.Equal(2, indexes.ElementAt(4).Length);
        }
    }
}
