// -----------------------------------------------------------------------
// <copyright file="BKDR.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Bodu.Security.Cryptography.Extensions
{
    #region Using Statements

    using System;
    using System.IO;
    using System.Security.Cryptography;

    using Xunit;

    #endregion Using Statements

    public partial class SymmetricAlgorithmExtensionTests
    {
        public class Decrypt
        {
            #region Methods

            [Fact]
            public void ShouldInvokeICryptoTransformDispose()
            {
                using (SimpleReversingSymmetricAlgorithm algorithm = new SimpleReversingSymmetricAlgorithm())
                {
                    byte[] buffer = new byte[0];
                    Assert.True(algorithm.IsCryptoTransformDisposed);
                    algorithm.Decrypt(buffer);
                    Assert.True(algorithm.IsCryptoTransformDisposed);
                    algorithm.Decrypt(buffer, 0);
                    Assert.True(algorithm.IsCryptoTransformDisposed);
                    algorithm.Decrypt(buffer, 0, 0);
                    Assert.True(algorithm.IsCryptoTransformDisposed);

                    Stream sourceStream = new MemoryStream();
                    Stream targetStream = new MemoryStream();
                    Assert.True(algorithm.IsCryptoTransformDisposed);
                    algorithm.Decrypt(sourceStream, targetStream);
                    Assert.True(algorithm.IsCryptoTransformDisposed);
                    algorithm.Decrypt(sourceStream, targetStream, 1024);
                    Assert.True(algorithm.IsCryptoTransformDisposed);
                }
            }

            [Fact]
            public void ShouldThrowArgumentNullException_WhenAlgorithmIsNull()
            {
                SymmetricAlgorithm algorithm = null;
                Assert.Throws<ArgumentNullException>(nameof(algorithm), () => SymmetricAlgorithmExtensions.Decrypt(algorithm, new byte[0]));
                Assert.Throws<ArgumentNullException>(nameof(algorithm), () => SymmetricAlgorithmExtensions.Decrypt(algorithm, new byte[0], 0));
                Assert.Throws<ArgumentNullException>(nameof(algorithm), () => SymmetricAlgorithmExtensions.Decrypt(algorithm, new byte[0], 0, 0));

                Stream stream = new MemoryStream();
                Assert.Throws<ArgumentNullException>(nameof(algorithm), () => SymmetricAlgorithmExtensions.Decrypt(algorithm, stream, stream));
                Assert.Throws<ArgumentNullException>(nameof(algorithm), () => SymmetricAlgorithmExtensions.Decrypt(algorithm, stream, stream, 1024));
            }

            [Fact]
            public void ShouldThrowArgumentNullException_WhenArrayIsNull()
            {
                SymmetricAlgorithm algorithm = new SimpleReversingSymmetricAlgorithm();
                byte[] array = null;
                Assert.Throws<ArgumentNullException>(nameof(array), () => SymmetricAlgorithmExtensions.Decrypt(algorithm, array));
                Assert.Throws<ArgumentNullException>(nameof(array), () => SymmetricAlgorithmExtensions.Decrypt(algorithm, array, 0));
                Assert.Throws<ArgumentNullException>(nameof(array), () => SymmetricAlgorithmExtensions.Decrypt(algorithm, array, 0, 0));
            }

            [Theory]
            [InlineData(0, -1)]
            [InlineData(0, 1)]
            public void ShouldThrowArgumentOutOfRangeException_WhenOffsetIsInvalid(int size, int offset)
            {
                SymmetricAlgorithm algorithm = new SimpleReversingSymmetricAlgorithm();
                byte[] array = new byte[size];
                Assert.Throws<ArgumentOutOfRangeException>(nameof(offset), () => SymmetricAlgorithmExtensions.Decrypt(algorithm, array, offset));
                Assert.Throws<ArgumentOutOfRangeException>(nameof(offset), () => SymmetricAlgorithmExtensions.Decrypt(algorithm, array, offset, 0));
            }

            [Theory]
            [InlineData(0, 0, -1)]
            [InlineData(1, 1, 1)]
            public void ShouldThrowArgumentOutOfRangeException_WhenCounttIsInvalid(int size, int offset, int count)
            {
                SymmetricAlgorithm algorithm = new SimpleReversingSymmetricAlgorithm();
                byte[] array = new byte[size];
                Assert.Throws<ArgumentOutOfRangeException>(nameof(count), () => SymmetricAlgorithmExtensions.Decrypt(algorithm, array, offset, count));
            }

            [Fact]
            public void ShouldThrowArgumentNullException_WhenSourceStreamIsNull()
            {
                Stream sourceStream = null;
                Stream targetStream = new MemoryStream();
                int bufferSize = 1024;
                Assert.Throws<ArgumentNullException>(nameof(sourceStream), () => SymmetricAlgorithmExtensions.Decrypt(new SimpleReversingSymmetricAlgorithm(), sourceStream, targetStream));
                Assert.Throws<ArgumentNullException>(nameof(sourceStream), () => SymmetricAlgorithmExtensions.Decrypt(new SimpleReversingSymmetricAlgorithm(), sourceStream, targetStream, bufferSize));
            }

            [Fact]
            public void ShouldThrowArgumentNullException_WhenTargetStreamIsNull()
            {
                Stream sourceStream = new MemoryStream();
                Stream targetStream = null;
                int bufferSize = 1024;
                Assert.Throws<ArgumentNullException>(nameof(targetStream), () => SymmetricAlgorithmExtensions.Decrypt(new SimpleReversingSymmetricAlgorithm(), sourceStream, targetStream));
                Assert.Throws<ArgumentNullException>(nameof(targetStream), () => SymmetricAlgorithmExtensions.Decrypt(new SimpleReversingSymmetricAlgorithm(), sourceStream, targetStream, bufferSize));
            }

            [Theory]
            [InlineData(-1)]
            [InlineData(0)]
            public void ShouldThrowArgumentOutOfRangeException_WhenBufferSizeIsInvalid(int bufferSize)
            {
                Stream sourceStream = new MemoryStream();
                Stream targetStream = new MemoryStream();
                Assert.Throws<ArgumentOutOfRangeException>(nameof(bufferSize), () => SymmetricAlgorithmExtensions.Decrypt(new SimpleReversingSymmetricAlgorithm(), sourceStream, targetStream, bufferSize));
            }

            [Fact]
            public void StreamParametersShouldNotBeDisposed()
            {
                using (SimpleReversingSymmetricAlgorithm algorithm = new SimpleReversingSymmetricAlgorithm())
                {
                    Stream sourceStream = new MemoryStream();
                    Stream targetStream = new MemoryStream();
                    algorithm.Decrypt(sourceStream, targetStream);
                    Assert.True(sourceStream.CanRead);
                    Assert.True(targetStream.CanWrite);
                }
            }

            #endregion Methods
        }
    }
}