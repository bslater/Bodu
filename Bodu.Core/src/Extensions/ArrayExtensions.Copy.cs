namespace Bodu.Extensions
{
	public static partial class ArrayExtensions
	{
		/// <summary>
		/// Creates a new array and copies all elements from the source array to the new array.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the array.</typeparam>
		/// <param name="array">The source array containing the elements to copy.</param>
		/// <returns>A new array containing all elements from the <paramref name="array" />.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="array" /> is <see langword="null" />.
		/// </exception>
		public static T[] Copy<T>(this T[] array)
			=> array.Slice(0, array?.Length ?? 0);
	}
}